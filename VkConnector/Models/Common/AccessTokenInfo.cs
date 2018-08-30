using Newtonsoft.Json;

namespace VkConnector.Models.Common
{
    public class AccessTokenInfo
    {
        [JsonProperty("access_token")]
        public string Token { get; set; }
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
        [JsonProperty("user_id")]
        public int IdUser { get; set; }
        [JsonProperty("error")]
        public string Error { get; set; }
        [JsonProperty("error_description")]
        public string ErrorText { get; set; }

        [JsonIgnore]
        public bool HasError => !string.IsNullOrWhiteSpace(Error);
    }
}
