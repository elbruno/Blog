using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using ElBruno.PolarH7.Annotations;
using ElBruno.PolarH7.Model;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Imaging;

namespace ElBruno.PolarH7
{
    public sealed partial class MainPage : INotifyPropertyChanged
    {
        private const int HrCollectionCount = 15;
        private DeviceInformation _devicePolarHr;
        private HrModel _lastBpm;
        DispatcherTimer _timerProcess = new DispatcherTimer();
        private bool _fakeMode;
        private bool _displayDebug;

        public MainPage()
        {
            InitializeComponent();
            DataContext = this;
            BpmCollection = new FixedSizeQueue<HrModel>(HrCollectionCount);
           BpmValue = 70;
            StressValue = 7;
            BpmValue = 60;
            Loaded += MainPage_Loaded;
            FakeModeDefineVisualElements();
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            _timerProcess = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timerProcess.Tick += TimerProcessTick;
            _timerProcess.Start();

            InitChartsLayout();
        }

        private void InitChartsLayout()
        {
        }

        private void TimerProcessTick(object sender, object e)
        {
            TimeInformation = $"{DateTime.Now:T}";
            if (_fakeMode)
            {
                SetBpmValue(BpmValue);
                ButtonFakeHrDown.Visibility = Visibility.Visible;
                ButtonFakeHrUp.Visibility = Visibility.Visible;
            }
            else
            {
                ButtonFakeHrDown.Visibility = Visibility.Collapsed;
                ButtonFakeHrUp.Visibility = Visibility.Collapsed;
            }
            UpdateBpmCollection();
            DisplayDeviceConnectionStatus();
            FindPolarHrDevices();

            if (_readHrValueFlag || _devicePolarHr == null) return;

            SuscribeToReadBpmValues();
        }

        private void DisplayDeviceConnectionStatus()
        {
            if (_devicePolarHr == null)
            {
                ImageConnected.Visibility = Visibility.Collapsed;
                ImageDisconnected.Visibility = Visibility.Visible;
            }
            else
            {
                ImageConnected.Visibility = Visibility.Visible;
                ImageDisconnected.Visibility = Visibility.Collapsed;
            }
        }

        private async void FindPolarHrDevices()
        {
            if (_lastBpm != null)
            {
                var dif = DateTime.Now - _lastBpm.Time;
                if (dif.Seconds > 3)
                    SuscribeToReadBpmValues();
            }
            if (_devicePolarHr != null) return;
            StatusInformation = $"Searching ...";
            var devices =
                await
                    DeviceInformation.FindAllAsync(
                        GattDeviceService.GetDeviceSelectorFromUuid(GattServiceUuids.HeartRate));
            if (null == devices || devices.Count <= 0) return;
            foreach (var device in devices.Where(device => device.Name.Contains("Polar H7")))
            {
                _devicePolarHr = device;
                StatusInformation = $"Polar HR [{_devicePolarHr.Name}], connect to Heart service ...";
                Debug.WriteLine(_devicePolarHr.Id);
                break;
            }
        }

        private async void SuscribeToReadBpmValues()
        {
            try
            {
                var service = await GattDeviceService.FromIdAsync(_devicePolarHr.Id);
                var characteristics = service?.GetAllCharacteristics();
                if (characteristics == null || characteristics.Count <= 0)
                {
                    Debug.WriteLine($"char null for {_devicePolarHr.Id}");
                    var charNull = characteristics == null;
                    StatusInformation = $"Polar HR [{_devicePolarHr.Name}], service is null = {charNull}";
                }
                else
                {
                    foreach (var characteristic in characteristics)
                    {
                        characteristic.ValueChanged += GattCharacteristic_ValueChanged;
                        await
                            characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                                GattClientCharacteristicConfigurationDescriptorValue.Notify);
                    }
                }
            }
            catch (Exception exception)
            {
                _devicePolarHr = null;
                _readHrValueFlag = false;
                Debug.Write(exception.ToString());
            }
        }

        private void GattCharacteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            if (args == null) return;
            if (args.CharacteristicValue.Length == 0) return;
            _readHrValueFlag = true;
            var arrayLenght = (int)args.CharacteristicValue.Length;
            var hrData = new byte[arrayLenght];
            DataReader.FromBuffer(args.CharacteristicValue).ReadBytes(hrData);

