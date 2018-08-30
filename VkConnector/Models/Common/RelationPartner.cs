using Newtonsoft.Json;

namespace VkConnector.Models.Common
{
    public class RelationPartner
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
