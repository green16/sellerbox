using Newtonsoft.Json;

namespace VkConnector.Models.Common
{
    public class Country
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("title")]
        public string Name { get; set; }
    }
}
