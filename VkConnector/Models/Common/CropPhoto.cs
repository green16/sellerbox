using Newtonsoft.Json;
using VkConnector.Models.Objects;

namespace VkConnector.Models.Common
{
    public class CropPhoto
    {
        public class Rectangle
        {
            [JsonProperty("x")]
            public float X { get; set; }
            [JsonProperty("y")]
            public float Y { get; set; }
            [JsonProperty("x2")]
            public float X2 { get; set; }
            [JsonProperty("y2")]
            public float Y2 { get; set; }
        }
        [JsonProperty("photo")]
        public Photo Photo { get; set; }
        [JsonProperty("crop")]
        public Rectangle Crop { get; set; }
        [JsonProperty("rect")]
        public Rectangle Rect { get; set; }
    }
}
