using System;
using RestSharp;

namespace FlowConsole02UsingJsonClasses
{
    class Program
    {
        static void Main(string[] args)
        {
            var flowNotificationMessage = new FlowNotificationMessage
            {
                NotificationMessage = "Let's view ElBruno latest posts",
                NotificationLink = @"http://elbruno.com"
            };
            var jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(flowNotificationMessage);
            
            var client = new RestClient(Config.FlowUrl);
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", jsonBody, ParameterType.RequestBody);

            var response = client.Execute(request);

            Console.Write(response.Content);
            Console.ReadKey();
        }
    }
}
