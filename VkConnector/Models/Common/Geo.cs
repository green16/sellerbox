using Newtonsoft.Json;

namespace VkConnector.Models.Common
{
    public class Geo
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("coordinates")]
        public string Coordinates { get; set; }
        [JsonProperty("place")]
        public Places Place { get; set; }
    }
}
