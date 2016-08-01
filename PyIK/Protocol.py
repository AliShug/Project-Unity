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
    def setGoalPosition(self, val):
        command = 'sP{pver}{packedid}{arg}'.format(
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
        command = 'sS{pver}{packedid}{arg}'.format(
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
    def setLED(self, val):
        command = 'se{pver}{packedid}{arg}'.format(
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
        command = 'sq{pver}{packedid}{arg}'.format(
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
        command = 's+{pver}{packedid}{arg}'.format(
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
        command = 's_{pver}{packedid}{arg}'.format(
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
        command = 's-{pver}{packedid}{arg}'.format(
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
        command = 's={pver}{packedid}{arg}'.format(
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
    def getVoltage(self):
        command = 'gv{pver}{packedid}'.format(
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
        command = 'gp{pver}{packedid}'.format(
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
        command = 'gl{pver}{packedid}'.format(
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
        command = 'gt{pver}{packedid}'.format(
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
        command = 'gs{pver}{packedid}'.format(
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
        command = 'gP{pver}{packedid}'.format(
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
        command = 'gS{pver}{packedid}'.format(
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
    def getLED(self):
        command = 'ge{pver}{packedid}'.format(
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
        command = 'gq{pver}{packedid}'.format(
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
        command = 'g+{pver}{packedid}'.format(
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
        command = 'g_{pver}{packedid}'.format(
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
        command = 'g-{pver}{packedid}'.format(
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
        command = 'g={pver}{packedid}'.format(
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