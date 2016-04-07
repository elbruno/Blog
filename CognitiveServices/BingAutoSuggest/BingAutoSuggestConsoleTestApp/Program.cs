using System;
using Microsoft.ProjectOxford.Autosuggest;

namespace ConsoleApplication1
{
    class Program
    {
        private static string _subscriptionKey = "";

        static void Main(string[] args)
        {
            Console.WriteLine("Write your subscription key");
            _subscriptionKey = Console.ReadLine();
            Console.WriteLine("Write your query search criteria");
            var query = Console.ReadLine();
            GetAutoSuggest(query);
            Console.ReadLine();
        }

        private static async void GetAutoSuggest(string query)
        {
            var autoSuggestServiceClient = new AutosuggestServiceClient(_subscriptionKey);
            var autosuggestResult = await autoSuggestServiceClient.SuggestAsync(query);
            Console.WriteLine("Query: " + query);
            Console.WriteLine(" Type: " + autosuggestResult._type);

            foreach (var suggestionGroup in autosuggestResult.suggestionGroups)
            {
                Console.WriteLine("  - Name: " + suggestionGroup.name);
                foreach (var searchSuggestion in suggestionGroup.searchSuggestions)
                {
                    Console.WriteLine("    - Suggestion Query : " + searchSuggestion.query + "[" + searchSuggestion.displayText + "]");
                    Console.WriteLine("    - Search Kind : " + searchSuggestion.searchKind);
                    //Console.WriteLine("    - Url : " + searchSuggestion.url);
                }
            }
            Console.WriteLine("Done !");
            Console.WriteLine("");
        }
    }
}
