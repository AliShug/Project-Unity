from __future__ import print_function

import sys
import time
import serial
import socket
import select
import struct

import numpy as np
import pygame as pyg

from pid import PIDControl
import Protocol
from solvers import IKSolver, PhysicalSolver
import window
import views
import litearm
import capsense

from util import *

RECV_PORT = 14001
TRAN_PORT = 14002

MAX_SPEED = 800.0
ACCEL = 1600.0
DECEL = 1.0

CS_PORT = 'COM4'

import pdb;

class Kinectics:
    def findSerial(self):
        ports = ['COM{0}'.format(i+1) for i in range(256)]
        results = []
        for port in ports:
            try:
                comm = serial.Serial(port)
                comm.close()
                results.append(port)
            except (OSError, serial.SerialException):
                pass
        if len(results) > 0:
            return results[0]
        else:
            print ("No valid serial port found")
            return None

    def connectController(self):
        port = 'COM3'
        try:
            return Protocol.serialConnect(port, 250000)
        except:
            return None

    def findServos(self, comm):
        """Retrieves and processes the list of available (responsive) servos
        attached to the arm's Arduino controller. Returns a dictionary mapping
        from servo functions to Servo() objects, or None if no matching servo
        was found for a particular function"""
        servos = {
            'swing': None,
            'actuator': None,
            'shoulder': None,
            'wrist_x': None,
            'wrist_y': None
        }
        # Mapping of IDs->functions
        v1_IDs = {
            0 : 'swing',
            1 : 'actuator',
            2 : 'shoulder'
        }
        v2_IDs = {
            1 : 'wrist_x',
            0 : 'wrist_y'
        }
        # Servos retrieved from Arduino
        found = Protocol.findServos(comm)
        i = 0
        for v1servo in found['v1']:
            servos[v1_IDs[i]] = v1servo
            i += 1
        j = 0
        for v2servo in found['v2']:
            servos[v2_IDs[j]] = v2servo
            j += 1
        # Return our output servo dictionary
        return servos

    def __init__(self):
        self.stopped = False

        # Bind our comms socket
        self.bindSocket()

        # Performance tracking
        self.ik_time_accum = 0
        self.ik_time_counter = 0
        self.render_time_accum = 0
        self.render_time_counter = 0
        self.serial_time_accum = 0
        self.serial_time_counter = 0
        self.perflog = open('Logs/{0}.perflog'.format(time.time()), 'w')
        self.time_start = time.clock()

        # Find the available servos through the attached board
        comm = self.connectController()
        self.servos = self.findServos(comm)


        window.InitRenderer()
        self.r = window.Renderer(pyg.display.set_mode([1200, 600], pyg.DOUBLEBUF|pyg.HWSURFACE))
        pyg.display.set_caption("IK Control Test")

        # The arm's controller - encapsulates IK and arm control
        self.arm = litearm.ArmController(
            servo_swing = self.servos['swing'],
            servo_shoulder = self.servos['shoulder'],
            servo_elbow = self.servos['actuator'],
            servo_wrist_x = self.servos['wrist_x'],
            servo_wrist_y = self.servos['wrist_y'],
            # use the default config
            arm_config = litearm.ArmConfig())

        # Capacitive sensor on connected UNO
        self.capSense = capsense.CapacitiveSensor('COM4')

        self.sideView = views.PlaneView(width=10)
        self.realSideView = views.PlaneView(width=5, color=gray)
        self.topView = views.TopView(width=12)
        self.realTopView = views.TopView(width=6, color=gray)

        self.armAngle = 0
        self.pid = PIDControl(0.2, 0, 0)

        self.arm.pollServos()
        realPose = self.arm.getRealPose()
        if realPose is not None:
            self.curGoal = self.arm.getRealPose().effector
        else:
            self.curGoal = np.array([0., 50., 200.])
        self.ikTarget = np.array(self.curGoal)
        self.lastValidGoal = np.array(self.curGoal)
        self.curDir = [0.0, 0.0]
        self.goalNormal = [0, 0, 1]
        self.ikOffset = np.array([0.,0.,0.])

        self.lerpSpeed = 0
        self.lerpTimer = time.clock()
        self.lastPose = None

        # render the reachable area
        self.sideView.renderReachableVolume(self.arm)

    def stop(self):
        self.perflog.write('ik_avg {0}\n'.format(self.ik_time_accum/self.ik_time_counter))
        self.perflog.write('serial_avg {0}\n'.format(self.serial_time_accum/self.serial_time_counter))
        self.perflog.write('render_avg {0}\n'.format(self.render_time_accum/self.render_time_counter))
        self.perflog.write('runtime {0}\n'.format(time.clock()-self.time_start))
        self.perflog.close()
        self.sockIn.close()
        self.sockOut.close()
        pyg.quit()
        self.stopped = True;

    def bindSocket(self):
        # We use UDP (datagrams) since we don't really care if the messages
        # arrive - just transmit and hope for the best
        self.sockIn = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self.sockOut = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self.sockIn.bind(('localhost', RECV_PORT))
        self.sockOut.connect(('localhost', TRAN_PORT))

    def tickComms(self):
        """(Socket comms) Check for incoming datagrams and send our updates"""
        while len(select.select([self.sockIn],[],[],0)[0]) > 0:
            # Ready to receive
            try:
                raw = self.sockIn.recv(4096)
                data = struct.unpack('?ffffff', raw)
                # enable or disable the physical arm
                self.arm.enableMovement(data[0])
                goalPos = np.array(data[1:4])
                self.goalNormal = np.array(data[4:])
                #print(data)
                newGoal = goalPos*1000
                self.curGoal = np.array(newGoal)
            except socket.error as err:
                print ("Socket error: {0}".format(err))
        #sensor = struct.pack('i', 0)
        self.capSense.updateReadings()
        val = self.capSense.latest()
        try:
            sensor = struct.pack('i', int(self.capSense.latest()))
        except:
            sensor = '\0\0\0\0'
        realPose = self.arm.getRealPose()
        if realPose is not None:
            self.sockOut.send(realPose.serialize() + sensor)
        else:
            self.sockOut.send(self.arm.getIKPose().serialize() + sensor)

    def lerpIKTarget(self):
        # Scales with time delta
        now = time.clock()
        dt = now - self.lerpTimer
        self.lerpTimer = now

        delta = np.subtract(self.curGoal, self.ikTarget)
        dist = np.linalg.norm(delta)
        if (dist < 1):
            self.lerpSpeed = ACCEL*0.03
            return
        else:
            # acceleration
            self.lerpSpeed += ACCEL*dt
            curMax = MAX_SPEED*(0.03 + 0.97*min(1, 0.01*DECEL*dist))
            if self.lerpSpeed > curMax:
                self.lerpSpeed = curMax
            self.ikTarget = self.ikTarget + normalize(delta)*min(dist, dt*self.lerpSpeed)
        # Disable motor error checks at high speeds
        if self.lerpSpeed > 0.11*ACCEL:
            self.arm.clearPositionError()


    def getGoalOffset(self):
        """3D Offset between measured position and goal position"""
        pose = self.arm.getRealPose()
        return np.subtract(self.ikTarget, pose.effector)

    def tick(self):
        if self.stopped:
            return

        # Polling/update loop
        for event in pyg.event.get():
            if event.type == pyg.QUIT:
                self.stop()
                return
            elif (event.type == pyg.MOUSEBUTTONDOWN
                    or event.type == pyg.MOUSEMOTION):
                if pyg.mouse.get_pressed()[0]:
                    if event.pos[0] < 600:
                        # lock direction on start of interaction
                        if event.type == pyg.MOUSEBUTTONDOWN:
                            goal_topdown = [self.curGoal[0], self.curGoal[2]]
                            self.curDir = normalize(goal_topdown)
                        sidegoal = views.unpt_l(event.pos)
                        radial = sidegoal[0]
                        if radial < 1:
                            radial = 1
                        vec = radial * self.curDir
                        self.curGoal[0] = vec[0]
                        self.curGoal[1] = sidegoal[1]
                        self.curGoal[2] = vec[1]
                    else:
                        topgoal = views.unpt_r(event.pos)
                        self.curGoal[0] = topgoal[0]
                        self.curGoal[2] = topgoal[1]
                    if self.curGoal[2] < 1:
                        self.curGoal[2] = 1
            elif event.type == pyg.KEYDOWN:
                if event.key == pyg.K_SPACE:
                    self.arm.enableMovement(True)

        # Clear the render canvas
        self.r.surf.fill(white)

        # Simulated arm swing - PID test
        self.armAngle -= 0.01 * self.pid.stepOutput(self.armAngle)

        # Lerp towards target
        self.lerpIKTarget()
        # Find offset of real position from goal position
        if (self.arm.getRealPose() is not None):
            self.ikOffset = self.ikOffset*0.9 + self.getGoalOffset()*0.1
        # IK - calculate swing and elbow pos using goal pos
        timer = time.clock()
        self.arm.setWristGoalPosition(self.ikTarget)
        self.arm.setWristGoalDirection(self.goalNormal)
        self.ik_time_accum += time.clock()-timer
        self.ik_time_counter += 1

        pose = self.arm.getIKPose()
        self.pid.setTarget(degrees(pose.swing_angle))

        # Incorporate wrist orientation to pose
        # wrist_x, wrist_y = self.wristFromNormal(self.goalNormal)
        # pose.servo_wrist_x = wrist_x;
        # pose.servo_wrist_y = wrist_y;

        # Draw the PID test
        armVec = rotate(vertical, radians(self.armAngle)) * 35
        self.r.drawLine(views.pt_r([0, 0]), views.pt_r(armVec), black)
        text = "Actual {d:.3f} deg".format(d = self.armAngle)
        self.r.drawText(text, gray, [600, 60])

        # Find the current servo positions
        self.arm.pollServos()

        # Calculate pose
        if self.lastPose is None:
            self.lastPose = pose
        display_pose = self.lastPose

        if pose.checkClearance():
            # Update display pose
            display_pose = pose
            self.lastPose = pose
            # Drive the main arm
            self.arm.setTargetPose(pose)
            timer = time.clock()
            self.arm.tick()
            self.serial_time_accum += time.clock()-timer
            self.serial_time_counter += 1
            # Update the last known valid goal
            self.lastValidGoal = np.array(self.ikTarget)
        else:
            # Revert to last good goal position
            self.curGoal = np.array(self.lastValidGoal)
            self.ikTarget = np.array(self.lastValidGoal)

        # Update the views
        realPose = self.arm.getRealPose()
        self.drawViews(display_pose)
        if realPose is not None:
            self.realSideView.draw(realPose, self.r)
            self.realTopView.draw(realPose, self.r)
        pyg.display.flip()

        self.tickComms()

    def displayServoPositions(self, col, pos):
        i = 0
        for (name,servo) in self.servos.iteritems():
            if servo is None:
                continue
            text = "{name} [{id}]: {d:.3f} deg".format(
                name = name,
                d = servo.data['pos'],
                id = servo.id)
            self.r.drawText(text, col, [pos[0], pos[1]+i*20])
            i += 1

    def drawViews(self, pose):
        timer = time.clock()
        self.sideView.draw(pose, self.r)
        self.topView.draw(pose, self.r)
        # Display critical pose angles
        text = "Elbow differential {0:.3f} deg".format(pose.armDiffAngle())
        self.r.drawText(text, blue if pose.checkDiff() else red, [40, 520])

        text = "Elevator servo target {0:.3f} deg".format(pose.getServoElevator())
        self.r.drawText(text, blue if pose.checkElevator() else red, [40, 540])

        text = "Actuator servo target {0:.3f} deg".format(pose.getServoActuator())
        self.r.drawText(text, blue if pose.checkActuator() else red, [40, 560])

        if pose.checkPositioning():
            self.r.drawText("Pose OK", blue, [40, 580])
        else:
            self.r.drawText("Pose Invalid", red, [40, 580])
        # Display servo positions
        self.displayServoPositions(black, [400, 20])
        # Store the render timing
        self.render_time_accum += time.clock() - timer
        self.render_time_counter += 1


if __name__ == "__main__":
    app = Kinectics()
    while not app.stopped:
        app.tick()
