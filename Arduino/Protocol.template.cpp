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

{% macro dispatchFunction(version) %}
void Protocol::dispatchV{{version}}Command(Stream &s, int mode, char command, char id) {
    int err;

    if (mode == MODE_SET) {
        switch (command) {
            {% for c in commands if c.can_set %}
            case '{{c.short}}': {
                {% if c.type == 'float' %}
                float val = _argf;
                {% else %}
                long val = _argi;
                {% endif %}
                err = _x{{version}}s[id].set{{c.name}}(val);
                if (err != DX{{version}}MOTOR_ERR_OK) {
                    s.print("ERROR ");
                    s.println(err);
                }
                else {
                    s.println("k");
                }
            } break;
            {% endfor %}
            default:
            s.println("ERROR: Bad command");
            return;
        }
        s.flush();
    }
    else if (mode == MODE_GET) {
        switch (command) {
            {% for c in commands if c.can_get %}
            case '{{c.short}}': {
                {% if c.type == 'float' %}
                float res;
                {% else %}
                long res;
                {% endif %}
                res = _x{{version}}s[id].get{{c.name}}(err);
                if (err != DX{{version}}MOTOR_ERR_OK) {
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
            {% endfor %}
            default:
            s.println("ERROR: Bad command");
            return;
        }
        s.flush();
    }
}
{% endmacro %}

{{dispatchFunction(1)}}

{{dispatchFunction(2)}}

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
