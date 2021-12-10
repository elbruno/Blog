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
#pragma once

#ifndef EB_TFT_H
#define EB_TFT_H

#include <Arduino.h>
#include "TFT_eSPI.h" // Display text + img on TFT

// TFT Display
extern const int tft_col0;
extern const int tft_row0;
extern const int tft_row1;
extern const int tft_row2;
extern const int tft_row3;
extern const int tft_row4;
extern const int tft_row5;
extern const int tft_row6;
extern const int tft_row7;

void tftCleanLineCol(int col, int row);
void tftCleanLine(int row);
void tftPrintLineCol(int col, int row, String messagePrefix, String message, bool cleanLine);
void tftPrintLineNoClean(int row, String messagePrefix, String message);
void tftPrintLine(int row, String messagePrefix, String message);
void tftInit();
void tftFirstSetup();

#endif