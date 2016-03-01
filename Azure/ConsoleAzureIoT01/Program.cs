using System;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using PclAzureIoT;

namespace ConsoleAzureIoT01
{
    class Program
    {
        static RegistryManager _registryManager;

        static void Main()
        {
            _registryManager = 
                RegistryManager.CreateFromConnectionString(Config.ConnectionString);
            AddDeviceAsync().Wait();
            Console.ReadLine();
        }

        private async static Task AddDeviceAsync()
        {
            Device device;
            try
            {
                device = await _registryManager
                    .AddDeviceAsync(new Device(Config.DeviceId));
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await _registryManager.GetDeviceAsync(Config.DeviceId);
            }
            Console.WriteLine("Device Id: {0}", Config.DeviceId);
            Console.WriteLine("Generated device key: {0}",
                device.Authentication.SymmetricKey.PrimaryKey);
        }
    }
}
