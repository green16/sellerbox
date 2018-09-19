using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VkConnector.Common;
using VkConnector.Models.Common;
using VkConnector.Models.Errors;

namespace VkConnector.Methods
{
    public class Messages : Base
    {
        public static async Task Send(string groupAccessToken, string text, IEnumerable<string> attachments, Keyboard keyboard, ICollection<int> userIds)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "user_ids", string.Join(',', userIds) }
            };
            if (!string.IsNullOrEmpty(text))
                parameters.Add("message", text);
            if (attachments != null && attachments.Any())
                parameters.Add("attachment", string.Join(',', attachments));
            if (keyboard != null)
                parameters.Add("keyboard", JsonConvert.SerializeObject(keyboard));

            string jsonResult = await SendCommand("messages.send", groupAccessToken, null, parameters);

            if (JsonConvert.DeserializeObject<BaseError>(jsonResult).Code > 0) { }
        }

        public static async Task<bool> IsMessagesFromGroupAllowed(string groupAccessToken, int idGroup, int idUser)
        {
            string jsonResult = await SendCommand("messages.isMessagesFromGroupAllowed", groupAccessToken, new Dictionary<string, object>()
            {
                { "group_id", idGroup },
                { "user_id", idUser }
            });
            if (JsonConvert.DeserializeObject<BaseError>(jsonResult).Code > 0)
                return false;

            BaseResponse<JObject> result = JsonConvert.DeserializeObject<BaseResponse<JObject>>(jsonResult);
            return result.Item.Value<int>("is_allowed") == 1;
        }

        public static async Task<GetConversationsResponse> GetConversations(string groupAccessToken, int idGroup, string additionalFields, int offset, int count = 20)
        {
            string jsonResult = await SendCommand("messages.getConversations", groupAccessToken, new Dictionary<string, object>()
            {
                { "group_id", idGroup },
                { "extended", 1},
                { "fields", additionalFields },
                { "offset", offset },
                { "count", count }
            });
            if (JsonConvert.DeserializeObject<BaseError>(jsonResult).Code > 0)
                return null;

            return JsonConvert.DeserializeObject<BaseResponse<GetConversationsResponse>>(jsonResult)?.Item;
        }

        public static async Task MarkAsRead(string groupAccessToken, int idGroup, int idUser)
        {
            string jsonResult = await SendCommand("messages.markAsRead", groupAccessToken, new Dictionary<string, object>()
            {
                { "group_id", idGroup },
                { "peer_id", idUser }
            });
            if (JsonConvert.DeserializeObject<BaseError>(jsonResult).Code > 0)
                return;
        }
    }
}
