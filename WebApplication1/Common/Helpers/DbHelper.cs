using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VkConnector.Models.Common;
using WebApplication1.Models.Database;

namespace WebApplication1.Common.Helpers
{
    internal static class DbHelper
    {
        #region Scenarios

        public static Scenarios GetScenario(DatabaseContext dbContext, Guid idScenario)
        {
            return dbContext.Scenarios
                .Include(x => x.Message)
                .Include(x => x.ErrorMessage)
                .Include(x => x.Message.Files)
                .FirstOrDefault(x => x.Id == idScenario);
        }

        #endregion

        #region Subscribers

        public static IQueryable<Subscribers> GetSubscribersInGroup(DatabaseContext dbContext, int idGroup)
        {
            return dbContext.Subscribers
                .Include(x => x.VkUser)
                .Where(x => x.IdGroup == idGroup && !x.IsUnsubscribed);
        }

        #endregion

        #region ChainContents

        public static IQueryable<ChainContents> GetChainContents(DatabaseContext dbContext, Guid idChain)
        {
            return dbContext.ChainContents
                .Where(x => x.IdChain == idChain)
                .Include(x => x.Message)
                .Include(x => x.ExcludeFromChain)
                .Include(x => x.GoToChain);
        }
        #endregion

        #region FilesInMessage

        public static void AddFilesInMessage(DatabaseContext dbContext, Guid idMessage, IEnumerable<Guid> idFiles)
        {
            IEnumerable<FilesInMessage> newIdFiles = idFiles.Select(x => new FilesInMessage()
            {
                IdFile = x,
                IdMessage = idMessage
            });
            dbContext.FilesInMessage.AddRange(newIdFiles);
            dbContext.SaveChanges();
        }

        #endregion

        #region Messages

        public static async Task<Messages> AddMessage(DatabaseContext dbContext, int idGroup, string message, Keyboard keyboard, bool isImageFirst, IEnumerable<Guid> idFiles)
        {
            Messages result = new Messages()
            {
                IdGroup = idGroup,
                Keyboard = keyboard?.Serialize(),
                Text = message,
                IsImageFirst = isImageFirst
            };
            await dbContext.Messages.AddAsync(result);
            await dbContext.SaveChangesAsync();

            List<FilesInMessage> attachments = new List<FilesInMessage>();
            foreach (Guid idFile in idFiles)
            {
                Files file = await dbContext.Files.FirstOrDefaultAsync(x => x.Id == idFile);
                string groupAccessToken = await dbContext.Groups.Where(x => x.IdVk == idGroup).Select(x => x.AccessToken).FirstOrDefaultAsync();

                file.VkUrl = await VkHelper.UploadMessageAttachment(groupAccessToken, idGroup, file);
                await dbContext.SaveChangesAsync();

                attachments.Add(new FilesInMessage()
                {
                    IdMessage = result.Id,
                    IdFile = idFile
                });
            }

            //result.Files = attachments;
            await dbContext.FilesInMessage.AddRangeAsync(attachments);
            await dbContext.SaveChangesAsync();

            return result;
        }

        public static void RemoveMessage(DatabaseContext dbContext, Guid idMessage)
        {
            var messages = dbContext.Messages.Include(x => x.Files).Where(x => x.Id == idMessage);
            dbContext.Messages.RemoveRange(messages);

            var filesInMessages = messages.SelectMany(x => x.Files);
            dbContext.FilesInMessage.RemoveRange(filesInMessages);

            var files = filesInMessages.Include(x => x.File);
            foreach (var file in files)
                dbContext.Files.RemoveRange(files.Select(x => x.File));

            dbContext.SaveChanges();
        }

        #endregion
    }
}
