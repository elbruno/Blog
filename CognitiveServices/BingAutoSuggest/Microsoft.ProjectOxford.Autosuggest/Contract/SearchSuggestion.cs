namespace Microsoft.ProjectOxford.Autosuggest.Contract
{
    public class SearchSuggestion
    {
        public string url { get; set; }
        public string displayText { get; set; }
        public string query { get; set; }
        public string searchKind { get; set; }
    }
}