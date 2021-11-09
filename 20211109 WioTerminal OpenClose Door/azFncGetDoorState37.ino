/*
  Bruno Capuano, https://www.elbruno.com
  
  Permission is hereby granted, free of charge, to any person obtaining a copy
  of this software and associated documentation files.

  The above copyright notice and this permission notice shall be included in all
  copies or substantial portions of the Software.

  - check the door state with http get from an azure function
  - process the http response with JSON format
  - validate door state to show open/closed image
  - show xbmp image for open / closed door
  - display countdown progress bar on the screen
  - press button A and B, open / close the door
*/

#include "rpcWiFi.h"
#include <HTTPClient.h>
#include <Arduino_JSON.h>
#include "Seeed_FS.h" //Including SD card library
#include "opencloseicons.h"  // Sketch tab header for xbm images

#include"TFT_eSPI.h"
TFT_eSPI tft;

#include"RawImage.h"  //Including image processing library

// WiFi information
const char* ssid     = "";
const char* password = "";
int connectingSeconds = 0;

// Set timer to 5 seconds
unsigned long timerDelay = 10000;
unsigned long lastTime   = 0;

// Refresh Countdown Progress bar
unsigned long lastTimeCounter = 0;
int countdownCounter     = 0;
int blocksPerSecond      = 2000;
String progressBarPrefix = "@> ";

// HTTP POST / GET
HTTPClient http;
unsigned long timerDelayHttpPost = 5000;
unsigned long lastTimeHttpPost   = 0;
bool performHttpPost             = false;
String url                       = "";
const char * urlGetData          = "http://***.azurewebsites.net/api/";
const char * urlOpen             = "http://***.azurewebsites.net/api/";
const char * urlClose            = "http://***.azurewebsites.net/api/";


// Azure Function and Door Information
String acFncResponse;
int    state        = -1;
String state_desc   = "undefined";
String state_source = "undefined";
int    last_state   = -1;

// TFT Display
const int col0 = 20;
const int row0 = 30;
const int row1 = 55;
const int row2 = 80;
const int row3 = 105;
const int row4 = 130;
const int row5 = 155;
const int row6 = 180;
const int row7 = 205;

void setup() {
  Serial.begin(115200);

  //Initialise SD card
  Serial.println("Initialise SD card");
  if (!SD.begin(SDCARD_SS_PIN, SDCARD_SPI)) {
      while (1);
  }
  Serial.println("Initialise SD card Done");

  // init buttons
  Serial.println("Init buttons");  
  pinMode(WIO_KEY_A, INPUT_PULLUP);
  pinMode(WIO_KEY_B, INPUT_PULLUP);
  pinMode(WIO_KEY_C, INPUT_PULLUP);

  tft.begin();
  tft.setRotation(3);
  tft.fillScreen(TFT_BLACK);
  tft.setTextSize(2);

  tftPrintLine(row0, "El Bruno - @elbruno", "");
  tftPrintLine(row1, "Connecting to WiFi...", "");
  
  // Set WiFi to station mode and disconnect from an AP if it was previously connected
  Serial.println("Connecting");
  WiFi.mode(WIFI_STA);
  WiFi.disconnect();
  WiFi.begin(ssid, password);

  // attempt to connect to Wifi network:
  while (WiFi.status() != WL_CONNECTED) {
      connectingSeconds++;
      tftPrintLine(row2, "Seconds waiting: ", String(connectingSeconds));
      // wait 1 second for re-trying
      delay(1000);
      WiFi.begin(ssid, password);
  }

  // init action time validators
  lastTimeCounter  = millis();
  lastTimeHttpPost = millis();
  tftPrintLine(row3, "Connected!", "");

  Serial.println("");
  Serial.print("Connected to WiFi network with IP Address: ");
  Serial.println(WiFi.localIP()); 
  delay(2000);

  // Clean screen to show results
  tftInit();  

  initProgressBarCountDown();
}

void loop() {

  validateWioButtons();
    
  if ((millis() - lastTime) > timerDelay) {

    //Check WiFi connection status
    if(WiFi.status()== WL_CONNECTED){

      acFncResponse = httpGETRequest();
      getDoorStateInformation(acFncResponse);

      Serial.print("Response processed. State: ");
      Serial.println(state);

      if(last_state != state){
        Serial.println ("State and Last state are different, process the changes");
        last_state = state;
        if(state == 0){
          tft.drawXBitmap(logo_x, logo_y, closedlogo_bits, logo_width, logo_width, TFT_RED, TFT_BLACK );          
        }
        else if (state == 1){
          tft.drawXBitmap(logo_x, logo_y, openlogo_bits, logo_width, logo_width, TFT_GREEN, TFT_BLACK );
        }
        else if (state == -1){
          tft.drawXBitmap(logo_x, logo_y, openlogo_bits, logo_width, logo_width, TFT_BLACK, TFT_BLACK);          
        }

        tftPrintLine(row3, "Source: ", state_source);      
        tftPrintLine(row4, "Door: ", state_desc);              
      }
    }
    else {
      Serial.println("WiFi Disconnected");
      tftPrintLine(row3, "WiFi Disconnected", "");
    }
    lastTime = millis();
    initProgressBarCountDown();
  } else {
    displayRefreshProgressBar();
  }
}

