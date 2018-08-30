using Newtonsoft.Json;
using VkConnector.Models.Objects;

namespace VkConnector.Events
{
    public class MarketCommentNew : Comment
    {
        [JsonProperty("item_id")]
        public int IdItem { get; set; }
        [JsonProperty("market_owner_id")]
        public int IdMarketOwner { get; set; }
    }
}
