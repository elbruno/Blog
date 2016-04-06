using System.Threading.Tasks;

namespace Microsoft.ProjectOxford.Autosuggest
{
    internal interface IAutosuggestServiceClient
    {
        Task<Contract.Autosuggest> RecognizeAsync(string query);
    }
}
