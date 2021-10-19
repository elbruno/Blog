#include <TFT_eSPI.h> //Hardware-specific library
#include <SPI.h>
#include"Histogram.h" //include histogram library
 
TFT_Histogram histogram=TFT_Histogram(); //Initializing tft and histogram
TFT_eSPI tft = TFT_eSPI();

int counterA = 2;
int counterB = 1;
int counterC = 3;
bool updateValues = false;
 
void setup() {
  // init buttons
  pinMode(WIO_KEY_A, INPUT_PULLUP);
  pinMode(WIO_KEY_B, INPUT_PULLUP);
  pinMode(WIO_KEY_C, INPUT_PULLUP);

  // init histogram
  tft.init();
  histogram.initHistogram(&tft);
  histogram.formHistogram("A", 1, counterA, 40, TFT_RED);   // Column 1
  histogram.formHistogram("B", 2, counterB, 40, TFT_GREEN); // Column 2
  histogram.formHistogram("C", 3, counterC, 50, TFT_BLUE); // Column 3
  histogram.showHistogram(); //show histogram

}
void loop() {
  updateValues = false;
  if (digitalRead(WIO_KEY_A) == LOW) 
  { 
    counterA ++;  
    histogram.changeParam(1, "A", counterA, TFT_RED);
    updateValues = true;
  }
  if (digitalRead(WIO_KEY_B) == LOW) 
  { 
    counterB ++;  
    histogram.changeParam(2, "B", counterB, TFT_GREEN);
    updateValues = true;
  }
  if (digitalRead(WIO_KEY_C) == LOW) 
  { 
    counterC ++;  
    histogram.changeParam(3, "C", counterC, TFT_BLUE);
    updateValues = true;
  }  
  if (updateValues == true)
    delay(500);
 
}
