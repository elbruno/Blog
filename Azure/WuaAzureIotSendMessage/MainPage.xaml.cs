using System;
using System.Text;
using Windows.UI.Xaml;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using PclAzureIoT;

namespace WuaAzureIotSendMessage
{
    public sealed partial class MainPage
    {
        DeviceClient _deviceClient;
        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var authenticationMethod = new DeviceAuthenticationWithRegistrySymmetricKey(Config.DeviceId, Config.DeviceKey);
            _deviceClient = DeviceClient.Create(Config.IotHubUri, authenticationMethod, TransportType.Http1);
        }

        private async void ButtonSendMessage_Click(object sender, RoutedEventArgs e)
        {
            var rand = new Random();
            var heartRate = rand.Next(55, 95);
            var heartRateDatapoint = new
            {
                deviceId = Config.DeviceId,
                heartRate = heartRate
            };
            var messageString = JsonConvert.SerializeObject(heartRateDatapoint);
            var message = new Message(Encoding.ASCII.GetBytes(messageString));
            await _deviceClient.SendEventAsync(message);
        }
    }
}
