using Newtonsoft.Json;

namespace VkConnector.Models.Common
{
    public class Military
    {
        [JsonProperty("unit")]
        public int IdUnit { get; set; }
        [JsonProperty("unit_id")]
        public string Unit { get; set; }
        [JsonProperty("country_id")]
        public int IdCountry { get; set; }
        [JsonProperty("from")]
        public int YearFrom { get; set; }
        [JsonProperty("until")]
        public int YearUntil { get; set; }
    }
}
