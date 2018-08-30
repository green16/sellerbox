using System.IO;
using System.Threading.Tasks;
using VkConnector.Methods;
using VkConnector.Models.Common;
using WebApplication1.Models.Database;

namespace WebApplication1.Common.Helpers
{
    public static class VkHelper
    {
        public static async Task<string> UploadMessageAttachment(string groupAccessToken, int idGroup, Files file)
        {
            string result = string.Empty;

            string extension = Path.GetExtension(file.Name).ToLower();
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".gif":
                    {
                        GetMessagesUploadServerResponse uploadServerInfo = await Photos.GetMessagesUploadServer(groupAccessToken, idGroup);
                        SavedPhotoInfo savedPhotoInfo = await Photos.SaveMessagesPhoto(groupAccessToken, uploadServerInfo, file.Name, file.Content);
                        result = $"photo{savedPhotoInfo.IdOwner}_{savedPhotoInfo.Id}_{savedPhotoInfo.AccessKey}";
                        break;
                    }

                default:
                    {
                        string uploadServerUrl = await Docs.GetUploadServer(groupAccessToken, idGroup);
                        await Docs.Save(groupAccessToken, uploadServerUrl, file.Name, file.Content);
                        result = "doc{}_{}_{}";
                        break;
                    }
            }

            return result;
        }
    }
}
