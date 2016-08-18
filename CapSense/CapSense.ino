#include <CapacitiveSensor.h>

CapacitiveSensor cs = CapacitiveSensor(2,4);

void setup() {
  // put your setup code here, to run once:
  Serial.begin(250000);
  while (!Serial);
}

void loop() {
  // put your main code here, to run repeatedly:
  Serial.println(cs.capacitiveSensor(40));
}
