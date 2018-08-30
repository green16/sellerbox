using Newtonsoft.Json;

namespace VkConnector.Models.Common
{
    public class University
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("country")]
        public int IdCountry { get; set; }
        [JsonProperty("city")]
        public int IdCity { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("faculty")]
        public int IdFaculity { get; set; }
        [JsonProperty("faculty_name")]
        public string Faculity { get; set; }
        [JsonProperty("chair")]
        public int IdChair { get; set; }
        [JsonProperty("chair_name")]
        public string Chair { get; set; }
        [JsonProperty("graduation")]
        public int GraduationYear { get; set; }
        [JsonProperty("education_form")]
        public string Form { get; set; }
        [JsonProperty("education_status")]
        public string Status { get; set; }
    }
}
