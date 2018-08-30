using Newtonsoft.Json;
using System.Collections.Generic;

namespace VkConnector.Models.Common
{
    public class GetObjectsResponse<T>
    {
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("items")]
        public IEnumerable<T> Items { get; set; }
    }
}
