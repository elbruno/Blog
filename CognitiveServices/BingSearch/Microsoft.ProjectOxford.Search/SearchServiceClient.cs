using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Common;
using Newtonsoft.Json;

namespace Microsoft.ProjectOxford.Search
{
    public class SearchServiceClient : ServiceClient, ISearchServiceClient
    {

        public SearchServiceClient(string subscriptionKey) 
        {
            ApiRoot = "https://bingapis.azure-api.net/api/v5/search";
            AuthKey = "Ocp-Apim-Subscription-Key";
            AuthValue = subscriptionKey;
        }


        public async Task<Contract.Search> SearchAsync(string query, int count = 0, int offset = 0, string mkt = "", string safeSearch = "")
        {
            Contract.Search search = null;
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add(AuthKey, AuthValue);
            var uri = GetApiUrl(query, count, offset, mkt, safeSearch);
            var response = await client.GetAsync(uri);
            if (!response.IsSuccessStatusCode) return search;
            string responseContent = null;
            if (response.Content != null)
            {
                responseContent = await response.Content.ReadAsStringAsync();
            }

            if (!string.IsNullOrWhiteSpace(responseContent))
            {
                search = JsonConvert.DeserializeObject<Contract.Search>(responseContent, s_settings);
            }
            return search;
        }

        private string GetApiUrl(string query, int count = 0, int offset = 0, string mkt = "", string safeSearch = "")
        {
            var builder = new StringBuilder();
            builder.Append(ApiRoot);
            builder.Append(@"/?q=");
            builder.Append(query);
            if (count > 0)
            {
                builder.Append(@"&count=");
                builder.Append(count);
            }
            if (offset > 0)
            {
                builder.Append(@"&offset=");
                builder.Append(count);
            }
            if (!string.IsNullOrEmpty(mkt))
            {
                builder.Append(@"&mkt=");
                builder.Append(mkt);
            }
            if (!string.IsNullOrEmpty(safeSearch))
            {
                builder.Append(@"&safeSearch=");
                builder.Append(safeSearch);
            }
            return builder.ToString();
        }
    }
}
