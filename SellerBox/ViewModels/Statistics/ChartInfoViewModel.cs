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
            public int[] Data { get; set; }

            public ChartData(string label, string backgroundColor, params int[] data) 
                : this(label, null, backgroundColor, data) { }

            public ChartData(string label, string stack, string backgroundColor, params int[] data)
            {
                BackgroundColor = backgroundColor;
                Data = data;
                Label = label;
                Stack = stack;
            }
        }

        public class ChartLegend
        {
            public string Text { get; set; }
            public string Color { get; set; }
            public long Value { get; set; }

            public ChartLegend(string text, string color, long value)
            {
                Text = text;
                Color = color;
                Value = value;
            }
        }

        public string Title { get; set; }

        public string[] YLabels { get; set; }
        public ChartData[] Dataset { get; set; }

        public ChartLegend[] Legend { get; set; }
    }
}
