using Newtonsoft.Json;

namespace VkConnector.Models.Common
{
    public class Carrer
    {
        [JsonProperty("group_id")]
        public int? IdGroup { get; set; }
        [JsonProperty("company")]
        public string Company { get; set; }
        [JsonProperty("country_id")]
        public int IdCountry { get; set; }
        [JsonProperty("city_id")]
        public int? IdCity { get; set; }
        [JsonProperty("city_name")]
        public string City { get; set; }
        [JsonProperty("from")]
        public int YearStart { get; set; }
        [JsonProperty("until")]
        public int? YearEnd { get; set; }
        [JsonProperty("position")]
        public string Postiton { get; set; }
    }
}
