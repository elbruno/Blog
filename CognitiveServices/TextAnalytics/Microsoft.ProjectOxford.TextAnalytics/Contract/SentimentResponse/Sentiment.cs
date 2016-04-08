using System.Collections.Generic;

namespace Microsoft.ProjectOxford.TextAnalytics.Contract.SentimentResponse
{
    public class Sentiment
    {
        public List<Document> documents { get; set; }
        public List<Error> errors { get; set; }
    }
}