using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using VkConnector.Common.Converters;
using VkConnector.Models.Common;

namespace VkConnector.Models.Objects
{
    public class Message
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("date"), JsonConverter(typeof(UnixTimeConverter))]
        public DateTime Dt { get; set; }
        [JsonProperty("peer_id")]
        public int IdPeer { get; set; } // Destination
        [JsonProperty("from_id")]
        public int IdFrom { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("random_id")]
        public bool IdRandom { get; set; }
        [JsonProperty("attachments")]
        public IEnumerable<Attachment> Attachments { get; set; }
        [JsonProperty("important")]
        public bool Important { get; set; }
        [JsonProperty("geo")]
        public Geo Geo { get; set; }
        [JsonProperty("payload")]
        public string ServiceField { get; set; }
        [JsonProperty("fwd_messages")]
        public IEnumerable<Message> ForwardedMessages { get; set; }
        [JsonProperty("action")]
        public MessageActions Action { get; set; }
    }
}
