from util import *

def pt_l(p):
    point = np.array(p).astype(int)
    return (150 + point[0], 300 - point[1])

def pt_r(p):
    point = np.array(p).astype(int)
    return (600 + point[0], 300 - point[1])


def unpt_l(p):
    return np.array([p[0] - 150, 300 - p[1]])

def unpt_r(p):
    return np.array([p[0] - 600, 300 - p[1]])


def getArmVerticalOffset(val):
    return 28.21 + (150 - val)

def getActuatorVerticalOffset(val):
    return val - 150 - 54.78

def getArmFromVerticalOffset(val):
    return 150 + (28.21 - val)

def getActuatorFromVerticalOffset(val):
    return val + 150 + 54.78

class PlaneView:
    def __init__(self, color=blue, width=6):
        self.color = color
        self.line_width = width

    def draw(self, pose, r):
        # Y axis
        r.drawLine(pt_l([0,0]), pt_l([0,150]), green)
        # Z axis from side
        zvec = rotate([0,1], pose.swing_angle)
        r.drawLine(pt_l([0,0]), pt_l([zvec[1]*150,0]), blue)
        # X axis from side
        xvec = rotate([1,0], pose.swing_angle)
        r.drawLine(pt_l([0,0]), pt_l([xvec[1]*150,0]), red)

        # Base
        r.drawRect(pt_l([-30, -25]), [60, 10], gray)

        # Components
        main_arm = pose.elbow2D - pose.shoulder2D
        fore_arm = pose.wrist2D - pose.elbow2D

        col = self.color
        # if not self.ik.valid:
        #     col = red

        # IK Rig
        r.drawLine(pt_l(pose.shoulder2D), pt_l(pose.elbow2D), col, self.line_width)
        r.drawLine(pt_l(pose.elbow2D), pt_l(pose.elbow2D+main_arm/3), gray)
        r.drawLine(pt_l(pose.elbow2D), pt_l(pose.wrist2D), col, self.line_width/2)
        r.drawCircle(pt_l(pose.shoulder2D), self.line_width, gray)
        r.drawCircle(pt_l(pose.elbow2D), self.line_width, gray)
        r.drawCircle(pt_l(pose.wrist2D), self.line_width, col)

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
    def __init__(self, color=blue, width=6):
        self.color = color
        self.line_width = width

    def draw(self, pose, r):
        # Base
        r.drawCircle(pt_r([0, 0]), 30, [230, 230, 230])

        offset = 400
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
        shoulder = [pose.shoulder[0], pose.shoulder[2]]
        elbow = [pose.elbow[0], pose.elbow[2]]
        wrist = [pose.wrist[0], pose.wrist[2]]

        # Wrist is drawn below everything
        r.drawCircle(pt_r(wrist), self.line_width, col)

        r.drawLine(pt_r(shoulder), pt_r(elbow), col, self.line_width)
        r.drawCircle(pt_r(shoulder), self.line_width, gray)
        r.drawLine(pt_r(elbow), pt_r(wrist), col, self.line_width/2)
        # Elbow joint is above everything
        r.drawCircle(pt_r(elbow), self.line_width, gray)
