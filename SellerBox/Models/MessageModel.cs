using Newtonsoft.Json;

namespace SellerBox.Models
{
    public class MessageModel
    {
        [JsonProperty("idGroup")]
        public int IdGroup { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("idFiles")]
        public string[] IdFiles { get; set; }
        [JsonProperty("isImageFirst")]
        public bool IsImageFirst { get; set; }
    }
}
