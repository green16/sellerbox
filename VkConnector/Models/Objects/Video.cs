using Newtonsoft.Json;
using System;
using VkConnector.Common.Converters;

namespace VkConnector.Models.Objects
{
    public class Video
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("owner_id")]
        public int IdOwner { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("duration")]
        public int Duration { get; set; }
        [JsonProperty("photo_130")]
        public Uri Photo130 { get; set; }
        [JsonProperty("photo_320")]
        public Uri Photo320 { get; set; }
        [JsonProperty("photo_640")]
        public Uri Photo640 { get; set; }
        [JsonProperty("photo_800")]
        public Uri Photo800 { get; set; }
        [JsonProperty("date"), JsonConverter(typeof(UnixTimeConverter))]
        public DateTime DtCreate { get; set; }
        [JsonProperty("adding_date"), JsonConverter(typeof(UnixTimeConverter))]
        public DateTime DtAdd { get; set; }
        [JsonProperty("views")]
        public int ViewsCount { get; set; }
        [JsonProperty("comments")]
        public int CommentsCount { get; set; }
        [JsonProperty("player")]
        public Uri PlayerUrl { get; set; }
        [JsonProperty("platform")]
        public string VideoHosting { get; set; }
        [JsonProperty("can_edit")]
        public bool CanEdit { get; set; }
        [JsonProperty("can_add")]
        public bool CanAdd { get; set; }
        [JsonProperty("is_private")]
        public bool IsPrivate { get; set; }
        [JsonProperty("access_key")]
        public string AccessKey { get; set; }
        [JsonProperty("processing")]
        public bool IsProcessing { get; set; }
        [JsonProperty("live")]
        public bool IsLiveStream { get; set; }
        [JsonProperty("upcoming")]
        public bool IsUpcomingStream { get; set; }
    }
}
