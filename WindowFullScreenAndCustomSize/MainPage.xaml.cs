using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace App1
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var currentView = ApplicationView.GetForCurrentView();
            if (!currentView.IsFullScreen)
                currentView.TryEnterFullScreenMode();
            else
            {
                currentView.ExitFullScreenMode();
                currentView.TryResizeView(new Size(767, 383));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var currentView = ApplicationView.GetForCurrentView();
            currentView.TryResizeView(new Size(767, 383));
        }
    }
}
