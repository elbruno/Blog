// *********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
// *********************************************************

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.ProjectOxford.Emotion.Contract;

namespace ElBruno.ProjectOxford.FaceApiEmotionVisionSample.UserControls
{
    /// <summary>
    /// Face view model
    /// </summary>
    public class Face : INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// Face gender text string
        /// </summary>
        private string _gender;

        /// <summary>
        /// Face age text string
        /// </summary>
        private string _age;

        /// <summary>
        /// Person name
        /// </summary>
        private string _personName;

        /// <summary>
        /// Face height in pixel
        /// </summary>
        private int _height;

        /// <summary>
        /// Face position X relative to image left-top in pixel
        /// </summary>
        private int _left;

        /// <summary>
        /// Face position Y relative to image left-top in pixel
        /// </summary>
        private int _top;

        /// <summary>
        /// Face width in pixel
        /// </summary>
        private int _width;

        private Scores _scores;
        private string _scoredEmotion;
        private Guid _faceId;

        #endregion Fields

        #region Events

        /// <summary>
        /// Implement INotifyPropertyChanged interface
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        public Guid FaceId
        {
            get
            {
                return _faceId;
            }

            set
            {
                _faceId = value;
                OnPropertyChanged<Guid>();
            }
        }

        /// <summary>
        /// Gets or sets gender text string 
        /// </summary>
        public string Gender
        {
            get
            {
                return _gender;
            }

            set
            {
                _gender = value;
                OnPropertyChanged<string>();
            }
        }

        /// <summary>
        /// Gets or sets age text string
        /// </summary>
        public string Age
        {
            get
            {
                return _age;
            }

            set
            {
                _age = value;
                OnPropertyChanged<string>();
            }
        }

        /// <summary>
        /// Gets face rectangle on image
        /// </summary>
        public System.Windows.Int32Rect UIRect
        {
            get
            {
                return new System.Windows.Int32Rect(Left, Top, Width, Height);
            }
        }

        /// <summary>
        /// Gets or sets image path
        /// </summary>
        public string ImagePath
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets person's name
        /// </summary>
        public string PersonName
        {
            get
            {
                return _personName;
            }

            set
            {
                _personName = value;
                OnPropertyChanged<string>();
            }
        }

        /// <summary>
        /// Gets or sets face height
        /// </summary>
        public int Height
        {
            get
            {
                return _height;
            }

            set
            {
                _height = value;
                OnPropertyChanged<int>();
            }
        }

        /// <summary>
        /// Gets or sets face position X
        /// </summary>
        public int Left
        {
            get
            {
                return _left;
            }

            set
            {
                _left = value;
                OnPropertyChanged<int>();
            }
        }

        /// <summary>
        /// Gets or sets face position Y
        /// </summary>
        public int Top
        {
            get
            {
                return _top;
            }

            set
            {
                _top = value;
                OnPropertyChanged<int>();
            }
        }

        /// <summary>
        /// Gets or sets face width
        /// </summary>
        public int Width
        {
            get
            {
                return _width;
            }

            set
            {
                _width = value;
                OnPropertyChanged<int>();
            }
        }

        public Scores Scores
        {
            get { return _scores; }
            set
            {
                _scores = value;
                CalculateEmotion();
                OnPropertyChanged<Scores>();
            }
        }


        public string ScoredEmotion
        {
            get
            {
                return _scoredEmotion;
            }
            set
            {
                _scoredEmotion = value;
                OnPropertyChanged<string>();
            }
        }

        #endregion Properties

        #region Methods

        public void CalculateEmotion()
        {
            if (Scores == null) return;
            var highValue = 0f;
            if (Scores.Anger > highValue)
            {
                _scoredEmotion = "Anger";
                highValue = Scores.Contempt;
            }
            if (Scores.Contempt > highValue)
            {
                _scoredEmotion = "Contempt";
                highValue = Scores.Contempt;
            }
            if (Scores.Disgust > highValue)
            {
                _scoredEmotion = "Disgust";
                highValue = Scores.Disgust;
            }
            if (Scores.Fear > highValue)
            {
                _scoredEmotion = "Fear";
                highValue = Scores.Fear;
            }
            if (Scores.Happiness > highValue)
            {
                _scoredEmotion = "Happiness";
                highValue = Scores.Happiness;
            }
            if (Scores.Neutral > highValue)
            {
                _scoredEmotion = "Neutral";
                highValue = Scores.Neutral;
            }
            if (Scores.Sadness > highValue)
            {
                _scoredEmotion = "Sadness";
                highValue = Scores.Sadness;
            }
            if (Scores.Surprise > highValue)
                _scoredEmotion = "Surprise ";
        }

        /// <summary>
        /// NotifyProperty Helper functions
        /// </summary>
        /// <typeparam name="T">property type</typeparam>
        /// <param name="caller">property change caller</param>
        private void OnPropertyChanged<T>([CallerMemberName]string caller = null)
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