using Newtonsoft.Json;

namespace VkConnector.Models.Common
{
    public class SendMessageResult
    {
        [JsonProperty("peer_id")]
        public int IdPeer { get; set; }
        [JsonProperty("message_id")]
        public int IdMessage { get; set; }
        [JsonProperty("error")]
        public string Error { get; set; }
    }
}
