from __future__ import print_function

import numpy as np
import struct

import solvers
from util import *

class ArmConfig:
    """Holds an arm's proportions, limits and other configuration data"""
    def __init__(self,
                 main_length = 148.4,
                 forearm_length = 160,
                 linkage_length = 155,
                 lower_actuator_length = 65,
                 upper_actuator_length = 54.4,
                 wrist_length = 90.6,
                 shoulder_offset = [-10, 18.568]):
        self.main_length = main_length
        self.forearm_length = forearm_length
        self.linkage_length = linkage_length
        self.lower_actuator_length = lower_actuator_length
        self.upper_actuator_length = upper_actuator_length
        self.wrist_length = wrist_length;
        self.shoulder_offset = shoulder_offset


class ArmPose:
    structFormat = 'fffff'

    """Defines a physical configuration of the arm"""
    def __init__(self,
                 arm_config,
                 swing_angle,
                 shoulder_angle,
                 actuator_angle,
                 elbow_angle,
                 elbow2D,
                 wrist2D,
                 effector2D,
                 effector):
        self.cfg = arm_config
        self.swing_angle = swing_angle
        self.shoulder_angle = shoulder_angle
        self.actuator_angle = actuator_angle
        self.elbow_angle = elbow_angle
        # Joints in the arm
        shoulder = rotate(self.cfg.shoulder_offset, swing_angle)
        self.shoulder2D = [self.cfg.shoulder_offset[1], 0]
        self.shoulder = [shoulder[0], 0, shoulder[1]]
        self.wrist2D = wrist2D
        self.effector2D = effector2D
        self.effector = effector
        # Construct the 3D elbow & wrist positions from the 2D (planar) IK
        # solution
        arm_vec = effector - self.shoulder
        arm_vec[1] = 0
        self.elbow2D = elbow2D
        self.elbow = self.shoulder + normalize(arm_vec)*elbow2D[0]
        self.elbow[1] = elbow2D[1]
        self.wrist = self.effector - normalize(arm_vec)*arm_config.wrist_length
        # Find the servo target angles for the pose
        self.servo_elevator = 178.21 - degrees(self.shoulder_angle)
        self.servo_actuator = degrees(self.actuator_angle) + 204.78
        self.servo_swing = 150 - degrees(self.swing_angle)
        # TODO wrist servo pose
        self.servo_wrist_x = 150
        self.servo_wrist_y = 150
        # Validate pose angles
        self.arm_diff_angle = degrees(shoulder_angle - actuator_angle)
        self.clear_diff = self.arm_diff_angle > 44
        self.clear_elevator = self.servo_elevator > 60
        self.clear_actuator = self.servo_actuator > 95

    def fullClearance(self):
        return self.clear_diff and self.clear_elevator and self.clear_actuator

    def serialize(self):
        """Returns a packed struct holding the pose information"""
        return struct.pack(
            ArmPose.structFormat,
            self.swing_angle,
            self.shoulder_angle,
            self.elbow_angle,
            # TODO wrist yaw
            0.0,
            # TODO wrist pitch
            0.0
        )

class ArmController:
    def __init__(self,
                 servo_swing,
                 servo_shoulder,
                 servo_elbow,
                 servo_wrist_x,
                 servo_wrist_y,
                 arm_config,
                 motion_enable = False):
        # Solvers are responsible for calculating the target servo positions to
        # reach a given goal position
        self.ik = solvers.IKSolver(
            arm_config.main_length,
            arm_config.forearm_length,
            arm_config.wrist_length,
            arm_config.shoulder_offset)
        self.physsolver = solvers.PhysicalSolver(
            arm_config.main_length,
            arm_config.linkage_length,
            arm_config.lower_actuator_length,
            arm_config.upper_actuator_length)
        # Servos
        self.servos = {}
        self.servos["swing"] = servo_swing
        self.servos["shoulder"] = servo_shoulder
        self.servos["elbow"] = servo_elbow
        self.servos["wrist_x"] = servo_wrist_x
        self.servos["wrist_y"] = servo_wrist_y
        for key, servo in self.servos.iteritems():
            if servo is None:
                print ("Warning: {0} servo not connected".format(key))
            else:
                servo.setGoalSpeed(0.4)
        # Store parameters
        self.motion_enable = False
        self.cfg = arm_config
        # Dirty flags for stored poses
        self.ik_pose = None
        self.ik_dirty = True
        self.real_pose = None
        self.real_dirty = True
        # Current target pose
        self.target_pose = None

    def disable_movement(self):
        self.motion_enable = False

    def enable_movement(self):
        print ("Warning: Arm enabled")
        self.motion_enable = True

    def setWristGoalPosition(self, pos):
        self.ik.setGoal(pos)
        self.ik_dirty = True

    def getIKPose(self):
        if self.ik_dirty and self.ik.valid:
            # Construct geometry of arm from IK state
            main_arm = self.ik.elbow - self.ik.originpl
            arm_vert_angle = sigangle(main_arm, vertical)
            forearm = self.ik.wristpl - self.ik.elbow
            elbow_angle = angle_between(main_arm, forearm)
            # Solve actuator angle for given elbow angle
            # Base angle is between the main arm and actuator
            base_angle = self.physsolver.inverse_forearm(elbow_angle)
            actuator_angle = arm_vert_angle - base_angle

            self.ik_pose = ArmPose(
                self.cfg,
                swing_angle = self.ik.swing,
                # angles from vertical
                shoulder_angle = arm_vert_angle,
                actuator_angle = actuator_angle,
                # angle between the main arm and forearm
                elbow_angle = elbow_angle,
                elbow2D = self.ik.elbow,
                wrist2D = self.ik.wristpl,
                effector2D = self.ik.goalpl,
                effector = self.ik.goal
            )
        return self.ik_pose

    def setTargetPose(self, new_pose):
        self.target_pose = new_pose

    def tick(self):
        if self.target_pose is not None:
            # Drive servos
            if self.servos['swing'] is not None:
                self.servos['swing'].setGoalPosition(self.target_pose.servo_swing)
            if self.servos['shoulder'] is not None:
                self.servos['shoulder'].setGoalPosition(self.target_pose.servo_elevator)
            if self.servos['elbow'] is not None:
                self.servos['elbow'].setGoalPosition(self.target_pose.servo_actuator)

            if self.servos['wrist_x'] is not None:
                angle = 150 + degrees(self.target_pose.swing_angle)
                self.servos['wrist_x'].setGoalPosition(self.target_pose.servo_wrist_y)
            if self.servos['wrist_y'] is not None:
                self.servos['wrist_y'].setGoalPosition(self.target_pose.servo_wrist_x)
