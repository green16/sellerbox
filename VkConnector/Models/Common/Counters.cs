using Newtonsoft.Json;

namespace VkConnector.Models.Common
{
    public class Counters
    {
        [JsonProperty("albums")]
        public int AlbumsCount { get; set; }
        [JsonProperty("videos")]
        public int VideosCount { get; set; }
        [JsonProperty("audios")]
        public int AudiosCount { get; set; }
        [JsonProperty("photos")]
        public int PhotosCount { get; set; }
        [JsonProperty("notes")]
        public int NotesCount { get; set; }
        [JsonProperty("friends")]
        public int FriendsCount { get; set; }
        [JsonProperty("groups")]
        public int GroupsCount { get; set; }
        [JsonProperty("online_friends")]
        public int OnlineFriendsCount { get; set; }
        [JsonProperty("mutual_friends")]
        public int CommonFirendsCount { get; set; }
        [JsonProperty("user_videos")]
        public int VideosWithUserCount { get; set; }
        [JsonProperty("followers")]
        public int FollowersCount { get; set; }
        [JsonProperty("pages")]
        public int InterestingPagesCount { get; set; }
    }
}
