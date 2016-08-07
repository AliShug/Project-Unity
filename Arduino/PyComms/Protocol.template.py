# Auto-generated file
# Modify via protocol.yaml and/or Protocol.template.py
from __future__ import print_function

import struct
import time

waitTime = 0.01

class TimeoutException(Exception):
    pass

def waitFor(serial, num_bytes, timeout=0.1):
    start = time.time()

    # Busy wait until required bytes arrive or we timeout
    while time.time() < start + timeout:
        if serial.in_waiting >= num_bytes:
            return

    raise TimeoutException('Timeout')

def tryRead(serial, num_bytes, timeout=0.05):
    start = time.time()
    read_bytes = 0

    # Busy wait until required bytes arrive or we timeout
    while time.time() < start + timeout:
        if serial.in_waiting >= num_bytes:
            return serial.read(num_bytes)

    raise TimeoutException('Timeout')


class CapacitiveSensor:
    def __init__(self, serial):
        self.serial = serial

    def read(self, count):
        command = 'c{0}'.format(chr(count))
        l = struct.pack('b', len(command))
        self.serial.write(l+command)
        waitFor(self.serial, count*4)

        values = [0]*count
        for i in xrange(count):
            values[i] = struct.unpack('i', self.serial.read(4))[0]
        return values

class Servo:
    def __init__(self, serial, protocol_ver, id):
        self.protocol = protocol_ver
        self.id = id
        self.serial = serial
        self.data = {'pos': 150}

    # Templated commands
{% for c in commands if c.can_set %}
    def set{{c.name}}(self, val):
        command = 's{{"\\x{0:02X}".format(ord(c.short))}}{pver}{packedid}{arg}'.format(
            pver = self.protocol,
            packedid = struct.pack('B', self.id),
            {% if c.type == 'int' %}
            arg = struct.pack('i', val)
            {% else %}
            arg = struct.pack('f', val)
            {% endif %}
        )
        l = struct.pack('b', len(command))
        self.serial.write(l+command)
        waitFor(self.serial, 2)

        # Response
        res = 'ERROR: Nothing received'
        while self.serial.in_waiting > 0:
            res = self.serial.readline()
        if res.startswith('ERROR'):
            print (res)
            return False
        return True
{% endfor %}
{% for c in commands if c.can_get %}
    def get{{c.name}}(self):
        command = 'g{{"\\x{0:02X}".format(ord(c.short))}}{pver}{packedid}'.format(
            pver = self.protocol,
            packedid = struct.pack('B', self.id)
        )
        l = struct.pack('b', len(command))
        self.serial.write(l+command)
        waitFor(self.serial, 5)

        # Retreive response
        try:
            arg = tryRead(self.serial, 1)
            if arg != 'k':
                print ('ERR: ',arg+self.serial.readline())
                return None
            arg = tryRead(self.serial, 4)
            {% if c.type == 'int' %}
            val = struct.unpack('i', arg)[0]
            {% else %}
            val = struct.unpack('f', arg)[0]
            {% endif %}
            return val
        except Exception as e:
            print ('ERR: Bad receive', e)
            return None
{% endfor %}
#def getServos():
