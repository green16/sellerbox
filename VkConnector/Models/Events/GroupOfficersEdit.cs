using Newtonsoft.Json;
using VkConnector.Models.Common;

namespace VkConnector.Events
{
    public class GroupOfficersEdit
    {
        [JsonProperty("admin_id")]
        public int IdAdmin { get; set; }
        [JsonProperty("user_id")]
        public int IdUser { get; set; }
        [JsonProperty("level_old")]
        public Levels LevelOld { get; set; }
        [JsonProperty("level_new")]
        public Levels LevelNew { get; set; }
    }
}
