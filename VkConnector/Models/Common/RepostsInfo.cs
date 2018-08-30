using Newtonsoft.Json;

namespace VkConnector.Models.Common
{
    public class RepostsInfo
    {
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("user_reposted")]
        public bool IsCurrentUserReposted { get; set; }
    }
}
