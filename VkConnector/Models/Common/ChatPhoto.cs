using Newtonsoft.Json;
using System;

namespace VkConnector.Models.Common
{
    public class ChatPhoto
    {
        [JsonProperty("photo_50")]
        public Uri Photo50 { get; set; }
        [JsonProperty("photo_100")]
        public Uri Photo100 { get; set; }
        [JsonProperty("photo_200")]
        public Uri Photo200 { get; set; }
    }
}
