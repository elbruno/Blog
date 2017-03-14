using System.Collections.Generic;

namespace Microsoft.ProjectOxford.TextAnalytics.Contract.KeyPhraseResponse
{

    public class Document
    {
        public List<string> keyPhrases { get; set; }
        public string id { get; set; }
    }
}
