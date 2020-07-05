# Bruno Capuano
# start a webserver with flask in a thread
# start a different thread +1 a shared var

from flask import Flask                                                         
import threading
import time

iCounter = 0
data = 'foo'
app = Flask(__name__)

def mainSum():
    # increment counter every second
    global iCounter
    while True:
        iCounter = iCounter + 1
        t = time.localtime()
        current_time = time.strftime("%H:%M:%S", t)    
        print(str(f"{current_time} - data {iCounter}"))
        time.sleep(1)


@app.route("/getdata")
def main():
    global iCounter
    t = time.localtime()
    current_time = time.strftime("%H:%M:%S", t)    
    return str(f"{current_time} - data {iCounter}")

if __name__ == "__main__":
    stateThread = threading.Thread(target=mainSum)
    stateThread.daemon = True
    stateThread.start()

    webThread = threading.Thread(target=app.run)
    webThread.start()

    # #threading.Thread(target=app.run).start()

