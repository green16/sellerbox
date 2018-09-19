using System;

namespace WebApplication1.Common.Helpers
{
    public static class VkHelper
    {
        public static DateTime? BirtdayConvert(string birthday)
        {
            if (string.IsNullOrWhiteSpace(birthday))
                return null;
            string[] dateParts = birthday.Split('.');
            if (DateTime.TryParseExact(birthday, new string[] { "d.M.yyyy", "d.M" }, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime result))
            {
                if (dateParts.Length == 2 && result.Year == DateTime.UtcNow.Year)
                    return result.AddYears(-result.Year + 1);
                return result;
            }
            return null;
        }
    }
}
