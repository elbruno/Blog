using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Media;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using UwpAppYolo01.Yolo9000;

namespace UwpAppYolo01
{
    public sealed partial class MainPage : Page
    {
        private TinyYoloV2Model _model;
        private IList<YoloBoundingBox> _boxes = new List<YoloBoundingBox>();
        private readonly YoloWinMlParser _parser = new YoloWinMlParser();
        private readonly SolidColorBrush _lineBrushYellow = new SolidColorBrush(Windows.UI.Colors.Yellow);
        private readonly SolidColorBrush _lineBrushGreen = new SolidColorBrush(Windows.UI.Colors.Green);
        private readonly SolidColorBrush _fillBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
        private readonly double _lineThickness = 2.0;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadYoloOnnxModel();

            await CameraPreview.StartAsync();
            CameraPreview.CameraHelper.FrameArrived += CameraHelper_FrameArrived;
        }

        private async void LoadYoloOnnxModel()
        {
            var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Tiny-YOLOv2.onnx"));
            _model = await TinyYoloV2Model.CreateTinyYoloV2Model(file); //, 
        }

        private async void CameraHelper_FrameArrived(object sender, Microsoft.Toolkit.Uwp.Helpers.FrameEventArgs e)
        {
            if (e?.VideoFrame?.SoftwareBitmap == null) return;

            var input = new TinyYoloV2ModelInput { Image = e.VideoFrame };
            var output = await _model.EvaluateAsync(input);
            _boxes = _parser.ParseOutputs(output.Grid.ToArray());

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                DrawOverlays(e.VideoFrame);
            });
        }

        private void DrawOverlays(VideoFrame inputImage)
        {
            YoloCanvas.Children.Clear();
            if (_boxes.Count <= 0) return;
            var filteredBoxes = _parser.NonMaxSuppress(_boxes, 5, .5F);

            foreach (var box in filteredBoxes)
                DrawYoloBoundingBox(box, YoloCanvas);
        }

        private void DrawYoloBoundingBox(YoloBoundingBox box, Canvas overlayCanvas)
        {
            // Scale is set to stretched 416x416 - Clip bounding boxes to image area
            var x = (uint)Math.Max(box.X, 0);
            var y = (uint)Math.Max(box.Y, 0);
            var w = (uint)Math.Min(overlayCanvas.ActualWidth - x, box.Width);
            var h = (uint)Math.Min(overlayCanvas.ActualHeight - y, box.Height);

            var rectStroke = box.Label == "person" ? _lineBrushGreen : _lineBrushYellow;

            var r = new Windows.UI.Xaml.Shapes.Rectangle
            {
                Tag = box,
                Width = w,
                Height = h,
                Fill = _fillBrush,
                Stroke = rectStroke,
                StrokeThickness = _lineThickness,
                Margin = new Thickness(x, y, 0, 0)
            };

            var tb = new TextBlock
            {
                Margin = new Thickness(x + 4, y + 4, 0, 0),
                Text = $"{box.Label} ({Math.Round(box.Confidence, 4)})",
                FontWeight = FontWeights.Bold,
                Width = 126,
                Height = 21,
                HorizontalTextAlignment = TextAlignment.Center
            };

            var textBack = new Windows.UI.Xaml.Shapes.Rectangle
            {
                Width = 134,
                Height = 29,
                Fill = rectStroke,
                Margin = new Thickness(x, y, 0, 0)
            };

            overlayCanvas.Children.Add(textBack);
            overlayCanvas.Children.Add(tb);
            overlayCanvas.Children.Add(r);
        }
    }
}
