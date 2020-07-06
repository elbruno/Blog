# Bruno Capuano
# simple webserver with fastapi
# run with  uvicorn 07:app -reload
# test with http://127.0.0.1:8000/getdata
# on each call, validate if the thread is started, 
# of the thread is None, start a different thread +1 a shared var


from typing import Optional
from fastapi import FastAPI
import threading
import time

stateThread = None
iCounter    = 0
app = FastAPI()

def validateStateThread():
    global stateThread
    if (stateThread is None):
        print(f"start thread")
        stateThread = threading.Thread(target=mainSum)
        stateThread.daemon = True
        stateThread.start()    

@app.get("/getdata")
def main():
    global iCounter
    validateStateThread()

    t = time.localtime()
    current_time = time.strftime("%H:%M:%S", t)    
    return str(f"{current_time} - data {iCounter}")

def mainSum():
    # increment counter every second
    global iCounter
    while True:
        iCounter = iCounter + 1
        t = time.localtime()
        current_time = time.strftime("%H:%M:%S", t)    
        print(str(f"{current_time} - data {iCounter}"))
        time.sleep(1)