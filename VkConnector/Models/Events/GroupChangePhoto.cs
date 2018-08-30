using Newtonsoft.Json;
using VkConnector.Models.Objects;

namespace VkConnector.Events
{
    public class GroupChangePhoto
    {
        [JsonProperty("user_id")]
        public int IdUser { get; set; }
        [JsonProperty("photo")]
        public Photo Photo { get; set; }
    }
}
