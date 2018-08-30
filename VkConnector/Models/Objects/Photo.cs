using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using VkConnector.Common.Converters;
using VkConnector.Models.Common;

namespace VkConnector.Models.Objects
{
    public class Photo
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("album_id")]
        public int IdAlbum { get; set; }
        [JsonProperty("owner_id")]
        public int IdOwner { get; set; }
        [JsonProperty("user_id")]
        public int IdUser { get; set; }
        [JsonProperty("text")]
        public string Description { get; set; }
        [JsonProperty("date"), JsonConverter(typeof(UnixTimeConverter))]
        public DateTime Dt { get; set; }
        [JsonProperty("sizes")]
        public IEnumerable<PhotoSize> PhotoSizes { get; set; }
        [JsonProperty("photo_75")]
        public Uri Photo75 { get; set; }
        [JsonProperty("photo_130")]
        public Uri Photo130 { get; set; }
        [JsonProperty("photo_604")]
        public Uri Photo604 { get; set; }
        [JsonProperty("photo_807")]
        public Uri Photo807 { get; set; }
        [JsonProperty("photo_1280")]
        public Uri Photo1280 { get; set; }
        [JsonProperty("photo_2560")]
        public Uri Photo2560 { get; set; }
        [JsonProperty("width")]
        public int? Width { get; set; }
        [JsonProperty("height")]
        public int? Height { get; set; }
    }
}
