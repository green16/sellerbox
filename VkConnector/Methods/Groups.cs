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
    public class Groups : Base
    {
        public static async Task<GetObjectsResponse<User>> GetMembers(string groupAccessToken, int idGroup, string fields, int offset = 0, int count = 1000)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>()
            {
                { "group_id", idGroup },
                { "offset", offset },
                { "count", count }
            };
            if (!string.IsNullOrWhiteSpace(fields))
                parameters.Add("fields", fields);

            string jsonResult = await SendCommand("groups.getMembers", groupAccessToken, parameters);

            if (JsonConvert.DeserializeObject<BaseError>(jsonResult).Code > 0)
                return null;

            BaseResponse<GetObjectsResponse<User>> response = JsonConvert.DeserializeObject<BaseResponse<GetObjectsResponse<User>>>(jsonResult);
            return response.Item;
        }

        public static async Task<IEnumerable<Group>> Get(string userAccessToken, int idUser)
        {
            string jsonResult = await SendCommand("groups.get", userAccessToken, new Dictionary<string, object>()
            {
                { "user_id", idUser },
                { "filter", "admin" },
                { "extended", 1 }
            });
            if (JsonConvert.DeserializeObject<BaseError>(jsonResult).Code > 0)
                return Enumerable.Empty<Group>();

            BaseResponse<GetObjectsResponse<Group>> response = JsonConvert.DeserializeObject<BaseResponse<GetObjectsResponse<Group>>>(jsonResult);
            return response.Item.Items;
        }

        public static async Task<Group> GetById(string groupAccessToken, int idGroup)
        {
            string jsonResult = await SendCommand("groups.getById", groupAccessToken, new Dictionary<string, object>()
            {
                { "group_id", idGroup },
                { "fields", "description" }
            });
            if (JsonConvert.DeserializeObject<BaseError>(jsonResult).Code > 0)
                return null;

            BaseResponse<IEnumerable<Group>> response = JsonConvert.DeserializeObject<BaseResponse<IEnumerable<Group>>>(jsonResult);
            return response.Item.FirstOrDefault();
        }

        public static async Task<IEnumerable<CallbackServersInfo>> GetCallbackServers(string groupAccessToken, int idGroup)
        {
            string jsonResult = await SendCommand("groups.getCallbackServers", groupAccessToken, new Dictionary<string, object>()
            {
                { "group_id", idGroup }
            });
            if (JsonConvert.DeserializeObject<BaseError>(jsonResult).Code > 0)
                return Enumerable.Empty<CallbackServersInfo>();

            BaseResponse<GetObjectsResponse<CallbackServersInfo>> response = JsonConvert.DeserializeObject<BaseResponse<GetObjectsResponse<CallbackServersInfo>>>(jsonResult);
            return response.Item.Items;
        }

        public static async Task<int> AddCallbackServer(string groupAccessToken, int idGroup, string serverName, string serverUrl)
        {
            string jsonResult = await SendCommand("groups.addCallbackServer", groupAccessToken, new Dictionary<string, object>()
            {
                { "group_id", idGroup },
                { "url", serverUrl },
                { "title", serverName }
            });
            if (JsonConvert.DeserializeObject<BaseError>(jsonResult).Code > 0)
                return -1;

            JObject result = JObject.Parse(jsonResult);
            return result["response"]["server_id"].Value<int>();
        }

        public static async Task SetCallbackSettings(string groupAccessToken, int idGroup, int idServer)
        {
            string[] flags = new string[] {
                "message_new","message_edit","message_reply","message_allow","message_deny","message_typing_state",
                "photo_new",
                "audio_new",
                "video_new",
                "wall_reply_new","wall_reply_edit","wall_reply_delete","wall_reply_restore","wall_post_new","wall_repost",
                "board_post_new","board_post_edit","board_post_restore","board_post_delete",
                "photo_comment_new","photo_comment_edit","photo_comment_delete","photo_comment_restore",
                "video_comment_new","video_comment_edit","video_comment_delete","video_comment_restore",
                "market_comment_new","market_comment_edit","market_comment_delete","market_comment_restore",
                "poll_vote_new",
                "group_join","group_leave","group_change_settings","group_change_photo",
                "group_officers_edit",
                "user_block","user_unblock",
                "lead_forms_new"
            };
            Dictionary<string, object> parameters = new Dictionary<string, object>()
            {
                { "group_id", idGroup },
                { "server_id", idServer },
                { "api_version", SupportedApiVersion}
            };
            foreach (string flag in flags)
                parameters.Add(flag, 1);
            string jsonResult = await SendCommand("groups.setCallbackSettings", groupAccessToken, parameters);
        }

        public static async Task<string> GetCallbackConfirmationCode(string groupAccessToken, int idGroup)
        {
            string jsonResult = await SendCommand("groups.getCallbackConfirmationCode", groupAccessToken, new Dictionary<string, object>()
            {
                { "group_id", idGroup }
            });
            if (JsonConvert.DeserializeObject<BaseError>(jsonResult).Code > 0)
                return "";
            JObject result = JObject.Parse(jsonResult);
            return result["response"]["code"].Value<string>();
        }

        public static async Task<bool> IsMember(string groupAccessToken, int idGroup, int idUser)
        {
            string jsonResult = await SendCommand("groups.isMember", groupAccessToken, new Dictionary<string, object>()
            {
                { "group_id", idGroup },
                { "user_id", idUser }
            });
            if (JsonConvert.DeserializeObject<BaseError>(jsonResult).Code > 0)
                return false;

            BaseResponse<int> result = JsonConvert.DeserializeObject<BaseResponse<int>>(jsonResult);
            return result.Item == 1;
        }
    }
}
