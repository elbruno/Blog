using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.TextAnalytics;

namespace TextAnalyticsConsoleTestAppConsoleTestApp
{
    class Program
    {
        private static string _subscriptionKey = "";

        static void Main()
        {
            Console.WriteLine("Write your subscription key");
            _subscriptionKey = Console.ReadLine();
            Console.WriteLine("Write your text for text analysis:");
            var query = Console.ReadLine();
            AnalyzeText(query);
            Console.ReadLine();

            Console.Clear();
            Console.WriteLine("Write your text for text analysis:");
            query = Console.ReadLine();
            AnalyzeText(query);

            Console.ReadLine();
        }

        private static async void AnalyzeText(string text)
        {
            var analyticsServiceClient = new TextAnalyticsServiceClient(_subscriptionKey);
            Console.WriteLine("");
            await AnalyzeLanguage(text, analyticsServiceClient);
            await AnalyzeSentiment(text, analyticsServiceClient);
            await DetectKeyPhrases(text, analyticsServiceClient);

            Console.WriteLine("Done !");
            Console.WriteLine("");
        }

        private static async Task AnalyzeLanguage(string text, TextAnalyticsServiceClient analyticsServiceClient)
        {
            var analytic = await analyticsServiceClient.AnalyzeLanguageAsync(text);
            Console.WriteLine(" - Language ");
            foreach (var document in analytic.documents)
            {
                var langs = document.detectedLanguages.Aggregate("",
                    (current, detectedLanguage) =>
                        current +
                        $@"name: {detectedLanguage.name} [{detectedLanguage.iso6391Name}], score [{detectedLanguage.score}] {
                            Environment.NewLine}");
                Console.WriteLine($@"  - Id: {document.id} - {langs}");
            }

            foreach (var error in analytic.errors)
            {
                Console.WriteLine($@"  - Id: {error.id}, {error.message}");
            }
        }
        private static async Task AnalyzeSentiment(string text, TextAnalyticsServiceClient analyticsServiceClient)
        {
            var sentiment = await analyticsServiceClient.AnalyzeSentimentAsync(text);
            Console.WriteLine(" - Sentiment ");
            foreach (var document in sentiment.documents)
            {
                Console.WriteLine($@"  - {document.id} Sentiment [{document.score}]");
            }

            foreach (var error in sentiment.errors)
            {
                Console.WriteLine($@"  - Id: {error.id}, {error.message}");
            }
        }
        private static async Task DetectKeyPhrases(string text, TextAnalyticsServiceClient analyticsServiceClient)
        {
            var keyphrases = await analyticsServiceClient.DetectKeyPhrasesAsync(text);
            Console.WriteLine(" - KeyPhrases ");
            foreach (var document in keyphrases.documents)
            {
                foreach (var keyPhrase in document.keyPhrases)
                {
                    Console.WriteLine($@"  key phrase: {keyPhrase}");
                }
            }

            foreach (var error in keyphrases.errors)
            {
                Console.WriteLine($@"  - Id: {error.id}, {error.message}");
            }
        }
    }
}
