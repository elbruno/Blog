namespace Microsoft.ProjectOxford.Search.Contract
{
    public class Value2
    {
        public string name { get; set; }
        public string webSearchUrl { get; set; }
        public string thumbnailUrl { get; set; }
        public string datePublished { get; set; }
        public string contentUrl { get; set; }
        public string hostPageUrl { get; set; }
        public string contentSize { get; set; }
        public string encodingFormat { get; set; }
        public string hostPageDisplayUrl { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public Thumbnail thumbnail { get; set; }
        public InsightsSourcesSummary insightsSourcesSummary { get; set; }
    }
}