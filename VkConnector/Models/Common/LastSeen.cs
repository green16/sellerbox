using Newtonsoft.Json;
using System;
using VkConnector.Common.Converters;

namespace VkConnector.Models.Common
{
    public class LastSeen
    {
        [JsonProperty("time"), JsonConverter(typeof(UnixTimeConverter))]
        public DateTime Time { get; set; }
        [JsonProperty("platform")]
        public Platform Platform { get; set; }
    }
}
