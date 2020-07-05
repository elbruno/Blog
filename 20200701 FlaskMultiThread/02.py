# Bruno Capuano
# simple webserver with flask
# 

from flask import Flask                                                         
import threading
import time

iCounter = 0
data = 'foo'
app = Flask(__name__)

def mainSum():
    # increment counter every second
    while True:
        iCounter = iCounter + 1
        time.sleep(1)



@app.route("/getdata")
def main():
    t = time.localtime()
    current_time = time.strftime("%H:%M:%S", t)    
    return str(f"{current_time} - data {iCounter}")

if __name__ == "__main__":
    webThread = threading.Thread(target=app.run)
    #webThread.daemon = True
    webThread.start()

    # #threading.Thread(target=app.run).start()

    # stateThread = threading.Thread(target=mainSum)
    # stateThread.daemon = True
    # stateThread.start()
