# Bruno Capuano
# sinple webserver with flask

from flask import Flask                                                         
import threading

data = 'foo'
app = Flask(__name__)

@app.route("/")
def main():
    return data

if __name__ == "__main__":
    threading.Thread(target=app.run).start()