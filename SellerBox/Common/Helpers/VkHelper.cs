using System;

namespace SellerBox.Common.Helpers
{
    public static class VkHelper
    {
        public static DateTime? BirtdayConvert(string birthday)
        {
            if (string.IsNullOrWhiteSpace(birthday))
                return null;
            string[] dateParts = birthday.Split('.');
            if (dateParts.Length < 2)
                return null;

            if (DateTime.TryParseExact(birthday, new string[] { "d.M.yyyy", "d.M", "dd.MM" }, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime result))
            {
                if (dateParts.Length == 2 && result.Year == 1)
                    return result.AddYears(-result.Year + 1);
                return result;
            }
            return null;
        }
    }
}
