using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Media.Imaging;

namespace Windows81App1.UserControls
{
    public class Face : INotifyPropertyChanged
    {
        #region Fields

        private string _gender;
        private double _age;
        private string _personName;
        private int _height;
        private int _left;
        private int _top;
        private int _width;
        private int _rectHeight;
        private int _rectLeft;
        private int _rectTop;
        private int _rectWidth;
        private string _imageFacePath;

        private BitmapImage _imageFaceBitmapImage;
        private string _ageComplete;

        #endregion Fields

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        public string Gender
        {
            get
            {
                return _gender;
            }

            set
            {
                _gender = value;
                OnPropertyChanged();
            }
        }

        public string Information
        {
            get
            {
                return string.Format("{0}, {1}", _age, _gender);
            }
        }

        public double Age
        {
            get
            {
                return _age;
            }

            set
            {
                _age = value;
                OnPropertyChanged();
            }
        }
        public string AgeComplete
        {
            get
            {
                return _ageComplete;
            }

            set
            {
                _ageComplete = value;
                OnPropertyChanged();
            }
        }

        public string ImagePath
        {
            get;
            set;
        }

        public string FaceId
        {
            get;
            set;
        }

        public string PersonName
        {
            get
            {
                return _personName;
            }

            set
            {
                _personName = value;
                OnPropertyChanged();
            }
        }

        public int Height
        {
            get
            {
                return _height;
            }

            set
            {
                _height = value;
                OnPropertyChanged();
            }
        }

        public int Left
        {
            get
            {
                return _left;
            }

            set
            {
                _left = value;
                OnPropertyChanged();
            }
        }

        public int Top
        {
            get
            {
                return _top;
            }

            set
            {
                _top = value;
                OnPropertyChanged();
            }
        }

        public string ImageFacePath
        {
            get { return _imageFacePath; }
            set
            {
                _imageFacePath = value;
                ImageFaceBitmapImage = new BitmapImage(new Uri(_imageFacePath));
                OnPropertyChanged();
            }
        }

        public int Width
        {
            get
            {
                return _width;
            }

            set
            {
                _width = value;
                OnPropertyChanged();
            }
        }

        public BitmapImage ImageFaceBitmapImage
        {
            get { return _imageFaceBitmapImage; }
            set
            {
                _imageFaceBitmapImage = value;
                OnPropertyChanged();
            }
        }

        public int RectHeight
        {
            get { return _rectHeight; }
            set
            {
                _rectHeight = value;
                OnPropertyChanged();
            }
        }

        public int RectLeft
        {
            get { return _rectLeft; }
            set
            {
                _rectLeft = value;
                OnPropertyChanged();
            }
        }

        public int RectTop
        {
            get { return _rectTop; }
            set
            {
                _rectTop = value;
                OnPropertyChanged();
            }
        }

        public int RectWidth
        {
            get { return _rectWidth; }
            set
            {
                _rectWidth = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region Methods

        private void OnPropertyChanged([CallerMemberName]string caller = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(caller));
            }
        }

        #endregion Methods
    }
}