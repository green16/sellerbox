using Newtonsoft.Json;

namespace SellerBox.ViewModels.Statistics
{
    public class ChartInfoViewModel
    {
        [System.Serializable]
        public class ChartData
        {
            [JsonProperty("label")]
            public string Label { get; set; }
            [JsonProperty("stack")]
            public string Stack { get; set; }
            [JsonProperty("backgroundColor")]
            public string BackgroundColor { get; set; }
            [JsonProperty("data")]
            public long[] Data { get; set; }
        }

        public class ChartLegend
        {
            public string Text { get; set; }
            public string Color { get; set; }
            public long Value { get; set; }
        }

        public string Title { get; set; }

        public string[] YLabels { get; set; }
        public ChartData[] Dataset { get; set; }

        public ChartLegend[] Legend { get; set; }
    }
}
