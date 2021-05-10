# Bruno Capuano - 2021
# Azure IoT Module running an Azure Custom Vision webapp

import time
import os
import sys
import asyncio
import threading

from six.moves import input
from azure.iot.device.aio import IoTHubModuleClient
from azure.iot.device import IoTHubDeviceClient, Message
from AzureIoTLogger import AzureIoTLogger
from AzureIoTEnvironment import AzureIoTEnvironment

# Custom Vision for ARM imports
import json
import io

# Imports for the REST API
from flask import Flask, request, jsonify

# Imports for image procesing
from PIL import Image

# Imports for prediction
from predict import initialize, predict_image, predict_url

module_client = None
trigger_enabled = False

AzureIoTLogger.Log( "init flask app" )
app = Flask(__name__)

# 4MB Max image size limit
app.config['MAX_CONTENT_LENGTH'] = 4 * 1024 * 1024 

AzureIoTLogger.Log("Load and intialize the model")
initialize()

trigger_enabled = AzureIoTEnvironment.GetEnvVarBool('ReportValues')


async def initAzureIoTModule():
    global module_client
    try:
        if not sys.version >= "3.5.3":
            raise Exception( "The sample requires python 3.5.3+. Current version of Python: %s" % sys.version )
        AzureIoTLogger.Log( "IoT Hub Client for Python" )

        # The client object is used to interact with your Azure IoT hub.
        module_client = IoTHubModuleClient.create_from_edge_environment()

        # connect the client.
        await module_client.connect()

        # define behavior for receiving an input message on input1
        async def input1_listener(module_client):
            while True:
                input_message = await module_client.receive_message_on_input("input1")  # blocking call
                AzureIoTLogger.Log("the data in the message received on input1 was ")
                AzureIoTLogger.Log(input_message.data)
                AzureIoTLogger.Log("custom properties are")
                AzureIoTLogger.Log(input_message.custom_properties)
                AzureIoTLogger.Log("forwarding mesage to output1")
                await module_client.send_message_to_output(input_message, "output1")

        # Schedule task for C2D Listener
        listeners = asyncio.gather(input1_listener(module_client))
        AzureIoTLogger.Log( "Azure IoT Module registerd and waiting for messages.")

    except Exception as e:
        AzureIoTLogger.Log('EXCEPTION:' + str(e))



# =============================================
# webapp for custom vision
# =============================================
async def webAppCustomVision():
    # Run the server
    AzureIoTLogger.Log ( "Start and run CV webapp" )
    app.run(host='0.0.0.0', port=8089)    

async def send_iot_message(strMessage):
    global module_client
    if (trigger_enabled == False):
        return

    # uncomment to have the body with the full message
    # MSG_TXT = '{{"cvresult": {cvresult}}}'
    # msg_txt_formatted = MSG_TXT.format(cvresult=strMessage)
    message = Message("")

    # load json def and add each tag as properties
    jsonStr = strMessage.get_data(as_text=True)
    jsonObj = json.loads(jsonStr)

    preds = jsonObj['predictions'] 
    sorted_preds = sorted(preds, key=lambda x: x['probability'], reverse=True)
    if (sorted_preds):
        for pred in sorted_preds:
            # tag name and prob * 100
            tagName     = str(pred['tagName'])
            probability = pred['probability'] * 100
            message.custom_properties[tagName] = str(probability)
            AzureIoTLogger.Log(f'tag: {tagName} - prob: {probability}')

    await module_client.send_message(message)

# Default route just shows simple text
@app.route('/')
def index():
    return 'CustomVision.ai simple drawing server.'

# Like the CustomVision.ai Prediction service /image route handles either
#     - octet-stream image file 
#     - a multipart/form-data with files in the imageData parameter
@app.route('/image', methods=['POST'])
@app.route('/<project>/image', methods=['POST'])
@app.route('/<project>/image/nostore', methods=['POST'])
@app.route('/<project>/classify/iterations/<publishedName>/image', methods=['POST'])
@app.route('/<project>/classify/iterations/<publishedName>/image/nostore', methods=['POST'])
@app.route('/<project>/detect/iterations/<publishedName>/image', methods=['POST'])
@app.route('/<project>/detect/iterations/<publishedName>/image/nostore', methods=['POST'])
def predict_image_handler(project=None, publishedName=None):
    try:
        imageData = None
        if ('imageData' in request.files):
            imageData = request.files['imageData']
        elif ('imageData' in request.form):
            imageData = request.form['imageData']
        else:
            imageData = io.BytesIO(request.get_data())

        img = Image.open(imageData)
        results = predict_image(img)

        jsonResults = jsonify(results)
        asyncio.run(send_iot_message(jsonResults))

        return jsonResults

    except Exception as e:
        AzureIoTLogger.Log('EXCEPTION:' + str(e))
        return 'Error processing image', 500


# Like the CustomVision.ai Prediction service /url route handles url's
# in the body of hte request of the form:
#     { 'Url': '<http url>'}  
@app.route('/url', methods=['POST'])
@app.route('/<project>/url', methods=['POST'])
@app.route('/<project>/url/nostore', methods=['POST'])
@app.route('/<project>/classify/iterations/<publishedName>/url', methods=['POST'])
@app.route('/<project>/classify/iterations/<publishedName>/url/nostore', methods=['POST'])
@app.route('/<project>/detect/iterations/<publishedName>/url', methods=['POST'])
@app.route('/<project>/detect/iterations/<publishedName>/url/nostore', methods=['POST'])
def predict_url_handler(project=None, publishedName=None):
    try:
        image_url = json.loads(request.get_data().decode('utf-8'))['url']
        results = predict_url(image_url)

        jsonResults = jsonify(results)
        asyncio.run(send_iot_message(jsonResults))

        return jsonResults
    except Exception as e:
        AzureIoTLogger.Log('EXCEPTION:' + str(e))
        return 'Error processing image'

if __name__ == "__main__":
    loop = asyncio.get_event_loop()
    loop.run_until_complete(initAzureIoTModule())
    loop.run_until_complete(webAppCustomVision())        
    #loop.close()