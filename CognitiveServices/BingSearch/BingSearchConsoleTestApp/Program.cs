using System;
using System.Linq;
using Microsoft.ProjectOxford.Search;

namespace BingSearchConsoleTestApp
{
    class Program
    {
        private static string _subscriptionKey = "";

        static void Main()
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
            var searchServiceClient = new SearchServiceClient(_subscriptionKey);
            var searchResult = await searchServiceClient.SearchAsync(query, 10);
            Console.WriteLine("Query: " + query);
            Console.WriteLine(" Type: " + searchResult._type);
            Console.WriteLine(" Total Estimated Match: " + searchResult.webPages.totalEstimatedMatches);
            Console.WriteLine(" Web search url: " + searchResult.webPages.webSearchUrl);

            foreach (var webPage in searchResult.webPages.value)
            {
                Console.WriteLine("  - Name: " + webPage.name);
                Console.WriteLine("  - Url: " + webPage.displayUrl);
                Console.WriteLine("  - Snippet: " + webPage.snippet);
                if (webPage.searchTags != null)
                {
                    var tags = webPage.searchTags.Aggregate(string.Empty,
                        (current, searchTag) => current + $@"{searchTag.name} [{searchTag.content}],");
                    Console.WriteLine("  - Tags: " + tags);
                }
                if (webPage.deepLinks != null)
                {
                    var deepLinks = webPage.deepLinks.Aggregate(string.Empty,
                        (current, deepLink) => current + $@"{deepLink.name} [{deepLink.url}],");
                    Console.WriteLine("  - Deep Links: " + deepLinks);
                }
            }

            Console.WriteLine(" Images");
            foreach (var image in searchResult.images.value)
            {
                Console.WriteLine("  - Name: " + image.name);
                System.Diagnostics.Process.Start(image.thumbnailUrl);
            }

            Console.WriteLine("Done !");
            Console.WriteLine("");
        }
    }
}
