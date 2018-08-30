using Newtonsoft.Json;
using System.Collections.Generic;
using VkConnector.Models.Objects;

namespace VkConnector.Models.Common
{
    public class ChatSettings
    {
        [JsonProperty("members_count")]
        public int MembersCount { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("pinned_message")]
        public Message PinnedMessage { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("photo")]
        public ChatPhoto ChatPhoto { get; set; }
        [JsonProperty("active_ids")]
        public IEnumerable<int> ActiveMembersIds { get; set; }
    }
}
