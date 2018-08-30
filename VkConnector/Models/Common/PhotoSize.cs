using Newtonsoft.Json;
using System;

namespace VkConnector.Models.Common
{
    public class PhotoSize
    {
        [JsonProperty("src")]
        public Uri Source { get; set; }
        [JsonProperty("width")]
        public int Width { get; set; }
        [JsonProperty("height")]
        public int Height { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; } //https://vk.com/dev/objects/photo_sizes
    }
}