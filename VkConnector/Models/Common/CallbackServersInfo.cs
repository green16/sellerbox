using Newtonsoft.Json;

namespace VkConnector.Models.Common
{
    public class CallbackServersInfo
    {
        [JsonProperty("id")]
        public int IdServer { get; set; }
        [JsonProperty("title")]
        public string Name { get; set; }
        [JsonProperty("creator_id")]
        public int IdCreator { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("secret_key")]
        public string SecretKey { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
