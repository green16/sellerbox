﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Common.Helpers
{
    public class MessageSentEventArgs : EventArgs
    {
        public long IdGroup { get; set; }

        public int Total { get; set; }
        public int Process { get; set; }

        public MessageSentEventArgs(long idGroup, int total, int process)
        {
            IdGroup = idGroup;
            Total = total;
            Process = process;
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

        public static readonly Dictionary<string, string> AvailableWallKeywords = new Dictionary<string, string>()
            {
                { "%USERS%", "Фамилия Имя" }
            };

        private readonly DatabaseContext _context;

        public event EventHandler<MessageSentEventArgs> MessageSent;
        protected virtual void OnMessageSent(long idGroup, int total, int process) => MessageSent?.Invoke(this, new MessageSentEventArgs(idGroup, total, process));

        public int Stepping { get; set; } = 100;

        private static bool HasKeywords(string text) => string.IsNullOrWhiteSpace(text) ? false : AvailableKeywords.Any(x => text.Contains(x.Key));

        private static async Task<string> UpdateMessage(DatabaseContext _context, long idVkUser, string text)
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

        private async Task SendMessage(VkNet.VkApi vkApi, bool isImageFirst, string text, IEnumerable<VkNet.Model.Attachments.MediaAttachment> attachments, VkNet.Model.Keyboard.MessageKeyboard keyboard, long[] ids)
        {
            if (keyboard != null)
                keyboard.OneTime = true;
            string nbspString = new string(new char[] { (char)160 });
            if (isImageFirst && attachments != null && attachments.Any())
            {
                await vkApi.Messages.SendToUserIdsAsync(new VkNet.Model.RequestParams.MessagesSendParams()
                {
                    UserIds = ids,
                    Message = nbspString,
                    Attachments = (!attachments?.Any() ?? false) ? null : attachments
                });
                if (!string.IsNullOrEmpty(text) || keyboard != null)
                    await vkApi.Messages.SendToUserIdsAsync(new VkNet.Model.RequestParams.MessagesSendParams()
                    {
                        UserIds = ids,
                        Message = string.IsNullOrEmpty(text) ? nbspString : text,
                        Keyboard = keyboard
                    });
                return;
            }

            await vkApi.Messages.SendToUserIdsAsync(new VkNet.Model.RequestParams.MessagesSendParams()
            {
                UserIds = ids,
                Message = string.IsNullOrEmpty(text) ? nbspString : text,
                Attachments = (!attachments?.Any() ?? false) ? null : attachments,
                Keyboard = keyboard
            });
        }

        public async Task SendMessages(VkNet.VkApi vkApi, long idGroup, Guid idMessage, params long[] vkUserIds)
        {
            var message = await _context.Messages
                .Include(x => x.Files)
                .FirstOrDefaultAsync(x => x.Id == idMessage);

            var idFiles = message.Files.Select(x => x.IdFile).ToArray();
            var attachments = await _context.Files
                .Where(x => idFiles.Contains(x.Id))
                .Select(x => x.VkUrl)
                .Select(x => Newtonsoft.Json.JsonConvert.DeserializeObject<VkNet.Model.Attachments.Photo>(x))
                .ToArrayAsync();

            var keyboard = string.IsNullOrWhiteSpace(message.Keyboard) ? null : Newtonsoft.Json.JsonConvert.DeserializeObject<VkNet.Model.Keyboard.MessageKeyboard>(message.Keyboard);

            if (HasKeywords(message.Text))
            {
                for (int idx = 0; idx < vkUserIds.Length; idx++)
                {
                    string text = await UpdateMessage(_context, vkUserIds[idx], message.Text);

                    await SendMessage(vkApi, message.IsImageFirst, text, attachments, keyboard, new long[] { vkUserIds[idx] });

                    OnMessageSent(idGroup, vkUserIds.Length, idx + 1);
                }
            }
            else
            {
                int offset = 0;
                do
                {
                    var currentUserIds = (Stepping > vkUserIds.Length - offset ? vkUserIds.Skip(offset) : vkUserIds.Skip(offset).Take(Stepping)).ToArray();
                    await SendMessage(vkApi, message.IsImageFirst, message.Text, attachments, keyboard, currentUserIds);

                    offset += currentUserIds.Length;
                    if (offset >= vkUserIds.Length)
                        break;

                    OnMessageSent(idGroup, vkUserIds.Length, offset);
                } while (offset < vkUserIds.Length);
            }
        }
    }
}
