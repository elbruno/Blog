using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Band;
using Microsoft.Band.Notifications;
using Microsoft.Band.Tiles;

namespace App1
{
    public sealed partial class MainPage : Page
    {
        private string _accX;
        private string _accY;
        private string _accZ;
        private string _gyrX;
        private string _gyrY;
        private string _gyrZ;
        private string _brightness;
        private string _pace;
        private string _currentMotion;
        private string _totalDistance;

        private IBandClientManager _bandManager;
        private IBandInfo[] _pairedBands;
        private IBandClient _bandClient;
        private IBandInfo _bandInfo;

        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            _bandManager = BandClientManager.Instance;
            _pairedBands = await _bandManager.GetBandsAsync();
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            if (_pairedBands.Length == 0)
            {
                var md = new MessageDialog("no band connected");
                await md.ShowAsync();
            }
            else
            {
                _bandInfo = _pairedBands[0];
                _bandClient = await BandClientManager.Instance.ConnectAsync(_bandInfo);
                var bi = new BandInformation();
                textBlockBandInformation.Text = await bi.RetrieveInfo(_bandInfo, _bandClient);

                if (_bandClient.SensorManager.Accelerometer.GetCurrentUserConsent() != UserConsent.Granted)
                    await _bandClient.SensorManager.Accelerometer.RequestUserConsentAsync();
                _bandClient.SensorManager.Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
                await _bandClient.SensorManager.Accelerometer.StartReadingsAsync();

                if (_bandClient.SensorManager.Gyroscope.GetCurrentUserConsent() != UserConsent.Granted)
                    await _bandClient.SensorManager.Gyroscope.RequestUserConsentAsync();
                _bandClient.SensorManager.Gyroscope.ReadingChanged += Gyroscope_ReadingChanged;
                await _bandClient.SensorManager.Gyroscope.StartReadingsAsync();

                if (_bandClient.SensorManager.Distance.GetCurrentUserConsent() != UserConsent.Granted)
                    await _bandClient.SensorManager.Distance.RequestUserConsentAsync();
                _bandClient.SensorManager.Distance.ReadingChanged += Distance_ReadingChanged;
                await _bandClient.SensorManager.Distance.StartReadingsAsync();
            }
        }

        private void Distance_ReadingChanged(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandDistanceReading> e)
        {
            _totalDistance = e.SensorReading.TotalDistance.ToString(CultureInfo.InvariantCulture);
            _currentMotion = e.SensorReading.CurrentMotion.ToString();
            _pace = e.SensorReading.Pace.ToString(CultureInfo.InvariantCulture);
        }

        private void AmbientLight_ReadingChanged(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandAmbientLightReading> e)
        {
            _brightness = e.SensorReading.Brightness.ToString(CultureInfo.InvariantCulture);
        }

        private void Gyroscope_ReadingChanged(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandGyroscopeReading> e)
        {
            _gyrX = e.SensorReading.AccelerationX.ToString(CultureInfo.InvariantCulture);
            _gyrY = e.SensorReading.AccelerationY.ToString(CultureInfo.InvariantCulture);
            _gyrZ = e.SensorReading.AccelerationZ.ToString(CultureInfo.InvariantCulture);

            DisplayInformation();
        }

        private void Accelerometer_ReadingChanged(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandAccelerometerReading> e)
        {
            _accX = e.SensorReading.AccelerationX.ToString(CultureInfo.InvariantCulture);
            _accY = e.SensorReading.AccelerationY.ToString(CultureInfo.InvariantCulture);
            _accZ = e.SensorReading.AccelerationZ.ToString(CultureInfo.InvariantCulture);

            DisplayInformation();
        }

        private async void buttonSendWarning_Click(object sender, RoutedEventArgs e)
        {

            //Guid myTileId = new Guid();
            Guid myTileId = new Guid("AC5DBA9F-12FD-47A5-83A9-E7270A43BB9E");
            await _bandClient.NotificationManager.SendMessageAsync(myTileId, "Titulo", "Cuerpo", DateTimeOffset.Now, MessageFlags.ShowDialog);
        }

        private void DisplayInformation()
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                textBlockBandSensors.Text = $@"Gyroscope: {_gyrX},{_gyrY},{_gyrZ}
Accelerometer: {_accX},{_accY},{_accZ}
Ambient light: {_brightness}
Distance: {_pace}, {_totalDistance}, {_currentMotion}";
            });
        }
    }

    public class BandInformation
    {
        public string Name { get; private set; }
        public string Firmware { get; private set; }
        public string Hardware { get; private set; }
        public BandConnectionType ConnectionType { get; private set; }

        public async Task<string> RetrieveInfo(IBandInfo bandInfo, IBandClient client)
        {
            Name = bandInfo.Name;
            ConnectionType = bandInfo.ConnectionType;
            Firmware = await client.GetFirmwareVersionAsync();
            Hardware = await client.GetHardwareVersionAsync();
            return string.Format(" Connected to: {0}" +
                                 " \n Connection type : {1}" +
                                 " \n Firmware : {2} \n Hardware : {3}",
                    Name, ConnectionType, Firmware, Hardware);
        }
    }
}

