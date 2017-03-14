using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.ProjectOxford.Common;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Face;

namespace ConsoleApplication1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Enter your Face API subscription key:");
            var subscriptionKeyFace = Console.ReadLine();
            Console.WriteLine("Enter your Emotions API subscription key:");
            var subscriptionKeyEmotion = Console.ReadLine();
            Console.WriteLine("Enter file location:");
            var fileLocation = Console.ReadLine();

            DetecFacesAndDisplayResult(fileLocation, subscriptionKeyFace, subscriptionKeyEmotion);
            Console.ReadLine();
        }

        private static async void DetecFacesAndDisplayResult(string fileLocation, string subscriptionKeyFace, string subscriptionKeyEmotion)
        {
            using (var fileStreamFace = File.OpenRead(fileLocation))
            {
                using (var fileStreamEmotions = File.OpenRead(fileLocation))
                {
                    try
                    {

                        var faceServiceClient = new FaceServiceClient(subscriptionKeyFace);
                        var emotionServiceClient = new EmotionServiceClient(subscriptionKeyEmotion);
                        var faces = await faceServiceClient.DetectAsync(fileStreamFace, false, true, true);
                        Console.WriteLine(" > " + faces.Length + " detected.");
                        if (faces.Length > 0)
                        {
                            var faceRectangles = new List<Rectangle>();
                            foreach (var face in faces)
                            {
                                Console.WriteLine(" >> age: " + face.Attributes.Age + " gender:" + face.Attributes.Gender);
                                var rectangle = new Rectangle
                                {
                                    Height = face.FaceRectangle.Height,
                                    Left = face.FaceRectangle.Left,
                                    Top = face.FaceRectangle.Top,
                                    Width = face.FaceRectangle.Width
                                };
                                faceRectangles.Add(rectangle);
                            }

                            // on face detected we start emotion analysis
                            var emotions = await emotionServiceClient.RecognizeAsync(fileStreamEmotions, faceRectangles.ToArray());
                            var emotionsDetails = "";
                            foreach (var emotion in emotions)
                            {
                                emotionsDetails += $@" Anger: {emotion.Scores.Anger}
    Contempt: {emotion.Scores.Contempt}
    Disgust: {emotion.Scores.Disgust}
    Fear: {emotion.Scores.Fear}
    Happiness: {emotion.Scores.Happiness}
    Neutral: {emotion.Scores.Neutral}
    Sadness: {emotion.Scores.Sadness}
    Surprise: {emotion.Scores.Surprise}
";
                            }

                            Console.WriteLine(" >> emotions: " + emotionsDetails);
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
}
