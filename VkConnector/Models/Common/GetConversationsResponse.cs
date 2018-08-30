using Newtonsoft.Json;
using System.Collections.Generic;
using VkConnector.Models.Objects;

namespace VkConnector.Models.Common
{
    public class GetConversationsResponse
    {
        public class GetConversationsResponseItem
        {
            [JsonProperty("conversation")]
            public Conversation Conversation { get; set; }
            [JsonProperty("last_message")]
            public Message LastMessage { get; set; }
        }
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("items")]
        public IEnumerable<GetConversationsResponseItem> Items { get; set; }
        [JsonProperty("unread_count")]
        public int UnreadCount { get; set; }
        [JsonProperty("profiles")]
        public IEnumerable<User> Users { get; set; }
        [JsonProperty("groups")]
        public IEnumerable<Group> Groups { get; set; }
    }
}
