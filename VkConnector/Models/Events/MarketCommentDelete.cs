using Newtonsoft.Json;

namespace VkConnector.Events
{
    public class MarketCommentDelete
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("owner_id")]
        public int IdOwner { get; set; }
        [JsonProperty("user_id")]
        public int IdUser { get; set; }
        [JsonProperty("deleter_id")]
        public int IdDeleter { get; set; }
        [JsonProperty("item_id")]
        public int IdItem { get; set; }
    }
}
