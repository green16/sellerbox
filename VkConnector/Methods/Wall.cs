using Newtonsoft.Json;
using System.Collections.Generic;
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
    }
}
