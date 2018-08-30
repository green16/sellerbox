using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using VkConnector.Common.Converters;
using VkConnector.Models.Common;

namespace VkConnector.Models.Objects
{
    public class BoardComment
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("from_id")]
        public int IdAuthor { get; set; }
        [JsonProperty("date"), JsonConverter(typeof(UnixTimeConverter))]
        public DateTime Dt { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("attachments")]
        public IEnumerable<Attachment> Attachments { get; set; }
        [JsonProperty("likes")]
        public LikesInfo LikesInfo { get; set; }
    }
}
