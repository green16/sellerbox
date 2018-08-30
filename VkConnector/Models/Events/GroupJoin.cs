using Newtonsoft.Json;

namespace VkConnector.Events
{
    public class GroupJoin
    {
        [JsonProperty("user_id")]
        public int IdUser { get; set; }
        [JsonProperty("join_type")]
        public string Type { get; set; }
    }
}
