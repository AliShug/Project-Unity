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
{% for c in commands if c.can_set %}
    def set{{c.name}}(self, val):
        command = 's{{c.short}}{pver}{packedid}{arg}'.format(
            pver = self.protocol,
            packedid = struct.pack('B', self.id),
            {% if c.type == 'int' %}
            arg = struct.pack('i', val)
            {% else %}
            arg = struct.pack('f', val)
            {% endif %}
        )
        self.serial.write(command)
        self.waitFor(2)

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
        command = 'g{{c.short}}{pver}{packedid}'.format(
            pver = self.protocol,
            packedid = struct.pack('B', self.id)
        )
        self.serial.write(command)
        self.waitFor(5)

        # Retreive response
        try:
            arg = self.tryRead(1)
            if arg != 'k':
                print ('ERR: ',arg+self.serial.readline())
                return None
            arg = self.tryRead(4)
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
