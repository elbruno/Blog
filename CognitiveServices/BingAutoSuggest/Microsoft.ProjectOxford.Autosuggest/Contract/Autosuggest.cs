using System.Collections.Generic;

namespace Microsoft.ProjectOxford.Autosuggest.Contract
{
    public class Autosuggest
    {
        public string _type { get; set; }
        public QueryContext queryContext { get; set; }
        public List<SuggestionGroup> suggestionGroups { get; set; }
    }
}
