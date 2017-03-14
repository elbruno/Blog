using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using App1.Annotations;
using Microsoft.Band.Notifications;
using Microsoft.Band.Tiles;
using static System.String;

namespace App1
{
    public sealed partial class MainPage : INotifyPropertyChanged
    {
        private string _statusMessage;
        private string _bandData;

        private int _buttonPressedCount;
        private string _bandUserActions;
        private BandTileComplex _bandTileComplex;
        private DispatcherTimer _timerSetData;

        public MainPage()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            _timerSetData = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};
            _timerSetData.Tick += TimerSetDataTick;
        }

        private async void ButtonConnectClick(object sender, RoutedEventArgs e)
        {
            var result = await BandHub.Instance.LoadBands();
            if (!result) return;
            BandData = BandHub.Instance.BandInformation;
            _bandTileComplex = new BandTileComplex(BandHub.Instance.BandClient);
            await SubscribeToTileEvents();
        }

        private async Task SubscribeToTileEvents()
        {
            var closePressed = new TaskCompletionSource<bool>();
            BandHub.Instance.BandClient.TileManager.TileOpened += TileManager_TileOpened;
            BandHub.Instance.BandClient.TileManager.TileButtonPressed += TileManager_TileButtonPressed;
            BandHub.Instance.BandClient.TileManager.TileClosed += 
                (s, args) => { closePressed.TrySetResult(true); };
            await BandHub.Instance.BandClient.TileManager.StartReadingsAsync();
        }

        private void TileManager_TileOpened(object sender, 
            BandTileEventArgs<IBandTileOpenedEvent> e)
        {
            StatusMessage = "Tile Opened";
        }

        private void TileManager_TileButtonPressed(object sender, 
            BandTileEventArgs<IBandTileButtonPressedEvent> e)
        {
            var elementId = (short)e.TileEvent.ElementId;
            var tileId = e.TileEvent.TileId;
            var message = Empty;
            if (_bandTileComplex.TileId == tileId)
            {
                switch (elementId)
                {
                    case BandTileComplex.ButtonAId:
                        message = "Click on button A";
                        break;
                    case BandTileComplex.ButtonBId:
                        message = "Click on button B";
                        break;
                    default:
                        message = "Click on other element";
                        break;
                }
            }
            BandUserActions = message;
            _buttonPressedCount++;
            StatusMessage = Format("Tile Button Pressed = {0}", _buttonPressedCount);
        }

        private async void ButtonAddSimpleTileClick(object sender, RoutedEventArgs e)
        {
            var bandTileSimple = new BandTileSimple();
            await bandTileSimple.AddTile();
        }

        private async void ButtonAddComplexTile_Click(object sender, RoutedEventArgs e)
        {
            await _bandTileComplex.AddTile();
            await _bandTileComplex.SetTileData("label", "Btn A", "Btn B", "12345");
        }

        private void ButtonSetInfo_Click(object sender, RoutedEventArgs e)
        {
            if (_timerSetData.IsEnabled)
                _timerSetData.Stop();
            else
                _timerSetData.Start();
        }

        private async void TimerSetDataTick(object sender, object e)
        {
            var rnd = new Random();
            string sampleCard = $"Card {rnd.Next(10)}";
            string btnA = $"Btn {rnd.Next(10)}";
            string btnB = $"Btn {rnd.Next(10)}";
            var barcode = rnd.Next(10000).ToString();
            await _bandTileComplex.SetTileData(sampleCard, btnA, btnB, barcode);

            BandData = $@"Card    : {sampleCard}
Button A: {btnA}
Button B: {btnB}
Barcode : {barcode}";
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
                return _bandData;
            }
            set
            {
                if (value == _bandData) return;
                _bandData = value;
                OnPropertyChanged();
            }
        }

        public string BandUserActions
        {
            get { return _bandUserActions; }
            set
            {
                if (value == _bandUserActions) return;
                _bandUserActions = value;
                OnPropertyChanged();
            }
        }

        #region OnPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            });
        }

        #endregion

        private void ButtonRemoveAllTilesClick(object sender, RoutedEventArgs e)
        {
            BandHub.Instance.RemoveAllCustomTiles();
        }

        private async void ButtonListTiles_Click(object sender, RoutedEventArgs e)
        {
            var tileCapacity = await BandHub.Instance.BandClient.TileManager.GetRemainingTileCapacityAsync();
            var tilesInfo = $"Tile Capacity: {tileCapacity.ToString()}\n";
            var tiles = await BandHub.Instance.BandClient.TileManager.GetTilesAsync();
            tilesInfo = tiles.Aggregate(
                tilesInfo, (current, bandTile) =>
                current + Format("{0} - {1}\n", bandTile.Name, bandTile.TileId));
            var md = new MessageDialog(tilesInfo);
            await md.ShowAsync();
        }

        private void ButtonSendMessageClick(object sender, RoutedEventArgs e)
        {
            var title = "Message Title";
            var body = "Message body";
            BandHub.Instance.BandClient.NotificationManager.SendMessageAsync(
                _bandTileComplex.TileId, title, body, 
                DateTimeOffset.Now, MessageFlags.ShowDialog);
        }

        private void ButtonShowDialog_Click(object sender, RoutedEventArgs e)
        {
            var title = "Dialog Title";
            var body = "Dialog body";
            BandHub.Instance.BandClient.NotificationManager.ShowDialogAsync(
                _bandTileComplex.TileId, title, body);
        }
    }
}

