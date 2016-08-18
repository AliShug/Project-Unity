import numpy as np

class ArmVolume:
    def __init__(self, arm, res=50, offset=90, backSize=500):
        self.offset = offset
        self.res = res
        self.samples = np.ndarray(shape=(res,res), dtype=bool)
        self.size = (430-offset, backSize)

        y_step = self.size[1]/float(res)
        x_step = self.size[0]/float(res)
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

                self.samples[x,y] = test

    def getRadii(self, height):
        y_step = self.size[1] / float(self.res)
        x_step = self.size[0] / float(self.res)
        y_ind = np.clip(int(height/y_step + self.res/2), 0, self.res-1)

        min = 0
        max = None
        for x in xrange(self.res):
            if max is None:
                if not self.samples[x, y_ind]:
                    min = self.offset + x*x_step
            if self.samples[x, y_ind]:
                max = self.offset + x*x_step

        return min,max
