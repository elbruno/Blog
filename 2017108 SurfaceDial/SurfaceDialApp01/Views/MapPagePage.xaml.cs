using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using SurfaceDialApp01.Helpers;
using SurfaceDialApp01.Services;

using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Navigation;

namespace SurfaceDialApp01.Views
{
    public sealed partial class MapPagePage : Page, INotifyPropertyChanged
    {
        // TODO WTS: Set your preferred default zoom level
        private const double DefaultZoomLevel = 17;

        private readonly LocationService locationService;

        // TODO WTS: Set your preferred default location if a geolock can't be found.
        private readonly BasicGeoposition defaultPosition = new BasicGeoposition()
        {
            Latitude = 47.609425,
            Longitude = -122.3417
        };

        private double _zoomLevel;

        public double ZoomLevel
        {
            get { return _zoomLevel; }
            set { Set(ref _zoomLevel, value); }
        }

        private Geopoint _center;

        public Geopoint Center
        {
            get { return _center; }
            set { Set(ref _center, value); }
        }

        public MapPagePage()
        {
            locationService = new LocationService();
            Center = new Geopoint(defaultPosition);
            ZoomLevel = DefaultZoomLevel;
            InitializeComponent();
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
            if (locationService != null)
            {
                locationService.PositionChanged += LocationService_PositionChanged;

                var initializationSuccessful = await locationService.InitializeAsync();

                if (initializationSuccessful)
                {
                    await locationService.StartListeningAsync();
                }

                if (initializationSuccessful && locationService.CurrentPosition != null)
                {
                    Center = locationService.CurrentPosition.Coordinate.Point;
                }
                else
                {
                    Center = new Geopoint(defaultPosition);
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
            if (locationService != null)
            {
                locationService.PositionChanged -= LocationService_PositionChanged;
                locationService.StopListening();
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
