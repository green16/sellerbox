using Newtonsoft.Json;

namespace VkConnector.Events
{
    public class PollVoteNew
    {
        [JsonProperty("owner_id")]
        public int IdOwner { get; set; }
        [JsonProperty("poll_id")]
        public int IdPoll { get; set; }
        [JsonProperty("option_id")]
        public int IdOption { get; set; }
        [JsonProperty("user_id")]
        public int IdUser { get; set; }
    }
}
