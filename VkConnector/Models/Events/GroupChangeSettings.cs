using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace VkConnector.Events
{
    public class GroupChangeSettings
    {
        [JsonProperty("user_id")]
        public int IdUser { get; set; }
        [JsonProperty("changes")]
        public Dictionary<string, JObject> Changes { get; set; }
        [JsonProperty("old_value")]
        public JObject ValueOld { get; set; }
        [JsonProperty("new_value")]
        public JObject ValueNew { get; set; }
    }
}
