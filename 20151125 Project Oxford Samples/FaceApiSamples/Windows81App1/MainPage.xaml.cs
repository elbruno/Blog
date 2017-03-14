using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using Windows81App1.Annotations;
using Windows81App1.Lib;
using Windows81App1.UserControls;

namespace Windows81App1
{
    public sealed partial class MainPage : INotifyPropertyChanged
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = this;

        }

        private async void ButtonGetData_Click(object sender, RoutedEventArgs e)
        {
            var openPicker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                ViewMode = PickerViewMode.Thumbnail
            };

            openPicker.FileTypeFilter.Clear();
            openPicker.FileTypeFilter.Add(".bmp");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".jpg");

            var file = await openPicker.PickSingleFileAsync();
            if (file == null) return;

            FileHelper.ClearTempFolder();

            PerformFaceAnalysis(file);
        }

        private async void ButtonGetWebCamPicture_Click(object sender, RoutedEventArgs e)
        {
            var file = await CameraActions.TakeWebCamPictureAndReturnFile(false);
            PerformFaceAnalysis(file);
        }

        private async void PerformFaceAnalysis(StorageFile file)
        {
            var imageInfo = await FileHelper.GetImageInfoForRendering(file.Path);
            NewImageSizeWidth = 300;
            NewImageSizeHeight = NewImageSizeWidth*imageInfo.Item2/imageInfo.Item1;

            var newSourceFile = await FileHelper.CreateCopyOfSelectedImage(file);
            var uriSource = new Uri(newSourceFile.Path);
            SelectedFileBitmapImage = new BitmapImage(uriSource);


            // start face api detection
            var faceApi = new FaceApiHelper();
            DetectedFaces = await faceApi.StartFaceDetection(newSourceFile.Path, newSourceFile, imageInfo, "4c138b4d82b947beb2e2926c92d1e514");

            // draw rectangles 
            var color = Color.FromArgb(125, 255, 0, 0);
            var bg = new SolidColorBrush(color);

            DetectedFacesCanvas = new ObservableCollection<Canvas>();
            foreach (var detectedFace in DetectedFaces)
            {
                var margin = new Thickness(detectedFace.RectLeft, detectedFace.RectTop, 0, 0);
                var canvas = new Canvas()
                {
                    Background = bg,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Height = detectedFace.RectHeight,
                    Width = detectedFace.RectWidth,
                    Margin = margin
                };
                DetectedFacesCanvas.Add(canvas);
            }
        }


        #region Properties
        private ObservableCollection<Face> _detectedFaces;
        private BitmapImage _selectedFileBitmapImage;
        private int _newImageSizeWidth;
        private int _newImageSizeHeight;
        private ObservableCollection<Canvas> _detectedFacesCanvas;

        public ObservableCollection<Face> DetectedFaces
        {
            get { return _detectedFaces; }
            set
            {
                if (Equals(value, _detectedFaces)) return;
                _detectedFaces = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Canvas> DetectedFacesCanvas
        {
            get { return _detectedFacesCanvas; }
            set
            {
                if (Equals(value, _detectedFacesCanvas)) return;
                _detectedFacesCanvas = value;
                OnPropertyChanged();
            }
        }

        public BitmapImage SelectedFileBitmapImage
        {
            get
            {
                return _selectedFileBitmapImage;
            }
            set
            {
                if (value == _selectedFileBitmapImage) return;
                _selectedFileBitmapImage = value;
                OnPropertyChanged();
            }
        }

        public int NewImageSizeWidth
        {
            get { return _newImageSizeWidth; }
            set
            {
                if (value == _newImageSizeWidth) return;
                _newImageSizeWidth = value;
                OnPropertyChanged();
            }
        }

        public int NewImageSizeHeight
        {
            get { return _newImageSizeHeight; }
            set
            {
                if (value == _newImageSizeHeight) return;
                _newImageSizeHeight = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region On Property Changed
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }
}
