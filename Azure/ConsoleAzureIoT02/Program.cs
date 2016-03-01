using System;
using System.Text;
using Microsoft.ServiceBus.Messaging;
using PclAzureIoT;

namespace ConsoleAzureIoT02
{
    class Program
    {

        static EventHubClient _eventHubClient;

        static void Main()
        {
            Console.WriteLine("Receive messages\n");
            _eventHubClient = EventHubClient
                .CreateFromConnectionString(Config.ConnectionString, Config.IotHubD2CEndpoint);

            var d2CPartitions = _eventHubClient.GetRuntimeInformation().PartitionIds;

            foreach (var partition in d2CPartitions)
            {
                ReceiveMessagesFromDeviceAsync(partition);
            }
            Console.ReadLine();
        }
        private async static void ReceiveMessagesFromDeviceAsync(string partition)
        {
            var eventHubReceiver = _eventHubClient
                .GetDefaultConsumerGroup()
                .CreateReceiver(partition, DateTime.UtcNow);
            while (true)
            {
                var eventData = await eventHubReceiver.ReceiveAsync();
                if (eventData == null) continue;

                var data = Encoding.UTF8.GetString(eventData.GetBytes());
                Console.WriteLine
                    ($"Message received. Partition: {partition} Data: '{data}'");
            }
        }
    }
}
