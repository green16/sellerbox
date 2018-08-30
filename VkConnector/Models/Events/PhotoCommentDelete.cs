using Newtonsoft.Json;

namespace VkConnector.Events
{
    public class PhotoCommentDelete
    {
        [JsonProperty("id")]
        public int IdComment { get; set; }
        [JsonProperty("owner_id ")]
        public int IdPhotoOwner { get; set; }
        [JsonProperty("user_id")]
        public int IdCommentAuthor { get; set; }
        [JsonProperty("deleter_id")]
        public int IdDeleter { get; set; }
        [JsonProperty("photo_id")]
        public int IdPhoto { get; set; }
    }
}
