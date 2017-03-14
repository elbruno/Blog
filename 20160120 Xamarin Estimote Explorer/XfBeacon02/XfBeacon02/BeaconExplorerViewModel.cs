using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Estimotes;
using Xamarin.Forms;
using XfBeacon02.Annotations;
using XfBeacon02.Model;

namespace XfBeacon02
{
    public class BeaconExplorerViewModel : INotifyPropertyChanged
    {
        private BeaconInitStatus _status;
        private ObservableCollection<ImageCellBeacon> _beacons;
        private DateTime _beaconsFound;
        private bool _timerStarted;
        private string _logStatus;

        public async void OnAppearing()
        {
            try
            {
                _status = await EstimoteManager.Instance.Initialize();
                LogStatus = _status != BeaconInitStatus.Success ? @"could not initialice beacons" : @"beacons initialized";
                if (_status != BeaconInitStatus.Success) return;
                EstimoteManager.Instance.Ranged += Instance_Ranged;
                EstimoteManager.Instance.StartRanging(new BeaconRegion(EstimoteConfig.EstimoteName, EstimoteConfig.EstimoteUuidVirtual));
                Beacons = new ObservableCollection<ImageCellBeacon>();
            }
            catch (Exception exception)
            {
                LogStatus = exception.Message;
            }
        }

        private void Instance_Ranged(object sender, IEnumerable<IBeacon> e)
        {
            try
            {
                Beacons.Clear();
                foreach (var beacon in e)
                {
                    var imageCellBeacon = new ImageCellBeacon
                    {
                        Text = beacon.Proximity.ToString(),
                        Detail = $"{beacon.Major}.{beacon.Minor}",
                        DetailColor =
                            beacon.Uuid == EstimoteConfig.EstimoteUuidVirtual ? "Blue" : "Purple",
                        ImageSource =
                            beacon.Uuid == EstimoteConfig.EstimoteUuidVirtual ? "EstimoteBlue.png"
                                : "EstimotePurple.png",
                    };
                    Beacons.Add(imageCellBeacon);
                }
                _beaconsFound = DateTime.Now;
                DisplayCurrentStatus();
                if (!_timerStarted)
                {
                    Device.StartTimer(TimeSpan.FromSeconds(5), TimerElapsed);
                    _timerStarted = true;
                }

                Debug.WriteLine("Beacons : " + Beacons.Count);

            }
            catch (Exception exception)
            {
                LogStatus = exception.Message;
            }
        }

        private void DisplayCurrentStatus()
        {
            LogStatus = $"{_beacons.Count} beacons found at {_beaconsFound.TimeOfDay}";
        }

        private bool TimerElapsed()
        {
            var dif = DateTime.Now.Subtract(_beaconsFound);
            Debug.WriteLine("Dif " + dif.Seconds);
            if (dif.Seconds <= 5) return true;
            Beacons.Clear();
            DisplayCurrentStatus();
            return true;
        }

        #region Props

        public string LogStatus
        {
            get { return _logStatus; }
            set
            {
                if (value == _logStatus) return;
                _logStatus = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<ImageCellBeacon> Beacons
        {
            get { return _beacons; }
            set
            {
                if (Equals(value, _beacons)) return;
                _beacons = value;
                OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
