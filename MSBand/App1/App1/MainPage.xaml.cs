using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using App1.Annotations;
using Microsoft.Band;
using Microsoft.Band.Sensors;
using Microsoft.Band.Tiles;
using Microsoft.Band.Tiles.Pages;
using static System.String;

namespace App1
{
    public sealed partial class MainPage : INotifyPropertyChanged
    {
        private string _accX;
        private string _accY;
        private string _accZ;
        private string _gyrX;
        private string _gyrY;
        private string _gyrZ;
        private string _pace;
        private string _currentMotion;
        private string _totalDistance;
        private string _statusMessage;
        private string _bandInformation;
        private string _bandData;

        private IBandClientManager _bandManager;
        private IBandInfo[] _pairedBands;
        private IBandClient _bandClient;
        private IBandInfo _bandInfo;
        private readonly Guid _myTileId;
        private int _buttonPressedCount;

        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
            DataContext = this;
            _myTileId = new Guid("E27830EA-AF6A-404F-A256-47EC79DE0768");
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
                await ConnectToBand();
                await GetBandInformation();
                await SubscribeToTileEvents();
                await SuscribeToSensorValueChanged();
            }
        }

        private async Task ConnectToBand()
        {
            _bandInfo = _pairedBands[0];
            _bandClient = await BandClientManager.Instance.ConnectAsync(_bandInfo);
        }

        private async Task SuscribeToSensorValueChanged()
        {
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

        private async Task SubscribeToTileEvents()
        {
            var closePressed = new TaskCompletionSource<bool>();
            _bandClient.TileManager.TileOpened += (s, args) => { StatusMessage = "Tile Opened"; };
            _bandClient.TileManager.TileButtonPressed += (s, args) =>
            {
                _buttonPressedCount++;
                StatusMessage = Format("Tile Button Pressed = {0}", _buttonPressedCount);
            };
            //_bandClient.TileManager.TileClosed += (s, args) => { closePressed.TrySetResult(true); };

            await _bandClient.TileManager.StartReadingsAsync();
        }

        private async Task GetBandInformation()
        {
            var bi = new BandInformation();
            _bandInformation = await bi.RetrieveInfo(_bandInfo, _bandClient);
            StatusMessage = $"Connected to {_bandInfo.Name}";
        }

        private void Distance_ReadingChanged(object sender, BandSensorReadingEventArgs<IBandDistanceReading> e)
        {
            _totalDistance = e.SensorReading.TotalDistance.ToString(CultureInfo.InvariantCulture);
            _currentMotion = e.SensorReading.CurrentMotion.ToString();
            _pace = e.SensorReading.Pace.ToString(CultureInfo.InvariantCulture);
        }

        private void Gyroscope_ReadingChanged(object sender, BandSensorReadingEventArgs<IBandGyroscopeReading> e)
        {
            _gyrX = e.SensorReading.AccelerationX.ToString(CultureInfo.InvariantCulture);
            _gyrY = e.SensorReading.AccelerationY.ToString(CultureInfo.InvariantCulture);
            _gyrZ = e.SensorReading.AccelerationZ.ToString(CultureInfo.InvariantCulture);
        }

        private void Accelerometer_ReadingChanged(object sender, BandSensorReadingEventArgs<IBandAccelerometerReading> e)
        {
            _accX = e.SensorReading.AccelerationX.ToString(CultureInfo.InvariantCulture);
            _accY = e.SensorReading.AccelerationY.ToString(CultureInfo.InvariantCulture);
            _accZ = e.SensorReading.AccelerationZ.ToString(CultureInfo.InvariantCulture);
            OnPropertyChanged(nameof(BandData));
        }

        private async void ButtonDisplayTileClick(object sender, RoutedEventArgs e)
        {
            var myTile = new BandTile(_myTileId)
            {
                Name = "El Bruno Tile",
                TileIcon = await LoadIcon("ms-appx:///Assets/RugbyBall46.png"),
                SmallIcon = await LoadIcon("ms-appx:///Assets/RugbyBall24.png")
            };
            var button = new TextButton() { ElementId = 1, Rect = new PageRect(10, 10, 200, 90) };
            var panel = new FilledPanel(button) { Rect = new PageRect(0, 0, 220, 150) };
            myTile.PageLayouts.Add(new PageLayout(panel));

            await _bandClient.TileManager.RemoveTileAsync(_myTileId);
            await _bandClient.TileManager.AddTileAsync(myTile);
            await _bandClient.TileManager.SetPagesAsync(_myTileId, 
                new PageData(new Guid("5F5FD06E-BD37-4B71-B36C-3ED9D721F200"), 0, new TextButtonData(1, "Click here")));
        }

        public string StatusMessage
        {
            get { return _statusMessage; }
            set
            {
                if (value == _statusMessage) return;
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public string BandData
        {
            get
            {
                CollectBandData();
                return _bandData;
            }
            set
            {
                if (value == _bandData) return;
                _bandData = value;
                OnPropertyChanged();
            }
        }

        private void CollectBandData()
        {
            _bandData = $@"{_bandInformation}
Gyroscope: {_gyrX},{_gyrY},{_gyrZ}
Accelerometer: {_accX},{_accY},{_accZ}
Distance: {_pace}, {_totalDistance}, {_currentMotion}";
        }

        private async Task<BandIcon> LoadIcon(string uri)
        {
            var imageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(uri));

            using (var fileStream = await imageFile.OpenAsync(FileAccessMode.Read))
            {
                var bitmap = new WriteableBitmap(1, 1);
                await bitmap.SetSourceAsync(fileStream);
                return bitmap.ToBandIcon();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            });
        }

        private async void ButtonRemoveTile_Click(object sender, RoutedEventArgs e)
        {
            if (_bandClient == null)
                await ConnectToBand();
            if (_bandClient != null)
                await _bandClient.TileManager.RemoveTileAsync(_myTileId);
        }

        private async void ButtonListTiles_Click(object sender, RoutedEventArgs e)
        {
            await DisplayTilesInformation();
        }

        private async Task DisplayTilesInformation()
        {
            var tilesInfo = Empty;
            var tiles = await _bandClient.TileManager.GetTilesAsync();
            tilesInfo = tiles.Aggregate(
                tilesInfo, (current, bandTile) => 
                current + Format("{0} - {1}\n", bandTile.Name, bandTile.TileId));
            var md = new MessageDialog(tilesInfo);
            await md.ShowAsync();
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
            return Format(" Connected to: {0}" +
                                 " \n Connection type : {1}" +
                                 " \n Firmware : {2} \n Hardware : {3}",
                    Name, ConnectionType, Firmware, Hardware);
        }
    }
}

