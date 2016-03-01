using System;
using System.Text;
using System.Threading;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using PclAzureIoT;

namespace ConsoleAzureIoT03
{
    class Program
    {
        static DeviceClient _deviceClient;

        static void Main()
        {
            Console.WriteLine("Simulated device\n");
            _deviceClient = DeviceClient.Create(Config.IotHubUri, 
                new DeviceAuthenticationWithRegistrySymmetricKey(Config.DeviceId, Config.DeviceKey));
            SendDeviceToCloudMessagesAsync();
            Console.ReadLine();
        }

        private static async void SendDeviceToCloudMessagesAsync()
        {
            const int avgWindSpeed = 10; // m/s
            var rand = new Random();
            while (true)
            {
                var currentWindSpeed = avgWindSpeed + rand.NextDouble() * 4 - 2;
                var telemetryDataPoint = new
                {
                    deviceId = Config.DeviceId,
                    windSpeed = currentWindSpeed
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));
                await _deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);
                Thread.Sleep(1000);
            }
        }
    }
}
