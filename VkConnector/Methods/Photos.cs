using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VkConnector.Common;
using VkConnector.Models.Common;

namespace VkConnector.Methods
{
    public class Photos : Base
    {
        public static async Task<GetMessagesUploadServerResponse> GetMessagesUploadServer(string groupAccessToken, long idGroup)
        {
            string json = await SendCommand("photos.getMessagesUploadServer", groupAccessToken);
            BaseResponse<GetMessagesUploadServerResponse> result = JsonConvert.DeserializeObject<BaseResponse<GetMessagesUploadServerResponse>>(json);
            return result.Item;
        }

        public static async Task<SavedPhotoInfo> SaveMessagesPhoto(string groupAccessToken, GetMessagesUploadServerResponse getMessagesUploadInfo, string fileName, byte[] fileContent)
        {
            string tempFilePath = Path.Combine(Path.GetTempPath(), fileName);
            File.WriteAllBytes(tempFilePath, fileContent);
            string responseJson;
            using (WebClient wc = new WebClient())
                responseJson = Encoding.ASCII.GetString(wc.UploadFile(getMessagesUploadInfo.UploadUrl, tempFilePath));
            UploadedPhotoInfo uploadedPhotoInfo = JsonConvert.DeserializeObject<UploadedPhotoInfo>(responseJson);

            string savedPhotoResponse = await SendCommand("photos.saveMessagesPhoto", groupAccessToken, new Dictionary<string, object>()
            {
                { "hash", uploadedPhotoInfo.Hash },
                { "server", uploadedPhotoInfo.Server },
                { "photo", uploadedPhotoInfo.Photo.ToString() },
            });

            try { File.Delete(tempFilePath); } catch { }

            SavedPhotoInfo savedPhotoInfo = JsonConvert.DeserializeObject<BaseResponse<IEnumerable<SavedPhotoInfo>>>(savedPhotoResponse).Item.FirstOrDefault();

            return savedPhotoInfo;
        }
    }
}
