using System.Collections.Generic;
using System.Linq;
using System.Net;
using Windows.UI.Xaml;
using Tweetinvi;
using Tweetinvi.Core.Credentials;
using Tweetinvi.Core.Interfaces.DTO;
using Tweetinvi.Core.Parameters;

namespace PostToTwitterApp1
{
    public sealed partial class MainPage
    {
        const string ConsumerKey = "<-- complete -->";
        const string ConsumerSecret = "<-- complete -->";
        const string AccessToken = "<-- complete -->";
        const string AccessTokenSecret = "<-- complete -->";

        public MainPage()
        {
            InitializeComponent();
            InitTwitterCredentials();
        }
        private static void InitTwitterCredentials()
        {
            var creds = new TwitterCredentials(AccessToken, AccessTokenSecret, ConsumerKey, ConsumerSecret);
            Auth.SetUserCredentials(ConsumerKey, ConsumerSecret, AccessToken, AccessTokenSecret);
            Auth.ApplicationCredentials = creds;
        }

        private void ButtonSendTweet_Click(object sender, RoutedEventArgs e)
        {
            PublishTweet("El Bruno sample tweet", "https://brunocapuano.files.wordpress.com/2015/12/clipboard025.png");
        }

        public async void PublishTweet(string text, string imageUrl)
        {
            var response = await WebRequest.Create(imageUrl).GetResponseAsync();
            var allBytes = new List<byte>();
            using (var imageStream = response.GetResponseStream())
            {
                byte[] buffer = new byte[4000];
                int bytesRead;
                while ((bytesRead = await imageStream.ReadAsync(buffer, 0, 4000)) > 0)
                {
                    allBytes.AddRange(buffer.Take(bytesRead));
                }
            }
            var media = Upload.UploadBinary(allBytes.ToArray());
            Tweet.PublishTweet(text, new PublishTweetOptionalParameters
            {
                Medias = new List<IMedia> { media }
            });
        }
    }
}
