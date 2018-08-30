using Newtonsoft.Json;

namespace VkConnector.Models.Common
{
    public class Employment
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
