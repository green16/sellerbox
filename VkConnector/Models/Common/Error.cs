using Newtonsoft.Json;

namespace VkConnector.Models.Common
{
    public class Error
    {
        [JsonProperty("error")]
        public string Type { get; set; }
        [JsonProperty("error_description")]
        public string Description { get; set; }
    }
}
