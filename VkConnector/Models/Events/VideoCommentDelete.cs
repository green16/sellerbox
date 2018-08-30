using Newtonsoft.Json;

namespace VkConnector.Events
{
    public class VideoCommentDelete
    {
        [JsonProperty("id")]
        public int IdComment { get; set; }
        [JsonProperty("owner_id ")]
        public int IdPhotoOwner { get; set; }
        [JsonProperty("user_id")]
        public int IdCommentAuthor { get; set; }
        [JsonProperty("deleter_id")]
        public int IdDeleter { get; set; }
        [JsonProperty("video_id")]
        public int IdVideo { get; set; }
    }
}
