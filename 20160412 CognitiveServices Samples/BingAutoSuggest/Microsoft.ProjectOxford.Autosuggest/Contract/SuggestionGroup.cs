using System.Collections.Generic;

namespace Microsoft.ProjectOxford.Autosuggest.Contract
{
    public class SuggestionGroup
    {
        public string name { get; set; }
        public List<SearchSuggestion> searchSuggestions { get; set; }
    }
}