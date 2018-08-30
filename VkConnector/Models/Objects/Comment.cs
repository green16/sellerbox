using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using VkConnector.Common.Converters;

namespace VkConnector.Models.Objects
{
    public class Comment
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("from_id")]
        public int IdAuthor { get; set; }
        [JsonProperty("date"), JsonConverter(typeof(UnixTimeConverter))]
        public DateTime Dt { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("reply_to_user")]
        public int IdReplyUser { get; set; }
        [JsonProperty("reply_to_comment")]
        public int IdReplyComment { get; set; }
        [JsonProperty("attachments")]
        public IEnumerable<Attachment> Attachments { get; set; }
    }
}
