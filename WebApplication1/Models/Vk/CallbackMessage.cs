using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebApplication1.Models.Vk
{
    public class CallbackMessage
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("group_id")]
        public long IdGroup { get; set; }
        [JsonProperty("object")]
        public JObject Object { get; set; }
    }
}
