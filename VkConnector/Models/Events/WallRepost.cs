using Newtonsoft.Json;
using VkConnector.Models.Objects;

namespace VkConnector.Events
{
    public class WallRepost : Post
    {
        [JsonProperty("postponed_id")]
        public int IdPostponed { get; set; } //идентификатор отложенной записи
    }
}
