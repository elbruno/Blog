using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Common;
using Microsoft.ProjectOxford.TextAnalytics.Contract.LanguageResponse;
using Newtonsoft.Json;
using Document = Microsoft.ProjectOxford.TextAnalytics.Contract.LanguageRequest.Document;

namespace Microsoft.ProjectOxford.TextAnalytics
{
    public class TextAnalyticsServiceClient : ServiceClient, ITextAnalyticsServiceClient
    {
        private string _languageUri = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/languages";
        private string _sentimentUri = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/sentiment";
        private string _keyphrasesUri = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/keyPhrases";

        public TextAnalyticsServiceClient(string subscriptionKey) 
        {
            ApiRoot = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/languages";
            AuthKey = "Ocp-Apim-Subscription-Key";
            AuthValue = subscriptionKey;
        }

        public async Task<Text> AnalyzeLanguageAsync(string text)
        {
            Text responseText = null;
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add(AuthKey, AuthValue);

            var requestText = new Contract.LanguageRequest.Text();
            var doc = new Document
            {
                id = "string",
                text = text
            };
            requestText.documents.Add(doc);

            HttpResponseMessage response;
            var req = JsonConvert.SerializeObject(requestText);
            var byteData = Encoding.UTF8.GetBytes(req);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(_languageUri, content);
            }

            if (!response.IsSuccessStatusCode) return responseText;
            string responseContent = null;
            if (response.Content != null)
            {
                responseContent = await response.Content.ReadAsStringAsync();
            }

            if (!string.IsNullOrWhiteSpace(responseContent))
            {
                responseText = JsonConvert.DeserializeObject<Text>(responseContent, s_settings);
            }
            return responseText;
        }
        public async Task<Contract.SentimentResponse.Sentiment> AnalyzeSentimentAsync(string text)
        {
            Contract.SentimentResponse.Sentiment responseSentiment = null;
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add(AuthKey, AuthValue);

            var requestText = new Contract.LanguageRequest.Text();
            var doc = new Document
            {
                id = "string",
                text = text
            };
            requestText.documents.Add(doc);

            HttpResponseMessage response;
            var req = JsonConvert.SerializeObject(requestText);
            var byteData = Encoding.UTF8.GetBytes(req);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(_sentimentUri, content);
            }

            if (!response.IsSuccessStatusCode) return responseSentiment;
            string responseContent = null;
            if (response.Content != null)
            {
                responseContent = await response.Content.ReadAsStringAsync();
            }

            if (!string.IsNullOrWhiteSpace(responseContent))
            {
                responseSentiment = JsonConvert.DeserializeObject<Contract.SentimentResponse.Sentiment>(responseContent, s_settings);
            }
            return responseSentiment;
        }

        public async Task<Contract.KeyPhraseResponse.KeyPhrase> DetectKeyPhrasesAsync(string text)
        {
            Contract.KeyPhraseResponse.KeyPhrase responseKeyPhrases = null;
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add(AuthKey, AuthValue);

            var requestText = new Contract.KeyPhraseRequest.Text();
            var doc = new Contract.KeyPhraseRequest.Document()
            {
                id = "string",
                text = text
            };
            requestText.documents.Add(doc);

            HttpResponseMessage response;
            var req = JsonConvert.SerializeObject(requestText);
            var byteData = Encoding.UTF8.GetBytes(req);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(_keyphrasesUri, content);
            }

            if (!response.IsSuccessStatusCode) return responseKeyPhrases;
            string responseContent = null;
            if (response.Content != null)
            {
                responseContent = await response.Content.ReadAsStringAsync();
            }

            if (!string.IsNullOrWhiteSpace(responseContent))
            {
                responseKeyPhrases = JsonConvert.DeserializeObject<Contract.KeyPhraseResponse.KeyPhrase>(responseContent, s_settings);
            }
            return responseKeyPhrases;
        }
    }
}
