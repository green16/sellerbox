﻿using Newtonsoft.Json;
using System;
using VkConnector.Common.Converters;

namespace VkConnector.Events
{
    public class MessageNew
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("date"), JsonConverter(typeof(UnixTimeConverter))]
        public DateTime Dt { get; set; }
        [JsonProperty("out")]
        public bool Type { get; set; }
        [JsonProperty("from_id")]
        public int IdUser { get; set; }
        [JsonProperty("read_state")]
        public bool ReadState { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
        //[JsonProperty("payload")]
        //public JValue Payload { get; set; }
    }
}
