//
// Auto-generated from Protocol.template.cpp
// Modify the config/template then run protocolgen.py
//

#include "Protocol.h"

Protocol::Protocol(DX1Motor *x1s, int nx1, DX2Motor *x2s, int nx2)
:   _capSense(30,31),
    _x1s(x1s), _x2s(x2s),
    _nx1(nx1), _nx2(nx2) {}

bool Protocol::waitTimeout(Stream &s, long time) {
    unsigned long start = millis();

    do {
        if (s.peek() >= 0) {
            return true;
        }
    } while (millis() - start < time);

    return false;
}




// Utility functions
void dx1FinaliseSet(Stream &s, int err) {
    if (err != DX1MOTOR_ERR_OK) {
        s.print("ERROR ");
        s.println(err);
    }
    else {
        s.println("k");
    }
}

void dx2FinaliseSet(Stream &s, int err) {
    if (err != DX2MOTOR_ERR_OK) {
        s.print("ERROR ");
        s.println(err);
    }
    else {
        s.println("k");
    }
}

void dx1FinaliseGet(Stream &s, int err, const void* res) {
    if (err != DX1MOTOR_ERR_OK) {
        s.print("ERROR ");
        s.println(err);
    }
    else {
        s.print('k');
        char *ptr = (char*)res;
        s.write(ptr, 4);
    }
}

void dx2FinaliseGet(Stream &s, int err, const void* res) {
    if (err != DX2MOTOR_ERR_OK) {
        s.print("ERROR ");
        s.println(err);
    }
    else {
        s.print('k');
        char *ptr = (char*)res;
        s.write(ptr, 4);
    }
}


// DX1 command processing
void Protocol::dispatchV1Command(Stream &s, int mode, char command, char id) {
    int err;

    if (mode == MODE_SET) {
        switch (command) {
            case '\x02': {
                long val = _argi;
                err = _x1s[id].setID(val);
                dx1FinaliseSet(s, err);
            } break;
            case '\x03': {
                long val = _argi;
                err = _x1s[id].setReturnDelay(val);
                dx1FinaliseSet(s, err);
            } break;
            case '\x04': {
                float val = _argf;
                err = _x1s[id].setMaxTorque(val);
                dx1FinaliseSet(s, err);
            } break;
            case '\x05': {
                long val = _argi;
                err = _x1s[id].setTorqueEnable(val);
                dx1FinaliseSet(s, err);
            } break;
            case '\x06': {
                long val = _argi;
                err = _x1s[id].setLED(val);
                dx1FinaliseSet(s, err);
            } break;
            case '\x0C': {
                float val = _argf;
                err = _x1s[id].setGoalPosition(val);
                dx1FinaliseSet(s, err);
            } break;
            case '\x0D': {
                float val = _argf;
                err = _x1s[id].setGoalSpeed(val);
                dx1FinaliseSet(s, err);
            } break;
            case '\x0E': {
                float val = _argf;
                err = _x1s[id].setTorqueLimit(val);
                dx1FinaliseSet(s, err);
            } break;
            case '\x0F': {
                long val = _argi;
                err = _x1s[id].setCWMargin(val);
                dx1FinaliseSet(s, err);
            } break;
            case '\x10': {
                long val = _argi;
                err = _x1s[id].setCCWMargin(val);
                dx1FinaliseSet(s, err);
            } break;
            case '\x11': {
                long val = _argi;
                err = _x1s[id].setCWSlope(val);
                dx1FinaliseSet(s, err);
            } break;
            case '\x12': {
                long val = _argi;
                err = _x1s[id].setCCWSlope(val);
                dx1FinaliseSet(s, err);
            } break;
            default:
                s.println("ERROR: Bad command");
            return;
        }
        //s.flush();
    }
    else if (mode == MODE_GET) {
        switch (command) {
            case '\x00': {
                long res;
                res = _x1s[id].getModelNumber(err);
                dx1FinaliseGet(s, err, &res);
            } break;
            case '\x01': {
                long res;
                res = _x1s[id].getFirmwareVersion(err);
                dx1FinaliseGet(s, err, &res);
            } break;
            case '\x02': {
                long res;
                res = _x1s[id].getID(err);
                dx1FinaliseGet(s, err, &res);
            } break;
            case '\x03': {
                long res;
                res = _x1s[id].getReturnDelay(err);
                dx1FinaliseGet(s, err, &res);
            } break;
            case '\x04': {
                float res;
                res = _x1s[id].getMaxTorque(err);
                dx1FinaliseGet(s, err, &res);
            } break;
            case '\x05': {
                long res;
                res = _x1s[id].getTorqueEnable(err);
                dx1FinaliseGet(s, err, &res);
            } break;
            case '\x06': {
                long res;
                res = _x1s[id].getLED(err);
                dx1FinaliseGet(s, err, &res);
            } break;
            case '\x07': {
                float res;
                res = _x1s[id].getVoltage(err);
                dx1FinaliseGet(s, err, &res);
            } break;
            case '\x08': {
                float res;
                res = _x1s[id].getPosition(err);
                dx1FinaliseGet(s, err, &res);
            } break;
            case '\x09': {
                float res;
                res = _x1s[id].getLoad(err);
                dx1FinaliseGet(s, err, &res);
            } break;
            case '\x0A': {
                long res;
                res = _x1s[id].getTemperature(err);
                dx1FinaliseGet(s, err, &res);
            } break;
            case '\x0B': {
                float res;
                res = _x1s[id].getSpeed(err);
                dx1FinaliseGet(s, err, &res);
            } break;
            case '\x0C': {
                float res;
                res = _x1s[id].getGoalPosition(err);
                dx1FinaliseGet(s, err, &res);
            } break;
            case '\x0D': {
                float res;
                res = _x1s[id].getGoalSpeed(err);
                dx1FinaliseGet(s, err, &res);
            } break;
            case '\x0E': {
                float res;
                res = _x1s[id].getTorqueLimit(err);
                dx1FinaliseGet(s, err, &res);
            } break;
            case '\x0F': {
                long res;
                res = _x1s[id].getCWMargin(err);
                dx1FinaliseGet(s, err, &res);
            } break;
            case '\x10': {
                long res;
                res = _x1s[id].getCCWMargin(err);
                dx1FinaliseGet(s, err, &res);
            } break;
            case '\x11': {
                long res;
                res = _x1s[id].getCWSlope(err);
                dx1FinaliseGet(s, err, &res);
            } break;
            case '\x12': {
                long res;
                res = _x1s[id].getCCWSlope(err);
                dx1FinaliseGet(s, err, &res);
            } break;
            default:
                s.println("ERROR: Bad command");
            return;
        }
        //s.flush();
    }
}


