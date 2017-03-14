// *********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
// *********************************************************
namespace Microsoft.ProjectOxford.Face
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// FaceSDK Project Name
        /// </summary>
        private string _appName;

        /// <summary>
        /// FaceSDK subscription key
        /// </summary>
        private string _subscriptionKey;

        /// <summary>
        /// Microsoft.ProjectOxford.Face project title
        /// </summary>
        private string _title;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;

            // "insert your own subscription key";
            this.SubscriptionKey = "4c138b4d82b947beb2e2926c92d1e514";

            this.AppName = "ProjectOxford";
            this.AppTitle = "Microsoft.ProjectOxford.Face Sample App";
            this.DataContext = this;
            App.Initialize(this.SubscriptionKey);

            this.FaceDetectionDescription = "Locate faces in an image. You can pick an image by 'Choose Image'. Detected faces will be shown on the image by rectangles surrounding the face, and related attributes will be shown in a list.";
            this.FaceVerificationDescription = "Determine whether two faces belong to the same person. You can pick single face image, the detected face will be shown on the image. Then click 'Verify' to get the verification result.";
            this.FaceGroupingDescription = "Put similar faces to same group according to appearance similarity. You can pick an image folder for grouping by 'Group', doing this will group all detected faces and shown under Grouping Result.";
            this.FaceFindSimilarDescription = "Find faces with appearance similarity. You can pick an image folder, all detected faces inside the folder will be treated as candidate. Use 'Open Query Face' to pick the query faces. The result will be list as 'query face's thumbnail'; similar to 'similar faces' thumbnails'.";
            this.FaceIdentificationDescription = "Tell whom an input face belongs to given a tagged person database. Here we only handle tagged person database in following format: 1). One root folder. 2). Sub-folders are named as person's name. 3). Each person's images are put into their own sub-folder. Pick the root folder, then choose an image to identify, all faces will be shown on the image with the identified person's name.";
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Implement INotifyPropertyChanged interface
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets sample application title
        /// </summary>
        public string AppTitle
        {
            get
            {
                return _title;
            }

            set
            {
                _title = value;
                OnPropertyChanged<string>();
            }
        }

        /// <summary>
        /// Gets or sets FaceSDK project name
        /// </summary>
        public string AppName
        {
            get
            {
                return _appName;
            }

            set
            {
                _appName = value;
                OnPropertyChanged<string>();
            }
        }

        /// <summary>
        /// Gets or sets subscription key
        /// </summary>
        public string SubscriptionKey
        {
            get
            {
                return _subscriptionKey;
            }

            set
            {
                _subscriptionKey = value;
                OnPropertyChanged<string>();
            }
        }

        /// <summary>
        /// Gets or sets description of face detection
        /// </summary>
        public string FaceDetectionDescription
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets description of face verification
        /// </summary>
        public string FaceVerificationDescription
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets description of face grouping 
        /// </summary>
        public string FaceGroupingDescription
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets description of find similar face
        /// </summary>
        public string FaceFindSimilarDescription
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets description of identification 
        /// </summary>
        public string FaceIdentificationDescription
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Navigate to relate statement page
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void Footer_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            System.Diagnostics.Process.Start(btn.Tag as string);
        }

        /// <summary>
        /// Navigate to how-to get subscription key help page
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.projectoxford.ai/doc/general/subscription-key-mgmt");
        }

        /// <summary>
        /// Prompt subscription key 
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event argument</param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.SubscriptionKey))
            {
                MessageBox.Show("Subscription key is missing. Please get your key and update \"this.SubscriptionKey\" in \"MainWindow.xaml.cs\" ", "Warning", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Helper function for INotifyPropertyChanged interface 
        /// </summary>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="caller">Property name</param>
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