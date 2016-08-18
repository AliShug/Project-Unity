import pygame as pyg
import numpy as np

from util import *

def pt_l(p):
    point = np.array(p).astype(int)
    return (150 + point[0], 300 - point[1])

def pt_r(p):
    point = np.array(p).astype(int)
    return (900 + point[0], 450 - point[1])


def unpt_l(p):
    return np.array([p[0] - 150, 300 - p[1]])

def unpt_r(p):
    return np.array([p[0] - 900, 450 - p[1]])


def getArmVerticalOffset(val):
    return 28.21 + (150 - val)

def getActuatorVerticalOffset(val):
    return val - 150 - 54.78

# def getArmFromVerticalOffset(val):
#     return 150 + (28.21 - val)
#
# def getActuatorFromVerticalOffset(val):
#     return val + 150 + 54.78

class PlaneView:
    def __init__(self, color=blue, width=6, positioning='normal'):
        self.color = color
        self.line_width = width
        self.backSurface = None
        self.backSize = 500
        self.reachableXOffset = 90
        self.positioning = positioning

    def renderReachableVolume(self, arm, res=50):
        offset = self.reachableXOffset
        size = (430-offset, self.backSize)
        renderSurface = pyg.Surface((res, res))

        y_step = size[1]/float(res)
        x_step = size[0]/float(res)
        for y in xrange(res):
            for x in xrange(res):
                pos = [0, (y-res/2)*y_step, offset + x*x_step]
                arm.setWristGoalPosition(pos)
                if arm.ik.valid:
                    pose = arm.getIKPose()
                    if pose is not None:
                        test = pose.checkClearance()
                    else:
                        test = False
                else:
                    test = False

                if test:
                    # reachable pixel
                    renderSurface.set_at((x,res-y-1), [210,255,210])
                else:
                    # unreachable
                    renderSurface.set_at((x,res-y-1), white)
        # scale to true size
        self.backSurface = pyg.transform.smoothscale(renderSurface, size)


    def draw(self, pose, r):
        if self.backSurface is not None:
            # render the reachable area
            r.surf.blit(self.backSurface, pt_l([self.reachableXOffset,self.backSize/2]))

        # Y axis
        r.drawLine(pt_l([0,0]), pt_l([0,150]), green)
        r.drawText('y', green, pt_l([-10,170]))
        # Z axis from side
        zvec = rotate([0,1], pose.swing_angle)
        r.drawLine(pt_l([0,0]), pt_l([zvec[1]*150,0]), blue)
        r.drawText('z', blue, pt_l([zvec[1]*150,0]))
        # X axis from side
        xvec = rotate([-1,0], pose.swing_angle)
        r.drawLine(pt_l([0,0]), pt_l([xvec[1]*150,0]), red)
        r.drawText('x', red, pt_l([xvec[1]*150,0]))

        # Base
        r.drawRect(pt_l([-30, -25]), [60, 10], gray)

        # Components
        main_arm = pose.elbow2D - pose.shoulder2D
        fore_arm = pose.wrist2D - pose.elbow2D
        effector = pose.effector2D - pose.wrist2D

        vec = rotate(vertical, pose.actuator_angle)
        actuator = vec*pose.cfg.lower_actuator_length
        upper_actuator = -normalize(fore_arm)*pose.cfg.upper_actuator_length

        col = self.color
        # if not self.ik.valid:
        #     col = red

        # Display actuation mechanism visually
        r.drawLine(pt_l(pose.shoulder2D),
                   pt_l(pose.shoulder2D + actuator),
                   col,
                   self.line_width / 2)
        r.drawLine(pt_l(pose.elbow2D),
                   pt_l(pose.elbow2D + upper_actuator),
                   col,
                   self.line_width / 2)
        r.drawLine(pt_l(pose.shoulder2D + actuator),
                   pt_l(pose.elbow2D + upper_actuator),
                   col,
                   self.line_width / 2)
        # IK Rig
        r.drawLine(pt_l(pose.shoulder2D), pt_l(pose.elbow2D), col, self.line_width)
        r.drawLine(pt_l(pose.elbow2D), pt_l(pose.elbow2D+main_arm/3), gray)
        r.drawLine(pt_l(pose.elbow2D), pt_l(pose.wrist2D), col, self.line_width/2)
        r.drawLine(pt_l(pose.wrist2D), pt_l(pose.effector2D), col, self.line_width)
        r.drawCircle(pt_l(pose.shoulder2D), self.line_width, gray)
        r.drawCircle(pt_l(pose.elbow2D), self.line_width, gray)
        r.drawCircle(pt_l(pose.wrist2D), self.line_width, gray)
        r.drawCircle(pt_l(pose.effector2D), self.line_width, col)

        # Wrist
        wrist_vec = rotate(horizontal, pose.wristYAngle) * 20
        r.drawLine(pt_l(pose.effector2D), pt_l(pose.effector2D + wrist_vec), col)

        # show 2D effector position
        r.drawText(prettyVec(pose.effector2D), col, pt_l(pose.effector2D + [15,0]))

        # Real positions
        # Rotations from vertical
        # left_rot = getArmVerticalOffset(self.l_servo.data['pos'])
        # right_rot = getActuatorVerticalOffset(self.r_servo.data['pos'])
        # real_main = rotate(vertical, left_rot)
        # real_actuator = rotate(vertical, right_rot)
        # fore_angle = self.phys.solve_forearm(left_rot, right_rot)
        # real_fore = rotate(real_main, fore_angle)

        # armPt = self.ik.originpl + real_main*self.ik.len0
        # r.drawLine(
        #     pt_l(self.ik.originpl),
        #     pt_l(armPt),
        #     black
        # )
        # r.drawLine(
        #     pt_l(self.ik.originpl),
        #     pt_l(self.ik.originpl + real_actuator*65),
        #     black
        # )
        # r.drawLine(
        #     pt_l(armPt),
        #     pt_l(armPt + real_fore*self.ik.len1),
        #     black
        # )

        # Text readout
        # text = "Elbow {0:.2f}, {1:.2f}".format(
        #     self.ik.elbow[0],
        #     self.ik.elbow[1]
        # )
        # r.drawText(text, gray, [50, 20])

        # Angles
        vert_angle = degrees(pose.shoulder_angle)
        text = "Main arm {0:.2f} deg".format(vert_angle)
        r.drawText(text, black, [50, 40])

        fore_angle = degrees(pose.elbow_angle)
        text = "Fore arm {0:.2f} deg".format(fore_angle)
        r.drawText(text, black, [50, 60])