void initProgressBarCountDown() {
  countdownCounter = (timerDelay / blocksPerSecond);  
}

void displayRefreshProgressBar() {
  // display progress timer to next get info
  if ((millis() - lastTimeCounter) > blocksPerSecond) {
    countdownCounter--;
    lastTimeCounter = millis();

    int prefixLength = progressBarPrefix.length();
    int progressBarLength = prefixLength + (timerDelay / blocksPerSecond);
    String progressBar = progressBarPrefix;
 
    for(int i = prefixLength; i <= progressBarLength - 1 ; i++) {
      if (i <= (countdownCounter + prefixLength) - 1){
        progressBar += '0';
        }
      else {
        progressBar += '.';  
      }
    }    

    // draw progress bar
    tftPrintLineNoClean(row7, progressBar, "");  
  }
}

String httpGETRequest() {

  Serial.println("Start http GET request");

  //HTTPClient http;
  http.end();  
  http.setTimeout(3000);  
  http.begin(urlGetData);

  int httpResponseCode = http.GET();
  Serial.println("http GET done");  
  
  String payload = ""; 
  
  if (httpResponseCode>0) {
    Serial.print("HTTP Response code: ");
    Serial.println(httpResponseCode);
    payload = http.getString();
  }
  else {
    Serial.print("Error code: ");
    Serial.println(httpResponseCode);
  }
  // Free resources
  http.end();

  return payload;
}

void getDoorStateInformation(String response){
  // init default values
  state = -1;
  state_desc = "undefined";
  state_source = "undefined";  

  // process JSon Response
  JSONVar myObject = JSON.parse(response);
  
  if (JSON.typeof(myObject) == "undefined") {
    Serial.println("Parsing input failed!");
    return;
  }
  
  if (myObject.hasOwnProperty("state")) {
    state = (int)myObject["state"];
  }
  if (myObject.hasOwnProperty("desc")) {
    state_desc = myObject["desc"];
  }
  if (myObject.hasOwnProperty("source")) {
    state_source = myObject["source"];
  }
  
  Serial.print("state: ");
  Serial.print(state);
  Serial.print(" - desc: ");
  Serial.print(state_desc);
  Serial.print(" - source: ");
  Serial.println(state_source); 
}

void tftInit(){
  tft.fillScreen(TFT_BLACK);
  tftPrintLine(row0, "El Bruno - @elbruno", "");
  tftPrintLine(row1, "Azure IoT Door State", "");
}

void tftCleanLine(int row){
  tftCleanLineCol(col0, row);
}

void tftCleanLineCol(int col, int row){
    tft.setCursor(col, row);
    tft.print("                         ");
}

void tftPrintLineCol(int col, int row, String messagePrefix, String message, bool cleanLine){
  if ( cleanLine == true ){
     tftCleanLineCol(col,row);
  }
  tft.setCursor(col, row);  
  tft.print(messagePrefix);
  tft.setCursor((col0 + tft.textWidth(messagePrefix)), row);
  tft.print(message);      
}

void tftPrintLine(int row, String messagePrefix, String message){
  tftPrintLineCol(col0, row, messagePrefix, message, true);
}

void tftPrintLineNoClean(int row, String messagePrefix, String message){
  tftPrintLineCol(col0, row, messagePrefix, message, false);
}

void validateWioButtons(){

  if (digitalRead(WIO_KEY_A) == LOW) 
  { 
    Serial.println("Button A Pressed, open door");     
    url = urlOpen;
    performHttpPost = true;
    tftPrintLineNoClean(row5, "[A] ", "Open Door");    
  }
  if (digitalRead(WIO_KEY_B) == LOW) 
  { 
    Serial.println("Button B Pressed, close door");     
    url = urlClose;
    performHttpPost = true;
    tftPrintLineNoClean(row5, "[B] ", "Close Door");        
  }
  if (digitalRead(WIO_KEY_C) == LOW) 
  {
    Serial.println("[C] refresh");     
    tftPrintLineNoClean(row5, "[C] ", "Refresh   ");
    lastTime = 0;
    performHttpPost = false;
  }  

  // validate last time http post call  
  if ((millis() - lastTimeHttpPost ) > timerDelayHttpPost) {  
    if(performHttpPost == true){    
      performHttpPost = false;      
   
      http.end();  
      http.setTimeout(3000);  
      http.begin(url);
    
      int httpResponseCode = http.POST();
      if (httpResponseCode>0) {
        Serial.print("HTTP Response code: ");
        Serial.println(httpResponseCode);
      }
      else {
        Serial.print("Error code: ");
        Serial.println(httpResponseCode);
      }
      // Free resources
      http.end();
      lastTime = 0;
      tftPrintLineNoClean(row5, "    ", "          ");
    }

    // update last time http post call
    lastTimeHttpPost = millis();
    performHttpPost = false;
  }
}