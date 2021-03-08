# Copyright (c) Bruno Capuano. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.
# Using the Python Device SDK for IoT Hub:
#   https://github.com/Azure/azure-iot-sdk-python
# Azure IoT dependencies installed with >> pip install azure-iot-device
# read temp and humi from DHT11 Sensor
# read values are triggered device message with the information.

import time
import os
import sys
import asyncio
import seeed_dht

from six.moves import input
import threading
from azure.iot.device.aio import IoTHubModuleClient
from azure.iot.device import IoTHubDeviceClient, Message
from AzureIoTLogger import AzureIoTLogger
from AzureIoTEnvironment import AzureIoTEnvironment

trigger_enabled = False
refresh_interval = 5
MSG_TXT = '{{"temperature": {temperature},"humidity": {humidity}}}'
sensor = None
log = None

def twin_patch_handler(patch):
    AzureIoTLogger.Log("the data in the desired properties patch was: {}".format(patch))

async def main():
    global refresh_interval, sensor, module_client, log
    try:
        if not sys.version >= "3.5.3":
            raise Exception( "The sample requires python 3.5.3+. Current version of Python: %s" % sys.version )

        # The client object is used to interact with your Azure IoT hub.
        module_client = IoTHubModuleClient.create_from_edge_environment()
        AzureIoTLogger.Log("IoT Hub Client for Python OK" )

        # read environmental vars
        trigger_enabled = AzureIoTEnvironment.GetEnvVarBool('ReportValues')
        refresh_interval = AzureIoTEnvironment.GetEnvVarInt('Interval')

        # connect the client.
        await module_client.connect()
        AzureIoTLogger.Log("Azure IoT client connected OK" )

        # set the twin patch handler on the client
        module_client.on_twin_desired_properties_patch_received = twin_patch_handler

        # init sensor
        sensor = seeed_dht.DHT("11", 12)
        AzureIoTLogger.Log("Temp/Humi sensor OK" )

        while True:
            try:
                humi, temp = sensor.read()
                if not humi is None:
                    AzureIoTLogger.Log('DHT{0}, humidity {1:.1f}%, temperature {2:.1f}*'.format(sensor.dht_type, humi, temp))
                else:
                    AzureIoTLogger.Log('DHT{0}, humidity & temperature: {1}'.format(sensor.dht_type, temp))

                if (trigger_enabled == True):
                    msg_txt_formatted = MSG_TXT.format(temperature=temp, humidity=humi)
                    message = Message(msg_txt_formatted)
                    message.custom_properties["temperature"] = temp
                    message.custom_properties["humidity"] = humi
                    await module_client.send_message(message)
            except Exception as e:
                AzureIoTLogger.Log("stdin_listener exception.", e)
            finally:            
                time.sleep(refresh_interval)

        # Cancel listening
        listeners.cancel()

        # Finally, disconnect
        await module_client.disconnect()

    except Exception as e:
        AzureIoTLogger.Log ( "Unexpected error %s " % e )
        raise

if __name__ == "__main__":
    asyncio.run(main())