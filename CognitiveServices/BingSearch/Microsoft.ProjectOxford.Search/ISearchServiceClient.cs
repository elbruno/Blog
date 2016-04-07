using System.Threading.Tasks;

namespace Microsoft.ProjectOxford.Search
{
    internal interface ISearchServiceClient
    {
        Task<Contract.Search> SearchAsync(string query, int count = 0, int offset = 0, string mkt = "", string safeSearch = "");
    }
}