class TopView:
    def __init__(self, color=blue, width=6, positioning='normal'):
        self.color = color
        self.line_width = width
        self.positioning = positioning

    def draw(self, pose, r):
        # Z axis from top
        r.drawLine(pt_r([0,0]), pt_r([0,150]), blue)
        r.drawText('z', blue, pt_r([-10,170]))
        # X axis from top
        r.drawLine(pt_r([0,0]), pt_r([150,0]), red)
        r.drawText('x', red, pt_r([150,0]))

        # Base
        r.drawCircle(pt_r([0, 0]), 30, [230, 230, 230])

        offset = 600
        swing = degrees(pose.swing_angle)
        text = "Swing {0:.2f} deg".format(swing)
        r.drawText(text, black, [offset, 20])

        # Show radial dist
        plane_vec = pose.wrist2D - pose.shoulder2D
        radial = plane_vec[0]
        text = "Radial dist {0:.2f}".format(radial)
        r.drawText(text, black, [offset, 40])

        col = self.color
        # if not self.ik.valid:
        #     col = red

        # Top-down view of shoulder, elbow and wrist points
        shoulder = np.array([pose.shoulder[0], pose.shoulder[2]])
        elbow = np.array([pose.elbow[0], pose.elbow[2]])
        wrist = np.array([pose.wrist[0], pose.wrist[2]])
        effector = np.array([pose.effector[0], pose.effector[2]])

        # Wrist is drawn below everything
        r.drawLine(pt_r(wrist), pt_r(effector), col, self.line_width)
        #r.drawCircle(pt_r(wrist), self.line_width/2, gray)
        r.drawCircle(pt_r(effector), self.line_width/2, gray)

        r.drawLine(pt_r(shoulder), pt_r(elbow), col, self.line_width)
        r.drawCircle(pt_r(shoulder), self.line_width, gray)
        r.drawLine(pt_r(elbow), pt_r(wrist), col, self.line_width/2)
        # Elbow joint is above everything
        r.drawCircle(pt_r(elbow), self.line_width, gray)

        # Wrist joint
        wrist_vec = rotate(vertical, pose.swing_angle + pose.wristXAngle) * 20
        r.drawLine(pt_r(effector), pt_r(effector + wrist_vec), col)

        # show top-down effector position
        r.drawText(prettyVec(effector), col, pt_r(effector + [15,0]))

        #r.drawArc(col, pt_r([0,0]), 60, 0, np.pi, 10)
