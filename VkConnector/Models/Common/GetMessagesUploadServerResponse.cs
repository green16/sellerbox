using Newtonsoft.Json;
using System;

namespace VkConnector.Models.Common
{
    public class GetMessagesUploadServerResponse
    {
        [JsonProperty("upload_url")]
        public Uri UploadUrl { get; set; }
        [JsonProperty("album_id")]
        public int IdAlbum { get; set; }
        [JsonProperty("group_id")]
        public int IdGroup { get; set; }
    }
}
