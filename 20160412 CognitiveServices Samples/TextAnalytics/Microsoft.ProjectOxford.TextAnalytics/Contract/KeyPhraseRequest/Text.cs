using System.Collections.Generic;

namespace Microsoft.ProjectOxford.TextAnalytics.Contract.KeyPhraseRequest
{
    public class Text
    {
        public Text()
        {
            documents = new List<Document>();
        }
        public List<Document> documents { get; set; }
    }
}