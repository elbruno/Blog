using System.Collections.Generic;

namespace Microsoft.ProjectOxford.Search.Contract
{
    public class Value
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string displayUrl { get; set; }
        public string snippet { get; set; }
        public List<DeepLink> deepLinks { get; set; }
        public string dateLastCrawled { get; set; }
        public List<SearchTag> searchTags { get; set; }
    }
}