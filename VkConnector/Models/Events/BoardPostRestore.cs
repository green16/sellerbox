using Newtonsoft.Json;
using VkConnector.Models.Objects;

namespace VkConnector.Events
{
    public class BoardPostRestore : BoardComment
    {
        [JsonProperty("topic_id")]
        public int IdTopic { get; set; }
        [JsonProperty("topic_owner_id")]
        public int IdTopicOwner { get; set; }
    }
}
