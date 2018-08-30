using Newtonsoft.Json;
using VkConnector.Models.Objects;

namespace VkConnector.Events
{
    public class WallReplyNew : Comment
    {
        [JsonProperty("post_id")]
        public int IdPost { get; set; }
        [JsonProperty("post_owner_id")]
        public int IdPostOwner { get; set; }
    }
}
