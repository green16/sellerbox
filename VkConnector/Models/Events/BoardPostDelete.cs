using Newtonsoft.Json;

namespace VkConnector.Events
{
    public class BoardPostDelete
    {
        [JsonProperty("topic_owner_id")]
        public int Id { get; set; }
        [JsonProperty("topic_id")]
        public int IdTopic { get; set; }
        [JsonProperty("topic_owner_id")]
        public int IdTopicOwner { get; set; }
    }
}
