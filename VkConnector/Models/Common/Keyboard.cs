using Newtonsoft.Json;
using System;

namespace VkConnector.Models.Common
{
    [Serializable]
    public class Keyboard
    {
        [Serializable]
        public class ButtonAction
        {
            [JsonProperty("type")]
            public string Type { get; set; } = "text";
            [JsonProperty("payload")]
            public object Payload { get; set; }
            [JsonProperty("label")]
            public string Text { get; set; }
        }
        [Serializable]
        public class Button
        {
            [JsonProperty("color")]
            public string Color { get; set; }
            [JsonProperty("action")]
            public ButtonAction Action { get; set; }
        }

        [JsonProperty("one_time")]
        public bool OneTime { get; set; } = true;
        [JsonProperty("buttons")]
        public Button[][] Buttons { get; set; }

        public string Serialize() => JsonConvert.SerializeObject(this);
        public static Keyboard Deserialize(string json) => (string.IsNullOrWhiteSpace(json)) ? null : JsonConvert.DeserializeObject<Keyboard>(json);
    }
}
