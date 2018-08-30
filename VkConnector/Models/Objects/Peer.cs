using Newtonsoft.Json;

namespace VkConnector.Models.Objects
{
    public class Peer
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("local_id")]
        public int IdLocal { get; set; }
    }
}
