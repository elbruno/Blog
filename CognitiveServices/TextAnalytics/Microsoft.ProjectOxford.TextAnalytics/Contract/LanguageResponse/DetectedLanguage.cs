namespace Microsoft.ProjectOxford.TextAnalytics.Contract.LanguageResponse
{
    public class DetectedLanguage
    {
        public string name { get; set; }
        public string iso6391Name { get; set; }
        public double score { get; set; }
    }
}