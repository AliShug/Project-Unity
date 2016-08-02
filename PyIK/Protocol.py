# Auto-generated file
# Modify via protocol.yaml and/or Protocol.template.py
from __future__ import print_function

import struct
import time

waitTime = 0.01


class Servo:
    def __init__(self, serial, protocol_ver, id):
        self.protocol = protocol_ver
        self.id = id
        self.serial = serial
        self.data = {'pos': 150}

    def waitFor(self, num_bytes, timeout=0.1):
        start = time.time()

        # Busy wait until required bytes arrive or we timeout
        while time.time() < start + timeout:
            if self.serial.in_waiting >= num_bytes:
                return

        raise Exception('Timeout')

    def tryRead(self, num_bytes, timeout=0.05):
        start = time.time()
        read_bytes = 0

        # Busy wait until required bytes arrive or we timeout
        while time.time() < start + timeout:
            if self.serial.in_waiting >= num_bytes:
                return self.serial.read(num_bytes)

        raise Exception('Timeout')

    # Templated commands
    def setID(self, val):
        command = 's\x02{pver}{packedid}{arg}'.format(
            pver = self.protocol,
            packedid = struct.pack('B', self.id),
            arg = struct.pack('i', val)
        )
        l = struct.pack('b', len(command))
        self.serial.write(l+command)
        self.waitFor(2)

        # Response
        res = 'ERROR: Nothing received'
        while self.serial.in_waiting > 0:
            res = self.serial.readline()
        if res.startswith('ERROR'):
            print (res)
            return False
        return True
    def setReturnDelay(self, val):
        command = 's\x03{pver}{packedid}{arg}'.format(
            pver = self.protocol,
            packedid = struct.pack('B', self.id),
            arg = struct.pack('i', val)
        )
        l = struct.pack('b', len(command))
        self.serial.write(l+command)
        self.waitFor(2)

        # Response
        res = 'ERROR: Nothing received'
        while self.serial.in_waiting > 0:
            res = self.serial.readline()
        if res.startswith('ERROR'):
            print (res)
            return False
        return True
    def setMaxTorque(self, val):
        command = 's\x04{pver}{packedid}{arg}'.format(
            pver = self.protocol,
            packedid = struct.pack('B', self.id),
            arg = struct.pack('f', val)
        )
        l = struct.pack('b', len(command))
        self.serial.write(l+command)
        self.waitFor(2)

        # Response
        res = 'ERROR: Nothing received'
        while self.serial.in_waiting > 0:
            res = self.serial.readline()
        if res.startswith('ERROR'):
            print (res)
            return False
        return True
    def setTorqueEnable(self, val):
        command = 's\x05{pver}{packedid}{arg}'.format(
            pver = self.protocol,
            packedid = struct.pack('B', self.id),
            arg = struct.pack('i', val)
        )
        l = struct.pack('b', len(command))
        self.serial.write(l+command)
        self.waitFor(2)

        # Response
        res = 'ERROR: Nothing received'
        while self.serial.in_waiting > 0:
            res = self.serial.readline()
        if res.startswith('ERROR'):
            print (res)
            return False
        return True
    def setLED(self, val):
        command = 's\x06{pver}{packedid}{arg}'.format(
            pver = self.protocol,
            packedid = struct.pack('B', self.id),
            arg = struct.pack('i', val)
        )
        l = struct.pack('b', len(command))
        self.serial.write(l+command)
        self.waitFor(2)

        # Response
        res = 'ERROR: Nothing received'
        while self.serial.in_waiting > 0:
            res = self.serial.readline()
        if res.startswith('ERROR'):
            print (res)
            return False
        return True
    def setGoalPosition(self, val):
        command = 's\x0C{pver}{packedid}{arg}'.format(
            pver = self.protocol,
            packedid = struct.pack('B', self.id),
            arg = struct.pack('f', val)
        )
        l = struct.pack('b', len(command))
        self.serial.write(l+command)
        self.waitFor(2)

        # Response
        res = 'ERROR: Nothing received'
        while self.serial.in_waiting > 0:
            res = self.serial.readline()
        if res.startswith('ERROR'):
            print (res)
            return False
        return True
    def setGoalSpeed(self, val):
        command = 's\x0D{pver}{packedid}{arg}'.format(
            pver = self.protocol,
            packedid = struct.pack('B', self.id),
            arg = struct.pack('f', val)
        )
        l = struct.pack('b', len(command))
        self.serial.write(l+command)
        self.waitFor(2)

        # Response
        res = 'ERROR: Nothing received'
        while self.serial.in_waiting > 0:
            res = self.serial.readline()
        if res.startswith('ERROR'):
            print (res)
            return False
        return True
    def setTorqueLimit(self, val):
        command = 's\x0E{pver}{packedid}{arg}'.format(
            pver = self.protocol,
            packedid = struct.pack('B', self.id),
            arg = struct.pack('f', val)
        )
        l = struct.pack('b', len(command))
        self.serial.write(l+command)
        self.waitFor(2)

        # Response
        res = 'ERROR: Nothing received'
        while self.serial.in_waiting > 0:
            res = self.serial.readline()
        if res.startswith('ERROR'):
            print (res)
            return False
        return True
    def setCWMargin(self, val):
        command = 's\x0F{pver}{packedid}{arg}'.format(
            pver = self.protocol,
            packedid = struct.pack('B', self.id),
            arg = struct.pack('i', val)
        )
        l = struct.pack('b', len(command))
        self.serial.write(l+command)
        self.waitFor(2)

        # Response
        res = 'ERROR: Nothing received'
        while self.serial.in_waiting > 0:
            res = self.serial.readline()
        if res.startswith('ERROR'):
            print (res)
            return False
        return True
    def setCCWMargin(self, val):
        command = 's\x10{pver}{packedid}{arg}'.format(
            pver = self.protocol,
            packedid = struct.pack('B', self.id),
            arg = struct.pack('i', val)
        )
        l = struct.pack('b', len(command))
        self.serial.write(l+command)
        self.waitFor(2)

        # Response
        res = 'ERROR: Nothing received'
        while self.serial.in_waiting > 0:
            res = self.serial.readline()
        if res.startswith('ERROR'):
            print (res)
            return False
        return True
    def setCWSlope(self, val):
        command = 's\x11{pver}{packedid}{arg}'.format(
            pver = self.protocol,
            packedid = struct.pack('B', self.id),
            arg = struct.pack('i', val)
        )
        l = struct.pack('b', len(command))
        self.serial.write(l+command)
        self.waitFor(2)

        # Response
        res = 'ERROR: Nothing received'
        while self.serial.in_waiting > 0:
            res = self.serial.readline()
        if res.startswith('ERROR'):
            print (res)
            return False
        return True
    def setCCWSlope(self, val):
        command = 's\x12{pver}{packedid}{arg}'.format(
            pver = self.protocol,
            packedid = struct.pack('B', self.id),
            arg = struct.pack('i', val)
        )
        l = struct.pack('b', len(command))
        self.serial.write(l+command)
        self.waitFor(2)

        # Response
        res = 'ERROR: Nothing received'
        while self.serial.in_waiting > 0:
            res = self.serial.readline()
        if res.startswith('ERROR'):
            print (res)
            return False
        return True
    def getModelNumber(self):
        command = 'g\x00{pver}{packedid}'.format(
            pver = self.protocol,
            packedid = struct.pack('B', self.id)
        )
        l = struct.pack('b', len(command))
        self.serial.write(l+command)
        self.waitFor(5)

        # Retreive response
        try:
            arg = self.tryRead(1)
            if arg != 'k':
                print ('ERR: ',arg+self.serial.readline())
                return None
            arg = self.tryRead(4)
            val = struct.unpack('i', arg)[0]
            return val
        except Exception as e:
            print ('ERR: Bad receive', e)
            return None
    def getFirmwareVersion(self):
        command = 'g\x01{pver}{packedid}'.format(
            pver = self.protocol,
            packedid = struct.pack('B', self.id)
        )
        l = struct.pack('b', len(command))
        self.serial.write(l+command)
        self.waitFor(5)

        # Retreive response
        try:
            arg = self.tryRead(1)
            if arg != 'k':
                print ('ERR: ',arg+self.serial.readline())
                return None
            arg = self.tryRead(4)
            val = struct.unpack('i', arg)[0]
            return val
        except Exception as e:
            print ('ERR: Bad receive', e)
            return None
    def getID(self):
        command = 'g\x02{pver}{packedid}'.format(
            pver = self.protocol,
            packedid = struct.pack('B', self.id)
        )
        l = struct.pack('b', len(command))
        self.serial.write(l+command)
        self.waitFor(5)

        # Retreive response
        try:
            arg = self.tryRead(1)
            if arg != 'k':
                print ('ERR: ',arg+self.serial.readline())
                return None
            arg = self.tryRead(4)
            val = struct.unpack('i', arg)[0]
            return val
        except Exception as e:
            print ('ERR: Bad receive', e)
            return None
    def getReturnDelay(self):
        command = 'g\x03{pver}{packedid}'.format(
            pver = self.protocol,
            packedid = struct.pack('B', self.id)
        )
        l = struct.pack('b', len(command))
        self.serial.write(l+command)
        self.waitFor(5)

        # Retreive response
        try:
            arg = self.tryRead(1)
            if arg != 'k':
                print ('ERR: ',arg+self.serial.readline())
                return None
            arg = self.tryRead(4)
            val = struct.unpack('i', arg)[0]
            return val
        except Exception as e:
            print ('ERR: Bad receive', e)
            return None
    def getMaxTorque(self):
        command = 'g\x04{pver}{packedid}'.format(
            pver = self.protocol,
            packedid = struct.pack('B', self.id)
        )
        l = struct.pack('b', len(command))
        self.serial.write(l+command)
        self.waitFor(5)

        # Retreive response
        try:
            arg = self.tryRead(1)
            if arg != 'k':
                print ('ERR: ',arg+self.serial.readline())
                return None
            arg = self.tryRead(4)
            val = struct.unpack('f', arg)[0]
            return val
        except Exception as e:
            print ('ERR: Bad receive', e)
            return None
    def getTorqueEnable(self):
        command = 'g\x05{pver}{packedid}'.format(
            pver = self.protocol,
            packedid = struct.pack('B', self.id)
        )
        l = struct.pack('b', len(command))
        self.serial.write(l+command)
        self.waitFor(5)

        # Retreive response
        try:
            arg = self.tryRead(1)
            if arg != 'k':
                print ('ERR: ',arg+self.serial.readline())
                return None
            arg = self.tryRead(4)
            val = struct.unpack('i', arg)[0]
            return val
        except Exception as e:
            print ('ERR: Bad receive', e)
            return None
    def getLED(self):
        command = 'g\x06{pver}{packedid}'.format(
            pver = self.protocol,
            packedid = struct.pack('B', self.id)
        )
        l = struct.pack('b', len(command))
        self.serial.write(l+command)
        self.waitFor(5)

        # Retreive response
        try:
            arg = self.tryRead(1)
            if arg != 'k':
                print ('ERR: ',arg+self.serial.readline())
                return None
            arg = self.tryRead(4)
            val = struct.unpack('i', arg)[0]
            return val
        except Exception as e:
            print ('ERR: Bad receive', e)
            return None
    def getVoltage(self):
        command = 'g\x07{pver}{packedid}'.format(
            pver = self.protocol,
            packedid = struct.pack('B', self.id)
        )
        l = struct.pack('b', len(command))
        self.serial.write(l+command)
        self.waitFor(5)

        # Retreive response
        try:
            arg = self.tryRead(1)
            if arg != 'k':
                print ('ERR: ',arg+self.serial.readline())
                return None
            arg = self.tryRead(4)
            val = struct.unpack('f', arg)[0]
            return val
        except Exception as e:
            print ('ERR: Bad receive', e)
            return None
    def getPosition(self):
        command = 'g\x08{pver}{packedid}'.format(
            pver = self.protocol,
            packedid = struct.pack('B', self.id)
        )
        l = struct.pack('b', len(command))
        self.serial.write(l+command)
        self.waitFor(5)

        # Retreive response
        try:
            arg = self.tryRead(1)
            if arg != 'k':
                print ('ERR: ',arg+self.serial.readline())
                return None
            arg = self.tryRead(4)
            val = struct.unpack('f', arg)[0]
            return val
        except Exception as e:
            print ('ERR: Bad receive', e)
            return None
    def getLoad(self):
        command = 'g\x09{pver}{packedid}'.format(
            pver = self.protocol,
            packedid = struct.pack('B', self.id)
        )
        l = struct.pack('b', len(command))
        self.serial.write(l+command)
        self.waitFor(5)

        # Retreive response
        try:
            arg = self.tryRead(1)
            if arg != 'k':
                print ('ERR: ',arg+self.serial.readline())
                return None
            arg = self.tryRead(4)
            val = struct.unpack('f', arg)[0]
            return val
        except Exception as e:
            print ('ERR: Bad receive', e)
            return None
    def getTemperature(self):
        command = 'g\x0A{pver}{packedid}'.format(
            pver = self.protocol,
            packedid = struct.pack('B', self.id)
        )
        l = struct.pack('b', len(command))
        self.serial.write(l+command)
        self.waitFor(5)

        # Retreive response
        try:
            arg = self.tryRead(1)
            if arg != 'k':
                print ('ERR: ',arg+self.serial.readline())
                return None
            arg = self.tryRead(4)
            val = struct.unpack('i', arg)[0]
            return val
        except Exception as e:
            print ('ERR: Bad receive', e)
            return None
    def getSpeed(self):
        command = 'g\x0B{pver}{packedid}'.format(
            pver = self.protocol,
            packedid = struct.pack('B', self.id)
        )
        l = struct.pack('b', len(command))
        self.serial.write(l+command)
        self.waitFor(5)

        # Retreive response
        try:
            arg = self.tryRead(1)
            if arg != 'k':
                print ('ERR: ',arg+self.serial.readline())
                return None
            arg = self.tryRead(4)
            val = struct.unpack('f', arg)[0]
            return val
        except Exception as e:
            print ('ERR: Bad receive', e)
            return None
    def getGoalPosition(self):
        command = 'g\x0C{pver}{packedid}'.format(
            pver = self.protocol,
            packedid = struct.pack('B', self.id)
        )
        l = struct.pack('b', len(command))
        self.serial.write(l+command)
        self.waitFor(5)

        # Retreive response
        try:
            arg = self.tryRead(1)
            if arg != 'k':
                print ('ERR: ',arg+self.serial.readline())
                return None
            arg = self.tryRead(4)
            val = struct.unpack('f', arg)[0]
            return val
        except Exception as e:
            print ('ERR: Bad receive', e)
            return None
    def getGoalSpeed(self):
        command = 'g\x0D{pver}{packedid}'.format(
            pver = self.protocol,
            packedid = struct.pack('B', self.id)
        )
        l = struct.pack('b', len(command))
        self.serial.write(l+command)
        self.waitFor(5)

        # Retreive response
        try:
            arg = self.tryRead(1)
            if arg != 'k':
                print ('ERR: ',arg+self.serial.readline())
                return None
            arg = self.tryRead(4)
            val = struct.unpack('f', arg)[0]
            return val
        except Exception as e:
            print ('ERR: Bad receive', e)
            return None
    def getTorqueLimit(self):
        command = 'g\x0E{pver}{packedid}'.format(
            pver = self.protocol,
            packedid = struct.pack('B', self.id)
        )
        l = struct.pack('b', len(command))
        self.serial.write(l+command)
        self.waitFor(5)

        # Retreive response
        try:
            arg = self.tryRead(1)
            if arg != 'k':
                print ('ERR: ',arg+self.serial.readline())
                return None
            arg = self.tryRead(4)
            val = struct.unpack('f', arg)[0]
            return val
        except Exception as e:
            print ('ERR: Bad receive', e)
            return None
    def getCWMargin(self):
        command = 'g\x0F{pver}{packedid}'.format(
            pver = self.protocol,
            packedid = struct.pack('B', self.id)
        )
        l = struct.pack('b', len(command))
        self.serial.write(l+command)
        self.waitFor(5)

        # Retreive response
        try:
            arg = self.tryRead(1)
            if arg != 'k':
                print ('ERR: ',arg+self.serial.readline())
                return None
            arg = self.tryRead(4)
            val = struct.unpack('i', arg)[0]
            return val
        except Exception as e:
            print ('ERR: Bad receive', e)
            return None
    def getCCWMargin(self):
        command = 'g\x10{pver}{packedid}'.format(
            pver = self.protocol,
            packedid = struct.pack('B', self.id)
        )
        l = struct.pack('b', len(command))
        self.serial.write(l+command)
        self.waitFor(5)

        # Retreive response
        try:
            arg = self.tryRead(1)
            if arg != 'k':
                print ('ERR: ',arg+self.serial.readline())
                return None
            arg = self.tryRead(4)
            val = struct.unpack('i', arg)[0]
            return val
        except Exception as e:
            print ('ERR: Bad receive', e)
            return None
    def getCWSlope(self):
        command = 'g\x11{pver}{packedid}'.format(
            pver = self.protocol,
            packedid = struct.pack('B', self.id)
        )
        l = struct.pack('b', len(command))
        self.serial.write(l+command)
        self.waitFor(5)

        # Retreive response
        try:
            arg = self.tryRead(1)
            if arg != 'k':
                print ('ERR: ',arg+self.serial.readline())
                return None
            arg = self.tryRead(4)
            val = struct.unpack('i', arg)[0]
            return val
        except Exception as e:
            print ('ERR: Bad receive', e)
            return None
    def getCCWSlope(self):
        command = 'g\x12{pver}{packedid}'.format(
            pver = self.protocol,
            packedid = struct.pack('B', self.id)
        )
        l = struct.pack('b', len(command))
        self.serial.write(l+command)
        self.waitFor(5)

        # Retreive response
        try:
            arg = self.tryRead(1)
            if arg != 'k':
                print ('ERR: ',arg+self.serial.readline())
                return None
            arg = self.tryRead(4)
            val = struct.unpack('i', arg)[0]
            return val
        except Exception as e:
            print ('ERR: Bad receive', e)
            return None
#def getServos():