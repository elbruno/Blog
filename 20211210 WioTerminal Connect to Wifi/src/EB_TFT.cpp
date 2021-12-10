/*
   Copyright (c) 2021
   Author      : Bruno Capuano
   Create Time : 2021 December
   Change Log  :
   
   - Functions to be used in Wio Terminal TFT

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

//#include "TFT_eSPI.h" // Display text + img on TFT
#include <EB_TFT.h>
TFT_eSPI tft;

// TFT Display
const int tft_col0 = 20;
const int tft_row0 = 30;
const int tft_row1 = 55;
const int tft_row2 = 80;
const int tft_row3 = 105;
const int tft_row4 = 130;
const int tft_row5 = 155;
const int tft_row6 = 180;
const int tft_row7 = 205;

// ==============================
// TFT Display
// ==============================

void tftCleanLineCol(int col, int row)
{
  tft.setCursor(col, row);
  tft.print("                         ");
}

void tftCleanLine(int row)
{
  tftCleanLineCol(tft_col0, row);
}

void tftPrintLineCol(int col, int row, String messagePrefix, String message, bool cleanLine)
{
  if (cleanLine == true)
  {
    tftCleanLineCol(col, row);
  }
  tft.setCursor(col, row);
  tft.print(messagePrefix);
  tft.setCursor((tft_col0 + tft.textWidth(messagePrefix)), row);
  tft.print(message);
}

void tftPrintLineNoClean(int row, String messagePrefix, String message)
{
  tftPrintLineCol(tft_col0, row, messagePrefix, message, false);
}

void tftPrintLine(int row, String messagePrefix, String message)
{
  tftPrintLineCol(tft_col0, row, messagePrefix, message, true);
}

void tftInit()
{
  tft.fillScreen(TFT_BLACK);
  tftPrintLine(tft_row0, "Microsoft Reactor", "");
  tftPrintLine(tft_row1, "Azure IoT Squirrels", "");
}

void tftFirstSetup()
{
  tft.begin();
  tft.setRotation(3);
  tft.fillScreen(TFT_BLACK);
  tft.setTextSize(2);
}