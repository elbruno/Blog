using System.Collections.Generic;

namespace Microsoft.ProjectOxford.TextAnalytics.Contract.KeyPhraseResponse
{
    public class KeyPhrase
    {
        public List<Document> documents { get; set; }
        public List<Error> errors { get; set; }
    }
}