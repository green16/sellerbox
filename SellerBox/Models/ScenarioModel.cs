﻿using Newtonsoft.Json;
using SellerBox.Models.Database.Common;

namespace SellerBox.Models
{
    public class ScenarioModel
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("InputMessage")]
        public string InputMessage { get; set; }
        [JsonProperty("IsStrictMatch")]
        public bool IsStrictMatch { get; set; }
        [JsonProperty("Action")]
        public ScenarioActions Action { get; set; }
        [JsonProperty("Message")]
        public MessageModel Message { get; set; }
        [JsonProperty("ErrorMessage")]
        public MessageModel ErrorMessage { get; set; }
        [JsonProperty("IdChain")]
        public string IdChain { get; set; }
    }
}