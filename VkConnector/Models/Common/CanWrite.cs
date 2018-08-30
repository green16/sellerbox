using Newtonsoft.Json;

namespace VkConnector.Models.Common
{
    public class CanWrite
    {
        [JsonProperty("allowed")]
        public bool IsAllowed { get; set; }
        [JsonProperty("reason")]
        public int BlockReason { get; set; }
    }
}
