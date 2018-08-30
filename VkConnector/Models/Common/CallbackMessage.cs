using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VkConnector.Models.Common
{
    public class CallbackMessage
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("group_id")]
        public int IdGroup { get; set; }
        [JsonProperty("object")]
        public JObject Object { get; set; }
    }
}
