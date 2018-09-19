using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VkConnector.Common;
using VkConnector.Models.Common;
using VkConnector.Models.Errors;
using VkConnector.Models.Objects;

namespace VkConnector.Methods
{
    public class Wall : Base
    {
        public static async Task<GetObjectsResponse<Post>> Get(string userAccessToken, int idGroup, int offset = 0, int count = 100)
        {
            string jsonResult = await SendCommand("wall.get", userAccessToken, new Dictionary<string, object>()
            {
                { "owner_id", -idGroup },
                { "offset", offset },
                { "count", count }
            });
            BaseResponse<GetObjectsResponse<Post>> result = JsonConvert.DeserializeObject<BaseResponse<GetObjectsResponse<Post>>>(jsonResult);
            if (JsonConvert.DeserializeObject<BaseError>(jsonResult).Code > 0)
                return null;// Enumerable.Empty<User>();
            return result.Item;
        }
        public static async Task<Post> GetById(string userAccessToken, int idGroup, int idPost)
        {
            string jsonResult = await SendCommand("wall.getById", userAccessToken, new Dictionary<string, object>()
            {
                { "posts", $"{-idGroup}_{idPost}" },
                { "copy_history_depth", 1 }
            });
            BaseResponse<Post> result = JsonConvert.DeserializeObject<BaseResponse<Post>>(jsonResult);
            if (JsonConvert.DeserializeObject<BaseError>(jsonResult).Code > 0)
                return null;// Enumerable.Empty<User>();
            return result.Item;
        }

        public static async Task<int> Post(string accessToken, int idGroup, string message, IEnumerable<string> attachments)
        {
            var baseParameters = new Dictionary<string, object>()
            {
                { "owner_id", -idGroup }
            };
            if (!string.IsNullOrEmpty(message))
                baseParameters.Add("message", message);
            if (attachments != null && attachments.Any())
                baseParameters.Add("attachments", string.Join(',', attachments));

            string jsonResult = await SendCommand("wall.post", accessToken, null, baseParameters);
            if (JsonConvert.DeserializeObject<BaseError>(jsonResult).Code > 0)
                return -1;// Enumerable.Empty<User>();

            BaseResponse<JObject> result = JsonConvert.DeserializeObject<BaseResponse<JObject>>(jsonResult);
            return result.Item.Value<int>("post_id");
        }
    }
}
