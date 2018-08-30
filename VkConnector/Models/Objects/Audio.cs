using Newtonsoft.Json;
using System;
using VkConnector.Common.Converters;
using VkConnector.Models.Common;

namespace VkConnector.Models.Objects
{
    public class Audio
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("owner_id")]
        public int IdOwner { get; set; }
        [JsonProperty("artist")]
        public string Artist { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("duration")]
        public int Duration { get; set; }
        [JsonProperty("url")]
        public Uri Url { get; set; }
        [JsonProperty("lyrics_id")]
        public int? IdLyrics { get; set; }
        [JsonProperty("album_id")]
        public int? IdAlbum { get; set; }
        [JsonProperty("genre_id")]
        public AudioGenres IdGenre { get; set; }
        [JsonProperty("date"), JsonConverter(typeof(UnixTimeConverter))]
        public DateTime DtAdd { get; set; }
        [JsonProperty("no_search")]
        public bool NoSearch { get; set; }
        [JsonProperty("is_hq")]
        public bool IsHighQuality { get; set; }
    }
}
