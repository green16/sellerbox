using Newtonsoft.Json;

namespace VkConnector.Models.Common
{
    public class MessageActions
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("member_id")]
        public int IdMember { get; set; }
        [JsonProperty("text")]
        public string Title { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("photo")]
        public ChatPhoto Photos { get; set; }
    }
}
