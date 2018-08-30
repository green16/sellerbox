using Newtonsoft.Json;

namespace VkConnector.Models.Common
{
    public class Contacts
    {
        [JsonProperty("mobile_phone")]
        public string Mobile { get; set; }
        [JsonProperty("home_phone")]
        public string Home { get; set; }
    }
}
