using System.Collections.Generic;

namespace Microsoft.ProjectOxford.TextAnalytics.Contract.LanguageResponse
{
    public class Text
    {
        public List<Document> documents { get; set; }
        public List<Error> errors { get; set; }
    }
}
