using Newtonsoft.Json;

namespace VkConnector.Models.Common
{
    public class CommentsInfo
    {
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("can_post")]
        public bool CanCurrentUserComment { get; set; } //может ли текущий пользователь комментировать запись
        [JsonProperty("groups_can_post")]
        public bool CanGroupsComment { get; set; } //могут ли сообщества комментировать запись
    }
}
