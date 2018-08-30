using Newtonsoft.Json;
using System;

namespace VkConnector.Common.Converters
{
    public class BirthdayConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string value = reader.Value as string;
            if (string.IsNullOrWhiteSpace(value))
                return null;
            string[] dateParts = value.Split('.');
            if (DateTime.TryParseExact(value, new string[] { "d.M.yyyy", "d.M" }, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime result))
            {
                if (dateParts.Length == 2 && result.Year == DateTime.UtcNow.Year)
                    return result.AddYears(-result.Year + 1);
                return result;
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }
    }
}
