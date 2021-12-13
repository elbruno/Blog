/*
   Copyright (c) 2021
   Author      : Bruno Capuano
   Create Time : 2021 December
   Change Log  :

   - Custom implementation for Wio Terminal of the Arduino Servo Library https://www.arduino.cc/reference/en/libraries/servo/
   - Button A moves the servo to 90 degress
   - Button B moves the servo to 0  degress

   The MIT License (MIT)

   Permission is hereby granted, free of charge, to any person obtaining a copy
   of this software and associated documentation files (the "Software"), to deal
   in the Software without restriction, including without limitation the rights
   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
   copies of the Software, and to permit persons to whom the Software is
   furnished to do so, subject to the following conditions:

   The above copyright notice and this permission notice shall be included in
   all copies or substantial portions of the Software.

   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
   THE SOFTWARE.
*/
#include <Arduino.h>
#include "EB_Servo.h"

// servo
Servo myservo;
long servoPos = 0;
const long servoOpen = 30;
const long servoClosed = 120;

void setup()
{
  Serial.begin(9600);

  // Init servo
  myservo.attach(D0); // working on right grove port
  myservo.write(servoClosed);

  // init buttons
  Serial.println("Init buttons");
  pinMode(WIO_KEY_A, INPUT_PULLUP);
  pinMode(WIO_KEY_B, INPUT_PULLUP);
  pinMode(WIO_KEY_C, INPUT_PULLUP);

  delay(1000);
}

void loop()
{
  // validate buttons
  if (digitalRead(WIO_KEY_A) == LOW)
  {
    Serial.println("[A] OPEN");
    servoPos = servoOpen;
  }
  if (digitalRead(WIO_KEY_B) == LOW)
  {
    Serial.println("[B] CLOSED ");
    servoPos = servoClosed;
  }
  if (digitalRead(WIO_KEY_C) == LOW)
  {
    Serial.println("[C] DEBUG ");
  }

  Serial.print("Servo Pos: ");
  Serial.println(servoPos);
  myservo.write(servoPos);

  delay(3000);
}
