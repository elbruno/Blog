#include"TFT_eSPI.h"
#include"Free_Fonts.h" //include the header file
#include "Seeed_FS.h" //Including SD card library
#include"RawImage.h"  //Including image processing library
TFT_eSPI tft;

int counter = 5;

void setup() {
    //Initialise SD card
    if (!SD.begin(SDCARD_SS_PIN, SDCARD_SPI)) {
        while (1);
    }
    tft.begin();
    tft.setRotation(3);
    tft.fillScreen(TFT_BLACK); //Black background
    tft.setFreeFont(FS12);    
}
  
void loop() {

  if(counter == 0)
  {
    drawImage<uint8_t>("bruno.bmp", 0, 0);
    tft.drawString("@elbruno",10,200);    
  }
  else if(counter > 0)
  {    
    counter--;
    tft.drawString("Bruno show in ...",75,100);
    tft.drawString(String(counter),75,125);
    tft.drawString("seconds",100,125);
  }
  delay(1000);  
}
