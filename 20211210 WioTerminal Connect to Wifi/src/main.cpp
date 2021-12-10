/*
   Copyright (c) 2021
   Author      : Bruno Capuano
   Create Time : 2021 December
   Change Log  :

   - Connect Wio Terminal to Wifi
   - Uses the TFT to display progress message
   - Display Wio Terminal IP in the TFT

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
#include <rpcWiFi.h>
#include <SPI.h>

#include "config.h"

#include "EB_Wifi.h"
// Display text + img on TFT
#include "EB_TFT.h"

int counter = 0;

// ==============================
// WIFI
// ==============================

void connectWiFi()
{
  int startWifiTime = millis();
  int connectAttemps = 0;
  Serial.print("Connecting to Wifi ...");
  tftPrintLine(tft_row1, "Connecting to Wifi ...", "");
  tftPrintLine(tft_row2, "SSID:", SSID);
  WiFi.mode(WIFI_STA);
  WiFi.disconnect();

  while (WiFi.status() != WL_CONNECTED)
  {
    int secondsWaiting = (millis() - startWifiTime) / 1000;
    tftPrintLine(tft_row4, "Seconds waiting: ", String(secondsWaiting));
    tftPrintLine(tft_row5, "Connection attemps: ", String(connectAttemps));
    Serial.println("Connecting to WiFi..");
    Serial.print("Seconds waiting : ");
    Serial.println(secondsWaiting);
    WiFi.begin(SSID, PASSWORD);
    delay(500);
    connectAttemps++;
  }

  // Clean screen to show results
  tftInit();

  Serial.println("Connected!");
  Serial.println(WiFi.localIP());
  tftPrintLine(tft_row3, "Connected to ", SSID);
  tftPrintLine(tft_row4, "IP: ", ip2Str(WiFi.localIP()));
}

// ==============================
// Main App
// ==============================

void setup()
{
  Serial.begin(9600);

  while (!Serial)
    ; // Wait for Serial to be ready

  // init TFT
  tftFirstSetup();

  // connect to Wifi
  connectWiFi();

  delay(2000);
}

void loop()
{
  counter++;
  // put your main code here, to run repeatedly:
  Serial.println("Checking button state");
  tftPrintLine(tft_row6, "Working cycle - ", String(counter));
  delay(3000);
  if (counter > 100)
    counter = 0;
}