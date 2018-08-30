using System.Collections.Generic;

namespace VkConnector.Models.Common
{
    public class PersonalInfo
    {
        public PoliticalView PoliticalView { get; set; }
        public IEnumerable<string> Langs { get; set; }
        public string Religion { get; set; }
        public string InspiredBy { get; set; }
        public ImportantInPeople ImportantInPeople { get; set; }
        public MainInLife MainInLife { get; set; }
        public SmokingRatio SmokingRatio { get; set; }
        public AlcoholRatio AlcoholRatio { get; set; }
    }
}
