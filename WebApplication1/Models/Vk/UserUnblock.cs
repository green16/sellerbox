using Newtonsoft.Json;

namespace WebApplication1.Models.Vk
{
    public class UserUnblock
    {
        [JsonProperty("admin_id")]
        public int? IdAdmin { get; set; }
        [JsonProperty("user_id")]
        public int IdUser { get; set; }
        [JsonProperty("by_end_date")]
        public bool IsByEndDate { get; set; }
    }
}