// DX2 command processing
void Protocol::dispatchV2Command(Stream &s, int mode, char command, char id) {
    int err;

    if (mode == MODE_SET) {
        switch (command) {
            case '\x02': {
                long val = _argi;
                err = _x2s[id].setID(val);
                dx2FinaliseSet(s, err);
            } break;
            case '\x03': {
                long val = _argi;
                err = _x2s[id].setReturnDelay(val);
                dx2FinaliseSet(s, err);
            } break;
            case '\x04': {
                float val = _argf;
                err = _x2s[id].setMaxTorque(val);
                dx2FinaliseSet(s, err);
            } break;
            case '\x05': {
                long val = _argi;
                err = _x2s[id].setTorqueEnable(val);
                dx2FinaliseSet(s, err);
            } break;
            case '\x06': {
                long val = _argi;
                err = _x2s[id].setLED(val);
                dx2FinaliseSet(s, err);
            } break;
            case '\x0C': {
                float val = _argf;
                err = _x2s[id].setGoalPosition(val);
                dx2FinaliseSet(s, err);
            } break;
            case '\x0D': {
                float val = _argf;
                err = _x2s[id].setGoalSpeed(val);
                dx2FinaliseSet(s, err);
            } break;
            case '\x0E': {
                float val = _argf;
                err = _x2s[id].setTorqueLimit(val);
                dx2FinaliseSet(s, err);
            } break;
            default:
                s.println("ERROR: Bad command");
            return;
        }
        s.flush();
    }
    else if (mode == MODE_GET) {
        switch (command) {
            case '\x00': {
                long res;
                res = _x2s[id].getModelNumber(err);
                dx2FinaliseGet(s, err, &res);
            } break;
            case '\x01': {
                long res;
                res = _x2s[id].getFirmwareVersion(err);
                dx2FinaliseGet(s, err, &res);
            } break;
            case '\x02': {
                long res;
                res = _x2s[id].getID(err);
                dx2FinaliseGet(s, err, &res);
            } break;
            case '\x03': {
                long res;
                res = _x2s[id].getReturnDelay(err);
                dx2FinaliseGet(s, err, &res);
            } break;
            case '\x04': {
                float res;
                res = _x2s[id].getMaxTorque(err);
                dx2FinaliseGet(s, err, &res);
            } break;
            case '\x05': {
                long res;
                res = _x2s[id].getTorqueEnable(err);
                dx2FinaliseGet(s, err, &res);
            } break;
            case '\x06': {
                long res;
                res = _x2s[id].getLED(err);
                dx2FinaliseGet(s, err, &res);
            } break;
            case '\x07': {
                float res;
                res = _x2s[id].getVoltage(err);
                dx2FinaliseGet(s, err, &res);
            } break;
            case '\x08': {
                float res;
                res = _x2s[id].getPosition(err);
                dx2FinaliseGet(s, err, &res);
            } break;
            case '\x09': {
                float res;
                res = _x2s[id].getLoad(err);
                dx2FinaliseGet(s, err, &res);
            } break;
            case '\x0A': {
                long res;
                res = _x2s[id].getTemperature(err);
                dx2FinaliseGet(s, err, &res);
            } break;
            case '\x0B': {
                float res;
                res = _x2s[id].getSpeed(err);
                dx2FinaliseGet(s, err, &res);
            } break;
            case '\x0C': {
                float res;
                res = _x2s[id].getGoalPosition(err);
                dx2FinaliseGet(s, err, &res);
            } break;
            case '\x0D': {
                float res;
                res = _x2s[id].getGoalSpeed(err);
                dx2FinaliseGet(s, err, &res);
            } break;
            case '\x0E': {
                float res;
                res = _x2s[id].getTorqueLimit(err);
                dx2FinaliseGet(s, err, &res);
            } break;
            default:
                s.println("ERROR: Bad command");
            return;
        }
        s.flush();
    }
}


