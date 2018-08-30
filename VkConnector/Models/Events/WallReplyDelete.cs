using Newtonsoft.Json;

namespace VkConnector.Events
{
    public class WallReplyDelete
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("owner_id")]
        public int IdOwner { get; set; }
        [JsonProperty("deleter_id")]
        public int IdDeleter { get; set; }
        [JsonProperty("post_id")]
        public int IdPost { get; set; }
    }
}
