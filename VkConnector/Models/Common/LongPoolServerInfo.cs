using Newtonsoft.Json;
using System;

namespace VkConnector.Models.Common
{
    public class LongPoolServerInfo
    {
        [JsonProperty("key")]
        public string Key { get; set; }
        [JsonProperty("server")]
        public Uri Address { get; set; }
        [JsonProperty("ts")]
        public int IdLastMessage { get; set; }
    }
}
