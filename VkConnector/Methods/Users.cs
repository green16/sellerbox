using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VkConnector.Common;
using VkConnector.Models.Errors;
using VkConnector.Models.Objects;

namespace VkConnector.Methods
{
    public class Users : Base
    {
        public static async Task<IEnumerable<User>> Get(string groupAccessToken, params object[] userIds)
        {
            string jsonResult = await SendCommand("users.get", groupAccessToken, new Dictionary<string, object>()
            {
                { "user_ids", string.Join(',', userIds) },
                { "fields", "bdate,city,country,domain,photo_50,photo_400_orig,sex,blacklisted" },
                { "name_case", "nom" }
            });

            System.Diagnostics.Trace.WriteLine(jsonResult);

            if (JsonConvert.DeserializeObject<BaseError>(jsonResult).Code > 0)
                return Enumerable.Empty<User>();

            BaseResponse<IEnumerable<User>> response = JsonConvert.DeserializeObject<BaseResponse<IEnumerable<User>>>(jsonResult);
            return response.Item;
        }
    }
}
