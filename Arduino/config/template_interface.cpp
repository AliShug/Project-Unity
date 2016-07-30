// Auto-generated file
// DO NOT DIRECTLY MODIFY

#include "DX1Motor.h"
#include "DX2Motor.h"

{{dx2lib}} small_servo(1);

void setup() {

    Serial.begin(921600);
    {{dx2lib}}::Init({{dx2lib|upper}}_BAUD_1MBS, {{comm_pin}});
    delay(500);

}

void loop() {

    /*
        N[g|s][attrib]
        N = 1 | Protocol 1
          = 2 | Protocol 2
        g - get
        s - set
    */

    if (Serial.available()) {
        int id = Serial.readString().toInt();
        String command = Serial.readString();
        

        {% for item in api %}
        if (command == "{{item}}") {

        }
        {% endfor %}
    }

}
