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
from Protocol import Servo
from solvers import IKSolver, PhysicalSolver
import window
import views
import litearm

from util import *

RECV_PORT = 14001
TRAN_PORT = 14002

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
        port = self.findSerial()
        if port is None:
            return None
        else:
            comm = serial.Serial()
            comm.baudrate = 250000
            comm.port = port

            print ("Waiting for board on {0}".format(port), end=" ")
            comm.timeout = 5
            result = ''
            attempts = 1
            while result == '':
                print ("Attempt {0}".format(attempts))
                comm.close()
                comm.open()
                result = comm.readline()
                attempts += 1

            if result.startswith("CommTest READY"):
                print ("OK")
            else:
                print ("ERROR: PC/Board software mismatch")
                print (result)
                self.perflog.close()
                sys.exit(0);
            timer = time.clock()
            self.perflog.write('connected {0}\n'.format(timer-self.time_start))
            return comm

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
        x1ids = {
            0 : 'swing',
            1 : 'actuator',
            2 : 'shoulder'
        }
        x2ids = {
            1 : 'wrist_x',
            0 : 'wrist_y'
        }
        # Retrieves a list of servos over serial, one per line
        if comm is not None:
            # Set relatively high timeout for this section of board comms
            comm.timeout = 3
            comm.write("list")
            read = comm.readline()
            x1num = int(read.split('=')[1])
            for i in range(x1num):
                str = comm.readline()
                if str.startswith("ERROR"):
                    print ("Hardware Error during X1 servo listing: ", str)
                else:
                    id = int(str)
                    servos[x1ids[id]] = Servo(comm, 1, id)
            read = comm.readline()
            x2num = int(read.split('=')[1])
            for i in range(x2num):
                str = comm.readline()
                if str.startswith("ERROR"):
                    print ("Hardware Error during X2 servo listing: ", str)
                else:
                    id = int(str)
                    servos[x2ids[id]] = Servo(comm, 2, id)
            self.perflog.write('found_servos {0}\n'.format(time.clock()-timer))
            # Revert to shorter timeout
            comm.timeout = 0.01
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

        self.sideView = views.PlaneView(width=10)
        self.topView = views.TopView(width=12)

        self.armAngle = 0
        self.pid = PIDControl(5, 0, 0)

        self.curGoal = [0, 50, 200]
        self.curDir = np.array([0, 0])

        self.last_pose = None

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
                data = struct.unpack('fff', raw)
                print(data)
                newGoal = np.array(data)*1000
                print(newGoal)
                self.curGoal = newGoal

            except socket.error as err:
                print ("Socket error: {0}".format(err))
        self.sockOut.send(self.arm.getIKPose().serialize())

    def updateServoPositions(self):


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

        # Clear the render canvas
        self.r.surf.fill(white)

        # Simulated arm swing - PID test
        self.armAngle -= 0.01 * self.pid.stepOutput(self.armAngle)

        # IK - calculate swing and elbow pos using goal pos
        timer = time.clock()
        self.arm.setWristGoalPosition(self.curGoal)
        self.ik_time_accum += time.clock()-timer
        self.ik_time_counter += 1

        pose = self.arm.getIKPose()
        self.arm.setTargetPose(pose)
        self.pid.setTarget(degrees(pose.swing_angle))

        # Draw the PID test
        armVec = rotate(vertical, radians(self.armAngle)) * 35
        self.r.drawLine(views.pt_r([0, 0]), views.pt_r(armVec), black)
        text = "Actual {d:.3f} deg".format(d = self.armAngle)
        self.r.drawText(text, gray, [600, 60])

        self.updateServoPositions()
        self.displayServoPositions()

        # Display critical pose angles
        text = "Elbow differential {0:.3f} deg".format(pose.arm_diff_angle)
        self.r.drawText(text, blue if pose.clear_diff else red, [40, 520])

        text = "Elevator servo target {0:.3f} deg".format(pose.servo_elevator)
        self.r.drawText(text, blue if pose.clear_elevator else red, [40, 540])

        text = "Actuator servo target {0:.3f} deg".format(pose.servo_actuator)
        self.r.drawText(text, blue if pose.clear_actuator else red, [40, 560])

        # Calculate pose
        if self.last_pose is None:
            self.last_pose = pose
        display_pose = self.last_pose

        if pose.fullClearance():
            # Update display pose
            display_pose = pose
            self.last_pose = pose
            # Drive the main arm
            timer = time.clock()
            self.arm.tick()
            self.serial_time_accum += time.clock()-timer
            self.serial_time_counter += 1

        # Update the views
        self.drawViews(display_pose)
        pyg.display.flip()

        self.tickComms()

    def displayServoPositions(self):
        i = 0
        for servo in self.servos.values():
            if servo is None:
                continue
            text = "Servo {id} {d:.3f} deg".format(
                d = servo.data['pos'],
                id = servo.id)
            self.r.drawText(text, gray, [550, 20+i*20])
            i += 1

    def drawViews(self, pose):
        timer = time.clock()
        self.sideView.draw(pose, self.r)
        self.topView.draw(pose, self.r)
        self.render_time_accum += time.clock() - timer
        self.render_time_counter += 1

if __name__ == "__main__":
    app = Kinectics()
    while not app.stopped:
        app.tick()
