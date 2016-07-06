from __future__ import print_function

import sys
import time
import serial
import socket
import select

import numpy as np
import pygame as pyg

from pid import PIDControl
from Protocol import Servo
from solvers import IKSolver, PhysicalSolver
import window
import views
import litearm

from util import *

armLen = 148.4
foreLen = 160

RECV_PORT = 14001
TRAN_PORT = 14002

def find_serial():
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

class Kinectics:
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

        # Log some performance info
        self.perflog = open('Logs/{0}.perflog'.format(time.time()), 'w')

        self.time_start = time.clock()
        port = find_serial()
        if port is not None:
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

            comm.timeout = 3

        # Find all the attached servos
        # Specify a map of servo functions to IDs;
        x1ids = {
            0 : 'swing',
            1 : 'actuator',
            2 : 'shoulder'
        }
        x2ids = {
            1 : 'wrist_x',
            0 : 'wrist_y'
        }
        self.servos = {
            'swing': None,
            'actuator': None,
            'shoulder': None,
            'wrist_x': None,
            'wrist_y': None
        }

        # Retrieves a list of servos over serial, one per line
        if port is not None:
            comm.write("list")
            read = comm.readline()
            x1num = int(read.split('=')[1])
            for i in range(x1num):
                str = comm.readline()
                if str.startswith("ERROR"):
                    print ("Hardware Error during X1 servo listing: ", str)
                else:
                    id = int(str)
                    self.servos[x1ids[id]] = Servo(comm, 1, id)
            read = comm.readline()
            x2num = int(read.split('=')[1])
            for i in range(x2num):
                str = comm.readline()
                if str.startswith("ERROR"):
                    print ("Hardware Error during X2 servo listing: ", str)
                else:
                    id = int(str)
                    self.servos[x2ids[id]] = Servo(comm, 2, id)
            self.perflog.write('found_servos {0}\n'.format(time.clock()-timer))

            comm.timeout = 0.01

        window.InitRenderer()
        self.r = window.Renderer(pyg.display.set_mode([800, 600], pyg.DOUBLEBUF|pyg.HWSURFACE))
        pyg.display.set_caption("IK Control Test")

        # The arm'comm controller - encapsulates IK and arm control
        self.arm = litearm.ArmController(
            servo_swing = self.servos['swing'],
            servo_shoulder = self.servos['shoulder'],
            servo_elbow = self.servos['actuator'],
            servo_wrist_x = self.servos['wrist_x'],
            servo_wrist_y = self.servos['wrist_y'],
            # default config
            arm_config = litearm.ArmConfig())

        # Visualization
        self.sideView = views.PlaneView(width=10)
        self.topView = views.TopView(width=12)

        self.armAngle = 0
        self.pid = PIDControl(5, 0, 0)

        self.curGoal = [0, 50, 200]
        self.curDir = np.array([0, 0])

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
        """Check for incoming datagrams and send our updates"""
        while len(select.select([self.sockIn],[],[],0)[0]) > 0:
            # Ready to receive
            try:
                print (self.sockIn.recv(4096))
            except socket.error as err:
                print ("Socket error: {0}".format(err))
        self.sockOut.send(self.arm.getIKPose().serialize())


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
                    if event.pos[0] < 400:
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

        # Update the views
        timer = time.clock()
        self.r.surf.fill(white)
        self.sideView.draw(pose, self.r)
        self.topView.draw(pose, self.r)
        self.render_time_accum += time.clock() - timer
        self.render_time_counter += 1

        # Draw the PID test
        armVec = rotate(vertical, radians(self.armAngle)) * 35
        self.r.drawLine(views.pt_r([0, 0]), views.pt_r(armVec), black)
        text = "Actual {d:.3f} deg".format(d = self.armAngle)
        self.r.drawText(text, gray, [400, 60])

        # Write out the current servo positions
        i = 0
        for servo in self.servos.values():
            if servo is None: continue
            text = "Servo {id} {d:.3f} deg".format(
                d = servo.data['pos'],
                id = servo.id
            )
            self.r.drawText(text, gray, [550, 20+i*20])
            i += 1

        # Serial updates
        for servo in self.servos.values():
            if servo is None: continue
            pos = servo.getPosition()
            if pos is not None:
                servo.data['pos'] = pos
            # no printing..

        # Serial Control Test

        # Display actuator visually
        vec = rotate(vertical, pose.actuator_angle)
        self.r.drawLine(views.pt_l([0, 0]), views.pt_l(vec*30), blue)

        # Swing angle from IK pose
        swing = 150 - degrees(pose.swing_angle)

        # Convert arm from vertical offset to servo target angle
        left_targ_rot = views.getArmFromVerticalOffset(degrees(pose.shoulder_angle))
        # Convert actuator from vertical offset to servo target angle
        right_targ_rot = views.getActuatorFromVerticalOffset(degrees(pose.actuator_angle))

        # Base angle is between the main arm and actuator
        base_angle = degrees(pose.shoulder_angle - pose.actuator_angle)

        # Display some feedback
        base_valid = True
        col = blue
        if base_angle < 44:
            arm_valid = False
            col = red
        text = "Arm/actuator differential {0:.3f} deg".format(base_angle)
        self.r.drawText(text, col, [40, 520])
        arm_valid = True
        col = blue
        if left_targ_rot < 60:
            arm_valid = False
            col = red
        text = "Arm target {0:.3f} deg".format(left_targ_rot)
        self.r.drawText(text, col, [40, 540])
        actuator_valid = True
        col = blue
        if right_targ_rot < 95:
            actuator_valid = False
            col = red
        text = "Actuator target {0:.3f} deg".format(right_targ_rot)
        self.r.drawText(text, col, [40, 560])

        # Drive the main arm
        if arm_valid and actuator_valid and base_valid:
            timer = time.clock()
            self.arm.tick()
            self.serial_time_accum += time.clock()-timer
            self.serial_time_counter += 1

        pyg.display.flip()
        # if comm.in_waiting > 0:
        #     sys.stdout.write("Extraneous read: {0}".format(comm.readline()))

        self.tickComms()

if __name__ == "__main__":
    app = Kinectics()
    while not app.stopped:
        app.tick()
