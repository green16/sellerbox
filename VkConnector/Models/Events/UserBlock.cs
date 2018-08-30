using Newtonsoft.Json;
using System;
using VkConnector.Common.Converters;
using VkConnector.Models.Common;

namespace VkConnector.Events
{
    public class UserBlock
    {
        [JsonProperty("admin_id")]
        public int? IdAdmin { get; set; }
        [JsonProperty("user_id")]
        public int IdUser { get; set; }
        [JsonProperty("unblock_date"), JsonConverter(typeof(UnixTimeConverter))]
        public DateTime DtUnblock { get; set; }
        [JsonProperty("reason")]
        public BlockReason BlockReason { get; set; }
        [JsonProperty("comment")]
        public string Comment { get; set; }
    }
}
