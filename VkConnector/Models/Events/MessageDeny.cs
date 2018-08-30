using Newtonsoft.Json;

namespace VkConnector.Events
{
    public class MessageDeny
    {
        [JsonProperty("user_id")]
        public int IdUser { get; set; }
    }
}
