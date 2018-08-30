using Newtonsoft.Json;
using VkConnector.Models.Objects;

namespace VkConnector.Events
{
    public class VideoCommentNew : Comment
    {
        [JsonProperty("video_id")]
        public int IdVideo { get; set; }
        [JsonProperty("video_owner_id ")]
        public int IdVideoOwner { get; set; }
    }
}
