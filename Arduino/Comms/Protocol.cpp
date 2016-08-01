//
// Auto-generated from Protocol.template.cpp
// Modify the config/template then run protocolgen.py
//

#include "Protocol.h"

Protocol::Protocol(DX1Motor *x1s, int nx1, DX2Motor *x2s, int nx2) {
    _x1s = x1s;
    _x2s = x2s;
    _nx1 = nx1;
    _nx2 = nx2;
}

bool Protocol::waitTimeout(Stream &s, long time) {
    unsigned long start = millis();

    do {
        if (s.peek() >= 0) {
            return true;
        }
    } while (millis() - start < time);

    return false;
}


void Protocol::dispatchV1Command(Stream &s, int mode, char command, char id) {
    int err;

    if (mode == MODE_SET) {
        switch (command) {
            case 'P': {
                float val = _argf;
                err = _x1s[id].setGoalPosition(val);
                if (err != DX1MOTOR_ERR_OK) {
                    s.print("ERROR ");
                    s.println(err);
                }
                else {
                    s.println("k");
                }
            } break;
            case 'S': {
                float val = _argf;
                err = _x1s[id].setGoalSpeed(val);
                if (err != DX1MOTOR_ERR_OK) {
                    s.print("ERROR ");
                    s.println(err);
                }
                else {
                    s.println("k");
                }
            } break;
            case 'e': {
                long val = _argi;
                err = _x1s[id].setLED(val);
                if (err != DX1MOTOR_ERR_OK) {
                    s.print("ERROR ");
                    s.println(err);
                }
                else {
                    s.println("k");
                }
            } break;
            case 'q': {
                float val = _argf;
                err = _x1s[id].setMaxTorque(val);
                if (err != DX1MOTOR_ERR_OK) {
                    s.print("ERROR ");
                    s.println(err);
                }
                else {
                    s.println("k");
                }
            } break;
            case '+': {
                long val = _argi;
                err = _x1s[id].setCWMargin(val);
                if (err != DX1MOTOR_ERR_OK) {
                    s.print("ERROR ");
                    s.println(err);
                }
                else {
                    s.println("k");
                }
            } break;
            case '_': {
                long val = _argi;
                err = _x1s[id].setCCWMargin(val);
                if (err != DX1MOTOR_ERR_OK) {
                    s.print("ERROR ");
                    s.println(err);
                }
                else {
                    s.println("k");
                }
            } break;
            case '-': {
                long val = _argi;
                err = _x1s[id].setCWSlope(val);
                if (err != DX1MOTOR_ERR_OK) {
                    s.print("ERROR ");
                    s.println(err);
                }
                else {
                    s.println("k");
                }
            } break;
            case '=': {
                long val = _argi;
                err = _x1s[id].setCCWSlope(val);
                if (err != DX1MOTOR_ERR_OK) {
                    s.print("ERROR ");
                    s.println(err);
                }
                else {
                    s.println("k");
                }
            } break;
            default:
            s.println("ERROR: Bad command");
            return;
        }
        s.flush();
    }
    else if (mode == MODE_GET) {
        switch (command) {
            case 'v': {
                float res;
                res = _x1s[id].getVoltage(err);
                if (err != DX1MOTOR_ERR_OK) {
                    s.print("ERROR ");
                    s.println(err);
                }
                else {
                    s.print('k');
                    char *ptr = (char*)&res;
                    s.write(ptr[0]);
                    s.write(ptr[1]);
                    s.write(ptr[2]);
                    s.write(ptr[3]);
                    //s.write('\n');
                }
            } break;
            case 'p': {
                float res;
                res = _x1s[id].getPosition(err);
                if (err != DX1MOTOR_ERR_OK) {
                    s.print("ERROR ");
                    s.println(err);
                }
                else {
                    s.print('k');
                    char *ptr = (char*)&res;
                    s.write(ptr[0]);
                    s.write(ptr[1]);
                    s.write(ptr[2]);
                    s.write(ptr[3]);
                    //s.write('\n');
                }
            } break;
            case 'l': {
                float res;
                res = _x1s[id].getLoad(err);
                if (err != DX1MOTOR_ERR_OK) {
                    s.print("ERROR ");
                    s.println(err);
                }
                else {
                    s.print('k');
                    char *ptr = (char*)&res;
                    s.write(ptr[0]);
                    s.write(ptr[1]);
                    s.write(ptr[2]);
                    s.write(ptr[3]);
                    //s.write('\n');
                }
            } break;
            case 't': {
                long res;
                res = _x1s[id].getTemperature(err);
                if (err != DX1MOTOR_ERR_OK) {
                    s.print("ERROR ");
                    s.println(err);
                }
                else {
                    s.print('k');
                    char *ptr = (char*)&res;
                    s.write(ptr[0]);
                    s.write(ptr[1]);
                    s.write(ptr[2]);
                    s.write(ptr[3]);
                    //s.write('\n');
                }
            } break;
            case 's': {
                float res;
                res = _x1s[id].getSpeed(err);
                if (err != DX1MOTOR_ERR_OK) {
                    s.print("ERROR ");
                    s.println(err);
                }
                else {
                    s.print('k');
                    char *ptr = (char*)&res;
                    s.write(ptr[0]);
                    s.write(ptr[1]);
                    s.write(ptr[2]);
                    s.write(ptr[3]);
                    //s.write('\n');
                }
            } break;
            case 'P': {
                float res;
                res = _x1s[id].getGoalPosition(err);
                if (err != DX1MOTOR_ERR_OK) {
                    s.print("ERROR ");
                    s.println(err);
                }
                else {
                    s.print('k');
                    char *ptr = (char*)&res;
                    s.write(ptr[0]);
                    s.write(ptr[1]);
                    s.write(ptr[2]);
                    s.write(ptr[3]);
                    //s.write('\n');
                }
            } break;
            case 'S': {
                float res;
                res = _x1s[id].getGoalSpeed(err);
                if (err != DX1MOTOR_ERR_OK) {
                    s.print("ERROR ");
                    s.println(err);
                }
                else {
                    s.print('k');
                    char *ptr = (char*)&res;
                    s.write(ptr[0]);
                    s.write(ptr[1]);
                    s.write(ptr[2]);
                    s.write(ptr[3]);
                    //s.write('\n');
                }
            } break;
            case 'e': {
                long res;
                res = _x1s[id].getLED(err);
                if (err != DX1MOTOR_ERR_OK) {
                    s.print("ERROR ");
                    s.println(err);
                }
                else {
                    s.print('k');
                    char *ptr = (char*)&res;
                    s.write(ptr[0]);
                    s.write(ptr[1]);
                    s.write(ptr[2]);
                    s.write(ptr[3]);
                    //s.write('\n');
                }
            } break;
            case 'q': {
                float res;
                res = _x1s[id].getMaxTorque(err);
                if (err != DX1MOTOR_ERR_OK) {
                    s.print("ERROR ");
                    s.println(err);
                }
                else {
                    s.print('k');
                    char *ptr = (char*)&res;
                    s.write(ptr[0]);
                    s.write(ptr[1]);
                    s.write(ptr[2]);
                    s.write(ptr[3]);
                    //s.write('\n');
                }
            } break;
            case '+': {
                long res;
                res = _x1s[id].getCWMargin(err);
                if (err != DX1MOTOR_ERR_OK) {
                    s.print("ERROR ");
                    s.println(err);
                }
                else {
                    s.print('k');
                    char *ptr = (char*)&res;
                    s.write(ptr[0]);
                    s.write(ptr[1]);
                    s.write(ptr[2]);
                    s.write(ptr[3]);
                    //s.write('\n');
                }
            } break;
            case '_': {
                long res;
                res = _x1s[id].getCCWMargin(err);
                if (err != DX1MOTOR_ERR_OK) {
                    s.print("ERROR ");
                    s.println(err);
                }
                else {
                    s.print('k');
                    char *ptr = (char*)&res;
                    s.write(ptr[0]);
                    s.write(ptr[1]);
                    s.write(ptr[2]);
                    s.write(ptr[3]);
                    //s.write('\n');
                }
            } break;
            case '-': {
                long res;
                res = _x1s[id].getCWSlope(err);
                if (err != DX1MOTOR_ERR_OK) {
                    s.print("ERROR ");
                    s.println(err);
                }
                else {
                    s.print('k');
                    char *ptr = (char*)&res;
                    s.write(ptr[0]);
                    s.write(ptr[1]);
                    s.write(ptr[2]);
                    s.write(ptr[3]);
                    //s.write('\n');
                }
            } break;
            case '=': {
                long res;
                res = _x1s[id].getCCWSlope(err);
                if (err != DX1MOTOR_ERR_OK) {
                    s.print("ERROR ");
                    s.println(err);
                }
                else {
                    s.print('k');
                    char *ptr = (char*)&res;
                    s.write(ptr[0]);
                    s.write(ptr[1]);
                    s.write(ptr[2]);
                    s.write(ptr[3]);
                    //s.write('\n');
                }
            } break;
            default:
            s.println("ERROR: Bad command");
            return;
        }
        s.flush();
    }
}


