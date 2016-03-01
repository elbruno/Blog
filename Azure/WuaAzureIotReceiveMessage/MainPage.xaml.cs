using System;
using System.Diagnostics;
using System.Text;
using Windows.UI.Notifications;
using NotificationsExtensions.Toasts;
using ppatierno.AzureSBLite.Messaging;
using PclAzureIoT;

namespace WuaAzureIotReceiveMessage
{
    public sealed partial class MainPage
    {

        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var partitionId = "1";
            DateTime startingDateTimeUtc;

            var factory = MessagingFactory.CreateFromConnectionString(Config.ConnectionStringAzureSbLite);
            var client = factory.CreateEventHubClient(Config.EventHubCompatibleName);
            var group = client.GetDefaultConsumerGroup();
            startingDateTimeUtc = DateTime.Now;
            var receiver = group.CreateReceiver(partitionId, startingDateTimeUtc);

            while (true)
            {
                var eventData = receiver.Receive();
                if (eventData == null) continue;
                var eventDataString = Encoding.UTF8.GetString(eventData.GetBytes());
                Debug.WriteLine("{0} {1} {2}", eventData.PartitionKey, eventData.EnqueuedTimeUtc.ToLocalTime(), eventDataString);
                SendToastNotification(eventDataString);
            }

            client.Close();
            factory.Close();
        }

        private void SendToastNotification(string heartRate)
        {
            const string title = "New heart rate measurement";
            var content = heartRate;
            const string image = "https://brunocapuano.files.wordpress.com/2016/02/clipboard012.png"; // "https://brunocapuano.files.wordpress.com/2016/01/ohhh-q-asombroso.gif";
            const string logo = "ms-appdata:///local/Heart-02.png";

            var visual = new ToastVisual()
            {
                TitleText = new ToastText()
                {
                    Text = title
                },

                BodyTextLine1 = new ToastText()
                {
                    Text = content
                },

                InlineImages =
                {
                    new ToastImage()
                    {
                        Source = new ToastImageSource(image)
                    }
                },
                AppLogoOverride = new ToastAppLogo()
                {
                    Source = new ToastImageSource(logo),
                    Crop = ToastImageCrop.None
                }
            };

            var toastContent = new ToastContent()
            {
                Visual = visual
            };
            var toast = new ToastNotification(toastContent.GetXml())
            {
                Tag = "1",
                Group = "Garmin"
            };
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

    }
}
