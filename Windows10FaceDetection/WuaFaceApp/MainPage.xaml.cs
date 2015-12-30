using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.Capture;
using Windows.Media.Core;
using Windows.Media.FaceAnalysis;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using WuaFaceApp.Annotations;
using WuaFaceApp.Lib;

namespace WuaFaceApp
{
    public sealed partial class MainPage : INotifyPropertyChanged
    {
        private MediaCapture _mediaCapture;
        private IMediaEncodingProperties _previewProperties;
        private string _messages;
        private bool _displayFaces;

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
        }

        private void ExecuteDisplayFaces()
        {
            _displayFaces = !_displayFaces;
            if (_displayFaces)
            {
                AppBarButtonDisplayFacesEnabled.Visibility = Visibility.Visible;
                AppBarButtonDisplayFaces.Visibility = Visibility.Collapsed;
            }
            else
            {
                AppBarButtonDisplayFacesEnabled.Visibility = Visibility.Collapsed;
                AppBarButtonDisplayFaces.Visibility = Visibility.Visible;
                FaceRectanglesCanvas.Children.Clear();
            }
        }

        private async Task InitMediaDevice()
        {
            try
            {
                if (!FaceDetector.IsSupported) return;

                var cameraDevice = await CameraActions.FindCameraDeviceByPanelAsync(Windows.Devices.Enumeration.Panel.Front);
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
            if (!_displayFaces) return;
            // Use the dispatcher because this method is sometimes called from non-UI threads
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                RenderFaceRectangles(args.ResultFrame.DetectedFaces.ToList());
            });
        }

        private void RenderFaceRectangles(IList<DetectedFace> faces)
        {
            FaceRectanglesCanvas.Children.Clear();

            for (var i = 0; i < faces.Count; i++)
            {
                var faceBoundingBox = FaceUi.ConvertPreviewToUiRectangle(faces[i].FaceBox, CameraViewer, _previewProperties);
                faceBoundingBox.StrokeThickness = 4;
                faceBoundingBox.Stroke = (i == 0 ? new SolidColorBrush(Colors.HotPink) : new SolidColorBrush(Colors.DeepSkyBlue));
                FaceRectanglesCanvas.Children.Add(faceBoundingBox);
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
