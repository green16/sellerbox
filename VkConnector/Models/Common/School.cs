using Newtonsoft.Json;

namespace VkConnector.Models.Common
{
    public class School
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("country")]
        public int IdCountry { get; set; }
        [JsonProperty("city")]
        public int IdCity { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("year_from")]
        public int YearFrom { get; set; }
        [JsonProperty("year_to")]
        public int YearTo { get; set; }
        [JsonProperty("year_graduated")]
        public int YearGraduated { get; set; }
        [JsonProperty("class")]
        public string Class { get; set; }
        [JsonProperty("speciality")]
        public string Speciality { get; set; }
        [JsonProperty("type")]
        public SchoolType Type { get; set; }
        [JsonProperty("type_str")]
        public string TypeName { get; set; }
    }
}