void Protocol::handleIncoming(Stream &s) {
    if (s.peek() < 0) return;

    // Get command string
    char buffer[64];
    int commandLen = s.read();
    for (int i = 0; i < commandLen; i++) {
        while (!s.available()); // busy wait for each byte
        buffer[i] = s.read();
    }

    // Ignore noise
    if (commandLen < 2) {
        return;
    }

    // First character indicates get/set mode (or special commands)
    char read = buffer[0];
    int mode;
    switch (read) {
    case 'g':
        mode = MODE_GET;
        break;
    case 's':
        mode = MODE_SET;
        break;
    case 'l':
        // List servos!
        s.print("X1 n=");
        s.println(_nx1);
        for (int i = 0; i < _nx1; i++) {
            int err, id;
            id = _x1s[i].getID(err);
            if (err != DX1MOTOR_ERR_OK) {
                s.println("ERROR: No response");
            }
            else {
                s.println(i);
            }
        }
        s.print("X2 n=");
        s.println(_nx2);
        for (int i = 0; i < _nx2; i++) {
            int err, id;
            id = _x2s[i].getID(err);
            if (err != DX2MOTOR_ERR_OK) {
                s.println("ERROR: No response");
            }
            else {
                s.println(i);
            }
        }
        return;
    case 'c':
        // Capacitive sensing
        for (int i = 0; i < buffer[1]; i++) {
            long sensor = _capSense.capacitiveSensor(20);
            s.write((char*)&sensor, sizeof(sensor));
        }
        return;
    default:
        s.print("ERROR: Mode error ");
        s.println(read);
        return;
    }

    // Second character indicates command
    char command_byte = buffer[1];

    // Servo protocol
    char dx_ver = buffer[2];
    // and ID
    char targ_id = buffer[3];

    if (mode == MODE_SET) {
        if (commandLen < 8) {
            s.println("ERROR: Arguments required");
            return;
        }

        // Copy in argument(s)
        memcpy(&_argi, buffer + 4, 4);
    }

    if (dx_ver == '1') {
        dispatchV1Command(s, mode, command_byte, targ_id);
    }
    else if (dx_ver == '2') {
        dispatchV2Command(s, mode, command_byte, targ_id);
    }
}