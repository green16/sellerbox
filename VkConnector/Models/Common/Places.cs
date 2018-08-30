using Newtonsoft.Json;
using System;
using VkConnector.Common.Converters;

namespace VkConnector
{
    public class Places
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("latitude")]
        public double Latitude { get; set; }
        [JsonProperty("longitude")]
        public double Longitude { get; set; }
        [JsonProperty("created"), JsonConverter(typeof(UnixTimeConverter))]
        public DateTime Created { get; set; }
        [JsonProperty("icon")]
        public Uri Icon { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }

        //Если место добавлено как чекин в сообщество

        [JsonProperty("type")]
        public int CheckInType { get; set; }
        [JsonProperty("group_id")]
        public int IdGroup { get; set; }
        [JsonProperty("group_photo")]
        public Uri GroupPhotoUrl { get; set; }
        [JsonProperty("checkins")]
        public int CheckInsCount { get; set; }
        [JsonProperty("updated"), JsonConverter(typeof(UnixTimeConverter))]
        public DateTime DtLastUpdate { get; set; }
        [JsonProperty("address")]
        public int Address { get; set; }
    }
}