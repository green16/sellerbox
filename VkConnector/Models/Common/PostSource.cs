using Newtonsoft.Json;
using System;

namespace VkConnector.Models.Common
{
    public class PostSource
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("platform")]
        public string Platform { get; set; }
        [JsonProperty("data")]
        public string ActionType { get; set; }
        [JsonProperty("url")]
        public Uri PublisherUrl { get; set; }
    }
}
