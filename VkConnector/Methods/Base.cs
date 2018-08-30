using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace VkConnector.Methods
{
    public class Base
    {
        public const string VkUrl = "https://vk.com";

        public const string SupportedApiVersion = "5.80";
        private const string VK_API_URL = "https://api.vk.com/method";

        private static string Encode(string data) => string.IsNullOrEmpty(data) ? string.Empty : WebUtility.UrlEncode(data).Replace("%20", "+");

        public static async Task<string> SendCommand(string Command, string accessToken = null, Dictionary<string, object> parameters = null, Dictionary<string, object> Body = null)
        {
            string VK_REST_URI = $"{VK_API_URL}/{Command}?access_token={accessToken}&v={SupportedApiVersion}";

            if (parameters != null) //Проверяем возможное указание параметров
                foreach (KeyValuePair<string, object> parameter in parameters)
                    VK_REST_URI += $"&{parameter.Key}={parameter.Value}";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

                FormUrlEncodedContent formContent = Body == null ? null : new FormUrlEncodedContent(Body.Select(x => new KeyValuePair<string, string>(x.Key, x.Value?.ToString())));
                HttpResponseMessage response = await client.PostAsync(VK_REST_URI, formContent);
                string result = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Trace.WriteLine($"Url: {VK_API_URL}\nResponse: {result}");

                return result;
            }
        }
    }
}
