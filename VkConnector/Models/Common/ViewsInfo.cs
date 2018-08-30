using Newtonsoft.Json;

namespace VkConnector.Models.Common
{
    public class ViewsInfo
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
