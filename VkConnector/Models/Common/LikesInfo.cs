using Newtonsoft.Json;

namespace VkConnector.Models.Common
{
    public class LikesInfo
    {
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("user_likes")]
        public bool IsCurrentUserLike { get; set; }
        [JsonProperty("can_like")]
        public bool CanCurrentUserLike { get; set; }
        [JsonProperty("can_publish")]
        public bool? CanCurrentUserRepost { get; set; }
    }
}
