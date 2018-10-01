using Newtonsoft.Json;

namespace SellerBox.Models.Vk
{
    public class GroupJoin
    {
        [JsonProperty("user_id")]
        public long? IdUser { get; set; }
        [JsonProperty("join_type")]
        public string Type { get; set; }
    }
}
