using System;
using System.Net.Http;
using System.Text;
using PclAzureIoT;

namespace ConsoleDeviceToCloudPlainHttp
{
    class Program
    {
        static void Main()
        {
            var url = $"{Config.EventHubCompatibleName}/publishers/{Config.DeviceId}/messages";

            for (var i = 0; i < 3; i++)
            {
                Console.WriteLine("send for " + i);
                Console.ReadLine();
                var httpClient = new HttpClient { BaseAddress = new Uri(Config.EventHubUri) };
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", Config.SharedAccessSignature);
                var message = @"{""data"" : {""Bpm"": 123 ,""Steps"": " + i + @" }}";
                var content = new StringContent(message, Encoding.UTF8, "application/json");
                content.Headers.Add("ContentType", "application/atom+xml");
                httpClient.PostAsync(url, content);
            }
        }
    }
}
