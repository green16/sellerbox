using Newtonsoft.Json;

namespace VkConnector.Models.Common
{
    public class Relatives
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
