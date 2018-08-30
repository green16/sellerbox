using Newtonsoft.Json;

namespace VkConnector.Events
{
    public class MessageAllow
    {
        [JsonProperty("user_id")]
        public int IdUser { get; set; }
        [JsonProperty("key")]
        public string Key { get; set; }
    }
}
