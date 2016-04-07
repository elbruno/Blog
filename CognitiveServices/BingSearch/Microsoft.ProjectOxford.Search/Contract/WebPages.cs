using System.Collections.Generic;

namespace Microsoft.ProjectOxford.Search.Contract
{
    public class WebPages
    {
        public string webSearchUrl { get; set; }
        public int totalEstimatedMatches { get; set; }
        public List<Value> value { get; set; }
    }
}