using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Media.Capture;
using Windows.Media.Core;
using Windows.Media.FaceAnalysis;
using Windows.Media.MediaProperties;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using WuaFaceAppChristmas.Annotations;
using WuaFaceAppChristmas.Lib;
using Panel = Windows.Devices.Enumeration.Panel;

namespace WuaFaceAppChristmas
{
    public sealed partial class MainPage : INotifyPropertyChanged
    {
        private MediaCapture _mediaCapture;
        private IMediaEncodingProperties _previewProperties;
        private string _messages;
        private bool _displayFaceFrames;
        private List<Image> _imageHats = new List<Image>();
        private bool     _displayFaceHat;

        public MainPage()
        {
            InitializeComponent();
            DataContext = this;
            DisplayFacesCommand = new Command(ExecuteDisplayFaces);
            Loaded += MainPage_Loaded;
            Unloaded += MainPage_Unloaded;
        }

        private async void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            _previewProperties = null;
            await _mediaCapture.StopPreviewAsync();
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            await InitMediaDevice();
            ImageHatInit();
        }

        private void ImageHatInit()
        {
            for (int i = 0; i < 6; i++)
            {
                var imageHat = new Image
                {
                    Width = 64,
                    Height = 64,
                    Source = new BitmapImage(new Uri(@"ms-appx:///Assets/santa-hat.png", UriKind.RelativeOrAbsolute)),
                    Visibility = Visibility.Collapsed
                };
                _imageHats.Add(imageHat);
            }
        }

        private void ImageHatHideAll()
        {
            foreach (var imageHat in _imageHats)
            {
                imageHat.Visibility = Visibility.Collapsed;
            }
        }

        private void ExecuteDisplayFaces()
        {
            if (AppBarToggleButtonFrame.IsChecked != null)
                _displayFaceFrames = AppBarToggleButtonFrame.IsChecked.Value;
            if (AppBarToggleButtonHat.IsChecked != null)
                _displayFaceHat = AppBarToggleButtonHat.IsChecked.Value;

            AppBarToggleButtonFrame.Icon = _displayFaceFrames ? new SymbolIcon(Symbol.Contact2) : new SymbolIcon(Symbol.Contact);
        }

        private async Task InitMediaDevice()
        {
            try
            {
                if (!FaceDetector.IsSupported) return;

                var cameraDevice = await CameraActions.FindCameraDeviceByPanelAsync(Panel.Front);
                var settings = new MediaCaptureInitializationSettings { VideoDeviceId = cameraDevice.Id };
                _mediaCapture = new MediaCapture();
                await _mediaCapture.InitializeAsync(settings);

                var definition = new FaceDetectionEffectDefinition
                {
                    SynchronousDetectionEnabled = false,
                    DetectionMode = FaceDetectionMode.HighPerformance
                };

                var faceDetectionEffect = (FaceDetectionEffect)await _mediaCapture.AddVideoEffectAsync(definition, MediaStreamType.VideoPreview);

                faceDetectionEffect.DesiredDetectionInterval = TimeSpan.FromMilliseconds(33);
                faceDetectionEffect.Enabled = true;
                faceDetectionEffect.FaceDetected += FaceDetectionEffect_FaceDetected;

                CameraViewer.Source = _mediaCapture;
                await _mediaCapture.StartPreviewAsync();
                _previewProperties = _mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview);

            }
            catch (Exception exception)
            {
                Messages = exception.ToString();
            }
        }

        private async void FaceDetectionEffect_FaceDetected(FaceDetectionEffect sender, FaceDetectedEventArgs args)
        {
            if (!_displayFaceFrames && !_displayFaceHat) return;
            // Use the dispatcher because this method is sometimes called from non-UI threads
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                RenderFaceRectangles(args.ResultFrame.DetectedFaces.ToList());
            });
        }

        private void RenderFaceRectangles(IList<DetectedFace> faces)
        {
            FaceRectanglesCanvas.Children.Clear();
            ImageHatHideAll();

            for (var i = 0; i < faces.Count; i++)
            {
                if (_displayFaceFrames)
                {
                    var faceBoundingBox = FaceUi.ConvertPreviewToUiRectangle(faces[i].FaceBox, CameraViewer,_previewProperties);
                    faceBoundingBox.Stroke = (i == 0 ? new SolidColorBrush(Colors.HotPink) : new SolidColorBrush(Colors.DeepSkyBlue));
                    FaceRectanglesCanvas.Children.Add(faceBoundingBox);
                }

                if (!_displayFaceHat) continue;
                var imageHat = _imageHats[i];
                imageHat = FaceUi.ConvertPreviewToUiHatImage(faces[i].FaceBox, CameraViewer, _previewProperties, imageHat);
                FaceRectanglesCanvas.Children.Add(imageHat);
            }
        }

        #region INotifyPropertyChanged
        public ICommand DisplayFacesCommand { get; }

        public string Messages
        {
            get { return _messages; }
            set
            {
                if (value == _messages) return;
                _messages = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
