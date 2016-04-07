using System.Collections.Generic;

namespace Microsoft.ProjectOxford.Search.Contract
{
    public class Images
    {
        public string id { get; set; }
        public string readLink { get; set; }
        public string webSearchUrl { get; set; }
        public bool isFamilyFriendly { get; set; }
        public List<Value2> value { get; set; }
        public bool displayShoppingSourcesBadges { get; set; }
        public bool displayRecipeSourcesBadges { get; set; }
    }
}