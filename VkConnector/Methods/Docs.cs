using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VkConnector.Common;
using VkConnector.Models.Errors;

namespace VkConnector.Methods
{
    public class Docs : Base
    {
        public static async Task<string> GetUploadServer(string groupAccessToken, int idGroup)
        {
            string jsonResult = await SendCommand("docs.getMessagesUploadServer", groupAccessToken, new Dictionary<string, object>()
            {
                { "peer_id", -idGroup }
            });
            BaseResponse<JObject> result = JsonConvert.DeserializeObject<BaseResponse<JObject>>(jsonResult);
            if (JsonConvert.DeserializeObject<BaseError>(jsonResult).Code > 0)
                return null;// Enumerable.Empty<User>();
            return result.Item.Value<string>("upload_url");
        }

        public static async Task Save(string groupAccessToken, string uploadUrl, string fileName, byte[] fileContent)
        {
            string tempFilePath = Path.Combine(Path.GetTempPath(), fileName);
            File.WriteAllBytes(tempFilePath, fileContent);
            string responseJson;
            using (WebClient wc = new WebClient())
                responseJson = Encoding.ASCII.GetString(wc.UploadFile(uploadUrl, tempFilePath));
            JObject uploadedDocInfo = JObject.Parse(responseJson);
            string fileInfo = uploadedDocInfo.Value<string>("file");
            string savedPhotoResponse = await SendCommand("docs.save", groupAccessToken, new Dictionary<string, object>()
            {
                { "file", fileInfo },
                { "title", fileName },
            });

            try { File.Delete(tempFilePath); } catch { }

            //SavedPhotoInfo savedPhotoInfo = JsonConvert.DeserializeObject<BaseResponse<IEnumerable<SavedPhotoInfo>>>(savedPhotoResponse).Item.FirstOrDefault();

            //return savedPhotoInfo;
        }
    }
}
