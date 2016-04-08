using System.Collections.Generic;

namespace Microsoft.ProjectOxford.TextAnalytics.Contract.LanguageResponse
{
    public class Document
    {
        public string id { get; set; }
        public List<DetectedLanguage> detectedLanguages { get; set; }
    }
}