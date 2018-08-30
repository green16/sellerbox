using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VkConnector.Models.Common
{
    public class UploadedPhotoInfo
    {
        [JsonProperty("server")]
        public int Server { get; set; }
        [JsonProperty("hash")]
        public string Hash { get; set; }
        [JsonProperty("photo")]
        public JValue Photo { get; set; }
    }
}
