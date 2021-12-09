#include <Arduino.h>
#include <rpcWiFi.h>
#include <SPI.h>
 
void setup() {
    Serial.begin(115200);
    while(!Serial); // Wait to open Serial Monitor
    Serial.printf("RTL8720 Firmware Version: %s", rpc_system_version());
}
 
void loop() {
}
