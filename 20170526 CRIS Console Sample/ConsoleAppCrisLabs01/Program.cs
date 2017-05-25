using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.SpeechRecognition;

namespace ConsoleAppCrisLabs01
{
    class Program
    {
        private static DataRecognitionClient _dataClient;

        static void Main(string[] args)
        {

            var mode = SpeechRecognitionMode.LongDictation;
            var language = "en-US";
            var authenticationUri = "https://westus.api.cognitive.microsoft.com/sts/v1.0/issueToken";
            var crisSubscriptionKey = Config.CrisSubscriptionKey;
            var crisUri = Config.CrisUri;

            _dataClient = SpeechRecognitionServiceFactory.CreateDataClient(mode, language, crisSubscriptionKey, crisSubscriptionKey, crisUri);
            _dataClient.AuthenticationUri = authenticationUri;

            _dataClient.OnResponseReceived += OnDataDictationResponseReceivedHandler;
            _dataClient.OnConversationError += OnConversationErrorHandler;
            _dataClient.OnIntent += OnIntentHandler;

            // start process
            SendAudioHelper("sample01.wav");
            Console.WriteLine("Process started, wait for results ...");

            Console.ReadLine();
        }

        private static void OnIntentHandler(object sender, SpeechIntentEventArgs e)
        {
            Console.WriteLine($"OnIntentHandler - Payload: {e.Payload}");
        }

        private static void OnConversationErrorHandler(object sender, SpeechErrorEventArgs e)
        {
            Console.WriteLine($"Exception: {e}");
        }

        private static void OnDataDictationResponseReceivedHandler(object sender, SpeechResponseEventArgs e)
        {
            if (!e.PhraseResponse.Results.Any()) return;
            foreach (var phraseResponseResult in e.PhraseResponse.Results)
            {
                Console.WriteLine($@"Response result
  - Confidence: {phraseResponseResult.Confidence}
  - Display Text: {phraseResponseResult.DisplayText}
  - InverseTextNormalizationResult: {phraseResponseResult.InverseTextNormalizationResult}
  - LexicalForm: {phraseResponseResult.LexicalForm}
  - MaskedInverseTextNormalizationResult: {phraseResponseResult.MaskedInverseTextNormalizationResult}");
            }
        }

        private static void SendAudioHelper(string wavFileName)
        {
            using (FileStream fileStream = new FileStream(wavFileName, FileMode.Open, FileAccess.Read))
            {
                int bytesRead = 0;
                byte[] buffer = new byte[1024];

                try
                {
                    do
                    {
                        bytesRead = fileStream.Read(buffer, 0, buffer.Length);
                        _dataClient.SendAudio(buffer, bytesRead);
                    }
                    while (bytesRead > 0);
                }
                finally
                {
                    _dataClient.EndAudio();
                }
            }
        }
    }
}