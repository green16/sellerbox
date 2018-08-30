using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace VkConnector.Models.Objects
{
    public class Group
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("screen_name")]
        public string ScreenName { get; set; }
        [JsonProperty("is_closed")]
        public int IsClosed { get; set; }
        [JsonProperty("deactivated")]
        public string Deactivated { get; set; }
        [JsonProperty("is_admin")]
        public bool IsAdmin { get; set; }
        [JsonProperty("admin_level")]
        public int AdminLevel { get; set; }
        [JsonProperty("is_member")]
        public bool IsMember { get; set; }
        [JsonProperty("invited_by")]
        public int InvitedBy { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("photo_50")]
        public Uri PhotoSquare50 { get; set; }
        [JsonProperty("photo_100")]
        public Uri PhotoSquare100 { get; set; }
        [JsonProperty("photo_200")]
        public Uri PhotoSquare200 { get; set; }
    }
}
