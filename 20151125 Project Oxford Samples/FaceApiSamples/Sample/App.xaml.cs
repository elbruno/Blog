// *********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
// *********************************************************
namespace Microsoft.ProjectOxford.Face
{
    using System.Windows;

    using Microsoft.ProjectOxford.Face;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Fields

        /// <summary>
        /// Face service client
        /// </summary>
        private static FaceServiceClient _instance;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="App"/> class from being created
        /// </summary>
        private App()
        {
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets face service client
        /// </summary>
        public static FaceServiceClient Instance
        {
            get
            {
                return _instance;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Initialize Face service client given subscription key
        /// </summary>
        /// <param name="subscriptionKey">subscription key</param>
        public static void Initialize(string subscriptionKey)
        {
            _instance = new FaceServiceClient(subscriptionKey);
        }

        /// <summary>
        /// Show unhandled exception in message box
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is ClientException)
            {
                var ex = e.Exception as ClientException;
                MessageBox.Show(ex.Error.Message, "Face API Calling Error", MessageBoxButton.OK);
            }
            else
            {
                if (e.Exception.InnerException != null)
                {
                    MessageBox.Show(e.Exception.InnerException.ToString(), "Error", MessageBoxButton.OK);
                }
                else
                {
                    MessageBox.Show(e.Exception.ToString(), "Error", MessageBoxButton.OK);
                }
            }

            e.Handled = true;
        }

        #endregion Methods
    }
}