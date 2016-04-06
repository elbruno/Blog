using System.Collections.Generic;

namespace Microsoft.ProjectOxford.Autosuggest.Contract
{
    public class Autosuggest
    {
        public string _type { get; set; }
        public QueryContext queryContext { get; set; }
        public List<SuggestionGroup> suggestionGroups { get; set; }
    }

    public class QueryContext
    {
        public string originalQuery { get; set; }
    }

    public class SearchSuggestion
    {
        public string url { get; set; }
        public string displayText { get; set; }
        public string query { get; set; }
        public string searchKind { get; set; }
    }

    public class SuggestionGroup
    {
        public string name { get; set; }
        public List<SearchSuggestion> searchSuggestions { get; set; }
    }
}
