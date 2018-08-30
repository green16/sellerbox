using Newtonsoft.Json;

namespace VkConnector.Events
{
    public class GroupLeave
    {
        [JsonProperty("user_id")]
        public int IdUser { get; set; }
        [JsonProperty("self")]
        public bool IsSelf { get; set; }
    }
}
