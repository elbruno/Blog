namespace Microsoft.ProjectOxford.Search.Contract
{
    public class Search
    {
        public string _type { get; set; }
        public WebPages webPages { get; set; }
        public Images images { get; set; }
        public RankingResponse rankingResponse { get; set; }
    }
}