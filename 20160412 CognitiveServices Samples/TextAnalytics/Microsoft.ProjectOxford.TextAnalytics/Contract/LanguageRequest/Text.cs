using System.Collections.Generic;

namespace Microsoft.ProjectOxford.TextAnalytics.Contract.LanguageRequest
{
    public class Text
    {
        public List<Document> documents { get; set; }
        public Text()
        {
            documents = new List<Document>();
        }
    }
}
