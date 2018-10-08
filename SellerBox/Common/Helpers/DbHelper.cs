using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SellerBox.Models.Database;

namespace SellerBox.Common.Helpers
{
    internal static class DbHelper
    {
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

        public static async Task<Messages> AddMessage(DatabaseContext dbContext, long idGroup, string message, VkNet.Model.Keyboard.MessageKeyboard keyboard, bool isImageFirst, IEnumerable<Guid> idFiles)
        {
            Messages result = new Messages()
            {
                IdGroup = idGroup,
                Keyboard = keyboard == null ? null : Newtonsoft.Json.JsonConvert.SerializeObject(keyboard),
                Text = message,
                IsImageFirst = isImageFirst
            };
            await dbContext.Messages.AddAsync(result);
            await dbContext.SaveChangesAsync();

            List<FilesInMessage> attachments = new List<FilesInMessage>();
            foreach (Guid idFile in idFiles)
            {
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

        public static async Task RemoveMessage(DatabaseContext dbContext, Guid idMessage)
        {
            var filesInMessages = await dbContext.FilesInMessage.Where(x => x.IdMessage == idMessage).ToArrayAsync();
                foreach (var fileInMessage in filesInMessages)

            dbContext.Files.RemoveRange(dbContext.Files.Where(x => x.Id == fileInMessage.IdFile));
            dbContext.FilesInMessage.RemoveRange(filesInMessages);
            dbContext.History_Messages.RemoveRange(dbContext.History_Messages.Where(x => x.IdMessage == idMessage));
            dbContext.Messages.RemoveRange(dbContext.Messages.Where(x => x.Id == idMessage));

            await dbContext.SaveChangesAsync();
        }

        #endregion
    }
}
