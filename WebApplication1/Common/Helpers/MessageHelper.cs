using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Common.Helpers
{
    public class MessageSentEventArgs : EventArgs
    {
        public int IdGroup { get; protected set; }
        public int Total { get; protected set; }
        public int Current { get; protected set; }

        public MessageSentEventArgs(int idGroup, int total, int current)
        {
            IdGroup = idGroup;
            Total = total;
            Current = current;
        }
    }

    public class MessageHelper
    {
        public static readonly Dictionary<string, string> AvailableKeywords = new Dictionary<string, string>()
            {
                { "%USERNAME%", "Имя" },
                { "%USERLASTNAME%", "Фамилия" },
                { "%USERSECONDNAME%", "Отчество (ник)" },
            };

        private readonly DatabaseContext _context;

        public event EventHandler<MessageSentEventArgs> MessageSent;
        protected virtual void OnMessageSent(int idGroup, int total, int current) => MessageSent?.Invoke(this, new MessageSentEventArgs(idGroup, total, current));

        public int Stepping { get; set; } = 100;

        private static bool HasKeywords(string text)
        {
            return AvailableKeywords.Any(x => text.Contains(x.Key));
        }

        private static async Task<string> UpdateMessage(DatabaseContext _context, int idVkUser, string text)
        {
            if (!HasKeywords(text))
                return text;

            var vkUser = await _context.VkUsers.FirstOrDefaultAsync(x => x.IdVk == idVkUser);
            if (vkUser == null)
                return text;

            return text.Replace("%USERNAME%", vkUser.FirstName)
                       .Replace("%USERLASTNAME%", vkUser.LastName)
                       .Replace("%USERSECONDNAME%", vkUser.SecondName);
        }

        public MessageHelper(DatabaseContext context)
        {
            _context = context;
        }

        private async Task SendMessage(string groupAccessToken, bool isImageFirst, string text, IEnumerable<string> attachments, VkConnector.Models.Common.Keyboard keyboard, int[] ids)
        {
            if (isImageFirst && attachments != null && attachments.Any())
            {
                await VkConnector.Methods.Messages.Send(groupAccessToken, null, attachments, null, ids);
                await VkConnector.Methods.Messages.Send(groupAccessToken, text, null, keyboard, ids);
            }
            else
                await VkConnector.Methods.Messages.Send(groupAccessToken, text, attachments, keyboard, ids);
        }

        public async Task SendMessages(int idGroup, Guid idMessage, params int[] vkUserIds)
        {
            var message = await _context.Messages
                .Include(x => x.Files)
                .FirstOrDefaultAsync(x => x.Id == idMessage);

            var idFiles = message.Files.Select(x => x.IdFile).ToArray();
            var attachments = await _context.Files
                .Where(x => idFiles.Contains(x.Id))
                .Select(x => x.VkUrl)
                .ToArrayAsync();

            string groupAccessToken = await _context.Groups.Where(x => x.IdVk == idGroup).Select(x => x.AccessToken).FirstOrDefaultAsync();

            if (HasKeywords(message.Text))
            {
                for (int idx = 0; idx < vkUserIds.Length; idx++)
                {
                    string text = await UpdateMessage(_context, vkUserIds[idx], message.Text);

                    await SendMessage(groupAccessToken, message.IsImageFirst, text, attachments, VkConnector.Models.Common.Keyboard.Deserialize(message.Keyboard), new int[] { vkUserIds[idx] });

                    OnMessageSent(idGroup, vkUserIds.Length, idx + 1);
                }
            }
            else
            {
                int offset = 0;
                do
                {
                    var currentUserIds = (Stepping > vkUserIds.Length - offset ? vkUserIds.Skip(offset) : vkUserIds.Skip(offset).Take(Stepping)).ToArray();
                    await SendMessage(groupAccessToken, message.IsImageFirst, message.Text, attachments, VkConnector.Models.Common.Keyboard.Deserialize(message.Keyboard), currentUserIds);

                    offset += currentUserIds.Length;
                    if (offset >= vkUserIds.Length)
                        break;

                    OnMessageSent(idGroup, vkUserIds.Length, offset);
                } while (offset < vkUserIds.Length);
            }
        }
    }
}
