using Newtonsoft.Json;

namespace VkConnector.Models.Common
{
    public class Education
    {
        [JsonProperty("university")]
        public int IdUniversity { get; set; }
        [JsonProperty("university_name")]
        public string University { get; set; }
        [JsonProperty("faculty")]
        public int IdFaculty { get; set; }
        [JsonProperty("faculty_name")]
        public string Faculty { get; set; }
        [JsonProperty("graduation")]
        public int GraduationYear { get; set; }
    }
}
