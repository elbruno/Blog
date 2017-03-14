using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Common;
using Newtonsoft.Json;

namespace Microsoft.ProjectOxford.Autosuggest
{
    public class AutosuggestServiceClient : ServiceClient, IAutosuggestServiceClient
    {

        public AutosuggestServiceClient(string subscriptionKey) : base()
        {
            ApiRoot = "https://bingapis.azure-api.net/api/v5/suggestions";
            AuthKey = "Ocp-Apim-Subscription-Key";
            AuthValue = subscriptionKey;
        }


        public async Task<Contract.Autosuggest> SuggestAsync(string query)
        {
            Contract.Autosuggest autoSuggestValue = null;
            var client = new HttpClient();

            // Request headers
            client.DefaultRequestHeaders.Add(AuthKey, AuthValue);

            // Request parameters
            var uri = GetApiUrl(query);

            var response = await client.GetAsync(uri);
            if (!response.IsSuccessStatusCode) return autoSuggestValue;
            string responseContent = null;
            if (response.Content != null)
            {
                responseContent = await response.Content.ReadAsStringAsync();
            }

            if (!string.IsNullOrWhiteSpace(responseContent))
            {
                autoSuggestValue = JsonConvert.DeserializeObject<Contract.Autosuggest>(responseContent, s_settings);
            }
            return autoSuggestValue;
        }

        private string GetApiUrl(string query)
        {
            var builder = new StringBuilder();
            builder.Append(ApiRoot);
            builder.Append(@"/?q=");
            builder.Append(query);
            return builder.ToString();
        }


    }
}
