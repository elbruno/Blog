using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using WpfTest01.Annotations;
using WpfTest01.Lib;
using WpfTest01.UserControls;

namespace WpfTest01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private ObservableCollection<Face> _detectedFaces;
        private ObservableCollection<Face> _facesRect;
        private string _selectedFile;
        private string _statusInformation;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void ButtonGetData_Click(object sender, RoutedEventArgs e)
        {
            // Show file picker dialog
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".jpg",
                Filter = "Image files(*.jpg) | *.jpg"
            };
            var result = dlg.ShowDialog();
            if (!result.HasValue || !result.Value) return;
            SelectedFile = dlg.FileName;
            var faceApi = new FaceApiHelper();
            var returnData = await faceApi.StartFaceDetection(SelectedFile, Properties.Settings.Default.FaceApiKey, Properties.Settings.Default.EmotionsApiKey);
            DetectedFaces = returnData.Item1;
            FacesRect = returnData.Item2;

            StatusInformation = $@"{DetectedFaces.Count} faces datected.";
        }


        public string StatusInformation
        {
            get { return _statusInformation; }
            set { _statusInformation = value; OnPropertyChanged(); }
        }

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

        public ObservableCollection<Face> FacesRect
        {
            get { return _facesRect; }
            set
            {
                if (Equals(value, _facesRect)) return;
                _facesRect = value;
                OnPropertyChanged();
            }
        }

        public string SelectedFile
        {
            get { return _selectedFile; }
            set
            {
                if (value == _selectedFile) return;
                _selectedFile = value;
                OnPropertyChanged();
            }
        }

        public int MaxImageSize
        {
            get
            {
                return 300;
            }
        }
    }
}
