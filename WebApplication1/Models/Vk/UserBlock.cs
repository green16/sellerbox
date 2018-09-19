using Newtonsoft.Json;

namespace WebApplication1.Models.Vk
{
    public class UserBlock
    {
        [JsonProperty("user_id")]
        public long IdUser { get; set; }
        [JsonProperty("comment")]
        public string Comment { get; set; }
    }
}