void Protocol::dispatchV2Command(Stream &s, int mode, char command, char id) {
    int err;

    if (mode == MODE_SET) {
        switch (command) {
            case 'P': {
                float val = _argf;
                err = _x2s[id].setGoalPosition(val);
                if (err != DX2MOTOR_ERR_OK) {
                    s.print("ERROR ");
                    s.println(err);
                }
                else {
                    s.println("k");
                }
            } break;
            case 'S': {
                float val = _argf;
                err = _x2s[id].setGoalSpeed(val);
                if (err != DX2MOTOR_ERR_OK) {
                    s.print("ERROR ");
                    s.println(err);
                }
                else {
                    s.println("k");
                }
            } break;
            case 'e': {
                long val = _argi;
                err = _x2s[id].setLED(val);
                if (err != DX2MOTOR_ERR_OK) {
                    s.print("ERROR ");
                    s.println(err);
                }
                else {
                    s.println("k");
                }
            } break;
            case 'q': {
                float val = _argf;
                err = _x2s[id].setMaxTorque(val);
                if (err != DX2MOTOR_ERR_OK) {
                    s.print("ERROR ");
                    s.println(err);
                }
                else {
                    s.println("k");
                }
            } break;
            default:
            s.println("ERROR: Bad command");
            return;
        }
        s.flush();
    }
    else if (mode == MODE_GET) {
        switch (command) {
            case 'v': {
                float res;
                res = _x2s[id].getVoltage(err);
                if (err != DX2MOTOR_ERR_OK) {
                    s.print("ERROR ");
                    s.println(err);
                }
                else {
                    s.print('k');
                    char *ptr = (char*)&res;
                    s.write(ptr[0]);
                    s.write(ptr[1]);
                    s.write(ptr[2]);
                    s.write(ptr[3]);
                    //s.write('\n');
                }
            } break;
            case 'p': {
                float res;
                res = _x2s[id].getPosition(err);
                if (err != DX2MOTOR_ERR_OK) {
                    s.print("ERROR ");
                    s.println(err);
                }
                else {
                    s.print('k');
                    char *ptr = (char*)&res;
                    s.write(ptr[0]);
                    s.write(ptr[1]);
                    s.write(ptr[2]);
                    s.write(ptr[3]);
                    //s.write('\n');
                }
            } break;
            case 'l': {
                float res;
                res = _x2s[id].getLoad(err);
                if (err != DX2MOTOR_ERR_OK) {
                    s.print("ERROR ");
                    s.println(err);
                }
                else {
                    s.print('k');
                    char *ptr = (char*)&res;
                    s.write(ptr[0]);
                    s.write(ptr[1]);
                    s.write(ptr[2]);
                    s.write(ptr[3]);
                    //s.write('\n');
                }
            } break;
            case 't': {
                long res;
                res = _x2s[id].getTemperature(err);
                if (err != DX2MOTOR_ERR_OK) {
                    s.print("ERROR ");
                    s.println(err);
                }
                else {
                    s.print('k');
                    char *ptr = (char*)&res;
                    s.write(ptr[0]);
                    s.write(ptr[1]);
                    s.write(ptr[2]);
                    s.write(ptr[3]);
                    //s.write('\n');
                }
            } break;
            case 's': {
                float res;
                res = _x2s[id].getSpeed(err);
                if (err != DX2MOTOR_ERR_OK) {
                    s.print("ERROR ");
                    s.println(err);
                }
                else {
                    s.print('k');
                    char *ptr = (char*)&res;
                    s.write(ptr[0]);
                    s.write(ptr[1]);
                    s.write(ptr[2]);
                    s.write(ptr[3]);
                    //s.write('\n');
                }
            } break;
            case 'P': {
                float res;
                res = _x2s[id].getGoalPosition(err);
                if (err != DX2MOTOR_ERR_OK) {
                    s.print("ERROR ");
                    s.println(err);
                }
                else {
                    s.print('k');
                    char *ptr = (char*)&res;
                    s.write(ptr[0]);
                    s.write(ptr[1]);
                    s.write(ptr[2]);
                    s.write(ptr[3]);
                    //s.write('\n');
                }
            } break;
            case 'S': {
                float res;
                res = _x2s[id].getGoalSpeed(err);
                if (err != DX2MOTOR_ERR_OK) {
                    s.print("ERROR ");
                    s.println(err);
                }
                else {
                    s.print('k');
                    char *ptr = (char*)&res;
                    s.write(ptr[0]);
                    s.write(ptr[1]);
                    s.write(ptr[2]);
                    s.write(ptr[3]);
                    //s.write('\n');
                }
            } break;
            case 'e': {
                long res;
                res = _x2s[id].getLED(err);
                if (err != DX2MOTOR_ERR_OK) {
                    s.print("ERROR ");
                    s.println(err);
                }
                else {
                    s.print('k');
                    char *ptr = (char*)&res;
                    s.write(ptr[0]);
                    s.write(ptr[1]);
                    s.write(ptr[2]);
                    s.write(ptr[3]);
                    //s.write('\n');
                }
            } break;
            case 'q': {
                float res;
                res = _x2s[id].getMaxTorque(err);
                if (err != DX2MOTOR_ERR_OK) {
                    s.print("ERROR ");
                    s.println(err);
                }
                else {
                    s.print('k');
                    char *ptr = (char*)&res;
                    s.write(ptr[0]);
                    s.write(ptr[1]);
                    s.write(ptr[2]);
                    s.write(ptr[3]);
                    //s.write('\n');
                }
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
    const int bLen = 128;
    char buffer[bLen];
    int ind = 0;
    while (waitTimeout(s, 3) && ind < bLen) {
        char b = s.read();
        if (b == 0 && ind == 0) continue;
        buffer[ind++] = b;
    }

    if (ind < 4) {
        // Noise, probably
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
    default:
        s.print("ERROR: Mode error ");
        s.println((int)read);
        return;
    }

    // Second character indicates command
    char command_byte = buffer[1];

    // Servo protocol
    char dx_ver = buffer[2];
    // and ID
    char targ_id = buffer[3];

    if (mode == MODE_SET) {
        if (ind < 8) {
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