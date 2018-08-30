using Newtonsoft.Json;
using VkConnector.Models.Objects;

namespace VkConnector.Events
{
    public class PhotoCommentNew : Comment
    {
        [JsonProperty("photo_id")]
        public int IdPhoto { get; set; }
        [JsonProperty("photo_owner_id")]
        public int IdPhotoOwner { get; set; }
    }
}