            //Convert to string  
            var bpmValue = ProcessData(hrData);
            Debug.WriteLine(bpmValue);
            SetBpmValue(bpmValue);
        }

        private async void SetBpmValue(int bpmValue)
        {
            try
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    BpmValue = bpmValue;

                    var currentBpm = new HrModel
                    {
                        HeartRate = bpmValue,
                        PreviousMeasureHeartRate = bpmValue
                    };
                    if (_lastBpm != null)
                    {
                        currentBpm.PreviousMeasureHeartRate = _lastBpm.HeartRate;
                        currentBpm.PreviousMeasureTime = _lastBpm.Time;
                    }
                    BpmCollection.Enqueue(currentBpm);
                    _lastBpm = currentBpm;
                });
            }
            catch (Exception exception)
            {
                var md = new MessageDialog(exception.ToString());
                await md.ShowAsync();
            }
        }

        private void UpdateBpmCollection()
        {
            var i = 0;
            foreach (var source in BpmCollection.ToList())
            {
                i++;
                source.TimeInt = i;
            }
        }

        private int ProcessData(byte[] data)
        {
            // Heart Rate profile defined flag values
            const byte heartRateValueFormat = 0x01;

            byte currentOffset = 0;
            byte flags = data[currentOffset];
            bool isHeartRateValueSizeLong = ((flags & heartRateValueFormat) != 0);

            currentOffset++;

            ushort heartRateMeasurementValue;

            if (isHeartRateValueSizeLong)
            {
                heartRateMeasurementValue = (ushort)((data[currentOffset + 1] << 8) + data[currentOffset]);
                currentOffset += 2;
            }
            else
            {
                heartRateMeasurementValue = data[currentOffset];
            }

            return heartRateMeasurementValue;
        }

        #region Properties and Property Changed

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private async void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            });
        }

        private string _statusInformation;
        public string StatusInformation
        {
            get => _statusInformation;
            set
            {
                if (value == _statusInformation) return;
                _statusInformation = value;
                OnPropertyChanged();
            }
        }

        private string _timeInformation;
        public string TimeInformation
        {
            get => _timeInformation;
            set
            {
                if (value == _timeInformation) return;
                _timeInformation = value;
                OnPropertyChanged();
            }
        }

        private string _stressProcessingInformation;
        public string StressProcessingInformation
        {
            get => _stressProcessingInformation;
            set
            {
                if (value == _stressProcessingInformation) return;
                _stressProcessingInformation = value;
                OnPropertyChanged();
            }
        }

        private int _bpmValue;

        public int BpmValue
        {
            get => _bpmValue;
            set
            {
                if (value == _bpmValue) return;
                _bpmValue = value;
                OnPropertyChanged();
            }
        }

        private int _stressValue;

        public int StressValue
        {
            get => _stressValue;
            set
            {
                if (value == _stressValue) return;
                _stressValue = value;
                OnPropertyChanged();
            }
        }
        private string _stressValueLevel;

        public string StressValueLevel
        {
            get => _stressValueLevel;
            set
            {
                if (value == _stressValueLevel) return;
                _stressValueLevel = value;
                OnPropertyChanged();
            }
        }

        private FixedSizeQueue<HrModel> _bpmCollection;

        public FixedSizeQueue<HrModel> BpmCollection
        {
            get => _bpmCollection;
            set
            {
                if (value == _bpmCollection) return;
                _bpmCollection = value;
                OnPropertyChanged();
            }
        }

        private bool _readHrValueFlag;
        private object _syncObj;

        #endregion

        private void ButtonFakeHrUp_Click(object sender, RoutedEventArgs e)
        {
            _fakeMode = true;
            BpmValue++;
        }

        private void ButtonFakeHrDown_Click(object sender, RoutedEventArgs e)
        {
            _fakeMode = true;
            BpmValue--;
        }

        private void FakeModeDefineVisualElements()
        {
            StackPanelStatus.Visibility = !_displayDebug ? Visibility.Collapsed : Visibility.Visible;
        }

        private void ButtonViewMore_OnClick(object sender, RoutedEventArgs e)
        {
            _displayDebug = !_displayDebug;
            FakeModeDefineVisualElements();
        }

        private void ButtonOltivaStressMonitor_OnClick(object sender, RoutedEventArgs e)
        {
            _fakeMode = !_fakeMode;
        }

        private async void ButtonTopRight_OnClick(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            Helpers.FillDecoderExtensions(picker.FileTypeFilter);

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                await LoadFileAsync(file);
            }
        }

        private async Task LoadFileAsync(StorageFile file)
        {
            try
            {
                var src = new BitmapImage();
                using (var stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    await src.SetSourceAsync(stream);
                }

                ImageTopRight.Source = src;
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.ToString());
            }
        }
    }
}
