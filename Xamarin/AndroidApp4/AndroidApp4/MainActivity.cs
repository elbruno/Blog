using System;
using Android.App;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.OS;
using Android.Widget;
using Robotics.Mobile.Core.Bluetooth.LE;
using Adapter = Robotics.Mobile.Core.Bluetooth.LE.Adapter;

namespace AndroidApp4
{
    [Activity(Label = "Search BLE devices", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private Button _buttonScanBle;
        private BluetoothManager _manager;
        private BluetoothAdapter _adapter;
        private BluetoothLeScanner _bleScanner;
        private Adapter _bleAdapter;
        private EditText _textboxResults;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            _buttonScanBle = FindViewById<Button>(Resource.Id.ButtonSearchBle);
            _textboxResults = FindViewById<EditText>(Resource.Id.TextBoxResults);
            _buttonScanBle.Click += ButtonScanBleClick;

            var appContext = Application.Context;
            _manager = (BluetoothManager)appContext.GetSystemService(BluetoothService); // ("bluetooth");
            _adapter = _manager.Adapter;

            _bleAdapter = new Adapter();
            _bleAdapter.DeviceDiscovered += _bleAdapter_DeviceDiscovered;
            _bleAdapter.ScanTimeoutElapsed += _bleAdapter_ScanTimeoutElapsed;
        }

        private void _bleAdapter_ScanTimeoutElapsed(object sender, EventArgs e)
        {
            _bleAdapter.StopScanningForDevices(); 
            DisplayInformation("Bluetooth scan timeout elapsed, no heart rate monitors were found");
        }

        private void _bleAdapter_DeviceDiscovered(object sender, DeviceDiscoveredEventArgs e)
        {
            var msg = string.Format("Device found: {0}", e.Device.Name);
            DisplayInformation(msg);
        }

        private void ButtonScanBleClick(object sender, EventArgs e)
        {
            if (!_bleAdapter.IsScanning)
                _bleAdapter.StartScanningForDevices();
        }

        private void DisplayInformation(string line)
        {
            _textboxResults.Text = $"{line}\r\n{_textboxResults.Text}";
            Console.WriteLine(line);
        }
    }
}

