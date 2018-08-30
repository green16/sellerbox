using Newtonsoft.Json;
using System.Collections.Generic;

namespace VkConnector.Common
{
    internal class BaseResponse<T>
    {
        [JsonProperty("response")]
        public T Item { get; set; }
    }
}
