using Newtonsoft.Json;

namespace WebApplication1.Models.Vk
{
    public class GroupLeave
    {
        [JsonProperty("user_id")]
        public long IdUser { get; set; }
        [JsonProperty("self")]
        public bool IsSelf { get; set; }
    }
}
