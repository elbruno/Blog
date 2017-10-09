using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Storage.Streams;
using Windows.UI.Input;

namespace SurfaceDialApp01.Views
{
    public sealed partial class MainPage : INotifyPropertyChanged
    {
        readonly RadialController _controller;

        public MainPage()
        {
            InitializeComponent();

            _controller = RadialController.CreateForCurrentView();
            var icon = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/StoreLogo.png"));
            var myItem = RadialControllerMenuItem.CreateFromIcon("El Bruno Sample App", icon);
            _controller.Menu.Items.Add(myItem);
            _controller.ButtonClicked += ControllerButtonClicked;
            _controller.RotationChanged += ControllerRotationChanged;
        }

        private void ControllerRotationChanged(RadialController sender, RadialControllerRotationChangedEventArgs args)
        {
            if (RotationSlider.Value + args.RotationDeltaInDegrees > 100)
            {
                RotationSlider.Value = 100;
                return;
            }
            if (RotationSlider.Value + args.RotationDeltaInDegrees < 0)
            {
                RotationSlider.Value = 0;
                return;
            }
            RotationSlider.Value += args.RotationDeltaInDegrees;
        }

        private void ControllerButtonClicked(RadialController sender, RadialControllerButtonClickedEventArgs args)
        {
            ButtonToggle.IsOn = !ButtonToggle.IsOn;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
