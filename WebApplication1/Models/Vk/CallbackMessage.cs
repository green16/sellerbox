﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace WebApplication1.Models.Vk
{
    public class CallbackMessage : IEquatable<CallbackMessage>, ICloneable
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("group_id")]
        public long IdGroup { get; set; }
        [JsonProperty("object")]
        public JObject Object { get; set; }

        public object Clone()
        {
            string json = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<CallbackMessage>(json);
        }

        public bool Equals(CallbackMessage other) => Type == other.Type && IdGroup == other.IdGroup && Object.ToString(Formatting.None) == other.Object.ToString(Formatting.None);

        public override bool Equals(object obj)
        {
            if (!(obj is CallbackMessage item))
                return false;

            return Equals(item);
        }

        public override int GetHashCode()
        {
            return Object.GetHashCode();
        }
    }
}
