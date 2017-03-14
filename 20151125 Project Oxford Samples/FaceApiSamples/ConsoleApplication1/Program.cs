using System;
using System.IO;
using Microsoft.ProjectOxford.Face;

namespace ConsoleApplication1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Enter your subscription key:");
            var subscriptionKey = Console.ReadLine();
            Console.WriteLine("Enter file location:");
            var fileLocation = Console.ReadLine();
            DetecFacesAndDisplayResult(fileLocation, subscriptionKey);
            Console.ReadLine();
        }

        private static async void DetecFacesAndDisplayResult(string fileLocation, string subscriptionKey)
        {
            using (var fileStream = File.OpenRead(fileLocation))
            {
                try
                {
                    var client = new FaceServiceClient(subscriptionKey);
                    var faces = await client.DetectAsync(fileStream, false, true, true);
                    Console.WriteLine(" > " + faces.Length + " detected.");
                    foreach (var face in faces)
                    {
                        Console.WriteLine(" >> age: " + face.Attributes.Age + " gender:" + face.Attributes.Gender);
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.ToString());
                }
            }
        }
    }
}
