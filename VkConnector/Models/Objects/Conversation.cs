using Newtonsoft.Json;
using VkConnector.Models.Common;

namespace VkConnector.Models.Objects
{
    public class Conversation
    {
        [JsonProperty("peer")]
        public Peer Peer { get; set; }
        [JsonProperty("in_read")]
        public int IdLastReadedInputMessage { get; set; }
        [JsonProperty("out_read")]
        public int IdLastReadedOutputMessage { get; set; }
        [JsonProperty("unread_count")]
        public int UnreadMessagesCount { get; set; }
        [JsonProperty("important")]
        public bool IsImportant { get; set; }
        [JsonProperty("unanswered")]
        public bool IsUnanswered { get; set; }
        [JsonProperty("push_settings")]
        public object PushSettings { get; set; }
        [JsonProperty("can_write")]
        public CanWrite CanWrite { get; set; }
        [JsonProperty("chat_settings")]
        public ChatSettings ChatSettings { get; set; }
    }
}
