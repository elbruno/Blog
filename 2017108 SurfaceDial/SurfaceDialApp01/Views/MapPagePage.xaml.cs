using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using SurfaceDialApp01.Helpers;
using SurfaceDialApp01.Services;

using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Navigation;

namespace SurfaceDialApp01.Views
{
    public sealed partial class MapPagePage : INotifyPropertyChanged
    {
        // TODO WTS: Set your preferred default zoom level
        private const double DefaultZoomLevel = 17;

        private readonly LocationService _locationService;

        // TODO WTS: Set your preferred default location if a geolock can't be found.
        private readonly BasicGeoposition _defaultPosition = new BasicGeoposition()
        {
            Latitude = 47.609425,
            Longitude = -122.3417
        };

        private double _zoomLevel;

        public double ZoomLevel
        {
            get => _zoomLevel;
            set => Set(ref _zoomLevel, value);
        }

        private Geopoint _center;

        public Geopoint Center
        {
            get => _center;
            set => Set(ref _center, value);
        }

        enum ControllerMode { Zoom, Rotate, Disable };
        ControllerMode _controllerMode;

        public MapPagePage()
        {
            _locationService = new LocationService();
            Center = new Geopoint(_defaultPosition);
            ZoomLevel = DefaultZoomLevel;
            InitializeComponent();

            var controller = RadialController.CreateForCurrentView();
            controller.RotationResolutionInDegrees = 0.2;
            controller.UseAutomaticHapticFeedback = false;

            var mapZoomItem = RadialControllerMenuItem.CreateFromFontGlyph("El Bruno - Maps Zoom", "\xE128", "Segoe MDL2 Assets");
            var mapRotationItem = RadialControllerMenuItem.CreateFromFontGlyph("El Bruno - Map Rotation", "\xE128", "Segoe MDL2 Assets");
            var disableDialItem = RadialControllerMenuItem.CreateFromFontGlyph("El Bruno - Disable Dial", "\xE128", "Segoe MDL2 Assets");
            controller.Menu.Items.Add(mapZoomItem);
            controller.Menu.Items.Add(mapRotationItem);
            controller.Menu.Items.Add(disableDialItem);

            var surfaceDialConfiguration = RadialControllerConfiguration.GetForCurrentView();
            surfaceDialConfiguration.SetDefaultMenuItems(new List<RadialControllerSystemMenuItemKind>{});

            // add 2 default system buttons
            //surfaceDialConfiguration.SetDefaultMenuItems(new[] {
            //    RadialControllerSystemMenuItemKind.Volume,
            //    RadialControllerSystemMenuItemKind.NextPreviousTrack
            //});

            mapZoomItem.Invoked += MapZoomItem_Invoked;
            mapRotationItem.Invoked += MapRotationItem_Invoked;
            disableDialItem.Invoked += DisableDialItem_Invoked;  

            controller.RotationChanged += ControllerRotationChangedAsync;
        }

        private void DisableDialItem_Invoked(RadialControllerMenuItem sender, object args)
        {
            _controllerMode = ControllerMode.Disable;
        }

        private void MapRotationItem_Invoked(RadialControllerMenuItem sender, object args)
        {
            _controllerMode = ControllerMode.Rotate;
        }

        private void MapZoomItem_Invoked(RadialControllerMenuItem sender, object args)
        {
            _controllerMode = ControllerMode.Zoom;
        }

        private async void ControllerRotationChangedAsync(RadialController sender, RadialControllerRotationChangedEventArgs args)
        {
            Debug.WriteLine($"{args.RotationDeltaInDegrees}");
            switch (_controllerMode)
            {
                case ControllerMode.Zoom:
                    mapControl.ZoomLevel = mapControl.ZoomLevel + args.RotationDeltaInDegrees;
                    break;
                case ControllerMode.Rotate:
                    await mapControl.TryRotateAsync(args.RotationDeltaInDegrees);
                    break;
                default:
                    Debug.WriteLine($"No action!");
                    break;
            }
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await InitializeAsync();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Cleanup();
        }

        public async Task InitializeAsync()
        {
            if (_locationService != null)
            {
                _locationService.PositionChanged += LocationService_PositionChanged;

                var initializationSuccessful = await _locationService.InitializeAsync();

                if (initializationSuccessful)
                {
                    await _locationService.StartListeningAsync();
                }

                if (initializationSuccessful && _locationService.CurrentPosition != null)
                {
                    Center = _locationService.CurrentPosition.Coordinate.Point;
                }
                else
                {
                    Center = new Geopoint(_defaultPosition);
                }
            }

            if (mapControl != null)
            {
                // TODO WTS: Set your map service token. If you don't have one, request at https://www.bingmapsportal.com/
                mapControl.MapServiceToken = string.Empty;

                AddMapIcon(Center, "Map_YourLocation".GetLocalized());
            }
        }

        public void Cleanup()
        {
            if (_locationService != null)
            {
                _locationService.PositionChanged -= LocationService_PositionChanged;
                _locationService.StopListening();
            }
        }

        private void LocationService_PositionChanged(object sender, Geoposition geoposition)
        {
            if (geoposition != null)
            {
                Center = geoposition.Coordinate.Point;
            }
        }

        private void AddMapIcon(Geopoint position, string title)
        {
            var mapIcon = new MapIcon()
            {
                Location = position,
                NormalizedAnchorPoint = new Point(0.5, 1.0),
                Title = title,
                Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/map.png")),
                ZIndex = 0
            };
            mapControl.MapElements.Add(mapIcon);
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
