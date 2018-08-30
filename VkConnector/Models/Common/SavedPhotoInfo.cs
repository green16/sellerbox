using Newtonsoft.Json;
using System.Collections.Generic;
using VkConnector.Models.Objects;

namespace VkConnector.Models.Common
{
    public class SavedPhotoInfo : Photo
    {
        [JsonIgnore]
        private new IEnumerable<PhotoSize> PhotoSizes { get; }
        [JsonProperty("access_key")]
        public string AccessKey { get; set; }
    }
}
