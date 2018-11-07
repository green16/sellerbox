using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SellerBox.Common.Helpers
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

        private readonly Dictionary<string, string> AvailableWallKeywords = new Dictionary<string, string>()
            {
                { "%USERS%", "Фамилия Имя" }
            };

        private readonly List<string> AvailableRegexKeywords = new List<string>()
        {
            { @"%SHORTLINK:(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})%" }
        };

        private readonly IConfiguration _configuration;
        private readonly DatabaseContext _context;

        public event EventHandler<MessageSentEventArgs> MessageSent;
        protected virtual void OnMessageSent(long idGroup, int total, int process) => MessageSent?.Invoke(this, new MessageSentEventArgs(idGroup, total, process));

        public int Stepping { get; set; } = 100;

        public MessageHelper(IConfiguration configuration, DatabaseContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        private bool HasAnyKeywords(string text) => !string.IsNullOrWhiteSpace(text) && (
            AvailableKeywords.Any(x => text.Contains(x.Key)) ||
            AvailableWallKeywords.Any(x => text.Contains(x.Key)) ||
            AvailableRegexKeywords.Any(x => Regex.IsMatch(text, x)));

        public string UpdateMessageByVkUser(Models.Database.VkUsers vkUser, string message)
        {
            if (vkUser == null)
                return message;

            return message.Replace("%USERNAME%", vkUser.FirstName)
                .Replace("%USERLASTNAME%", vkUser.LastName)
                .Replace("%USERSECONDNAME%", vkUser.SecondName);
        }

        public async Task<string> UpdateMessageByShortUrls(DatabaseContext _context, string siteUrl, Models.Database.Subscribers subscriber, string message)
        {
            if (subscriber == null)
                return message;

            var matches = Regex.Matches(message, AvailableRegexKeywords[0]);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    var guidString = match.Groups[1].Value;
                    if (Guid.TryParse(guidString, out Guid idShortUrl))
                    {
                        var shortUrl = await _context.ShortUrls.FindAsync(idShortUrl);
                        if (shortUrl != null)
                        {
                            string url = $"{siteUrl}/sl={UrlShortenerHelper.Encode(shortUrl.Id)}";
                            if (shortUrl.IsSubscriberRequired)
                                url += $"&{UrlShortenerHelper.Encode(subscriber.Id)}";
                            message = message.Replace(match.Value, url);
                        }
                    }
                }
            }
            return message;
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
                .ToArrayAsync();

            var images = attachments.Select(x =>
            {
                try { return Newtonsoft.Json.JsonConvert.DeserializeObject<VkNet.Model.Attachments.Photo>(x); }
                catch { return null; }
            }).Where(x => x != null);

            var keyboard = string.IsNullOrWhiteSpace(message.Keyboard) ? null : Newtonsoft.Json.JsonConvert.DeserializeObject<VkNet.Model.Keyboard.MessageKeyboard>(message.Keyboard);

            if (HasAnyKeywords(message.Text))
            {
                string siteUrl = _configuration.GetValue<string>("SiteUrl");
                for (int idx = 0; idx < vkUserIds.Length; idx++)
                {
                    string text = message.Text;

                    var subscriber = await _context.Subscribers
                        .Include(nameof(Models.Database.Subscribers.VkUser))
                        .FirstOrDefaultAsync(x => x.IdGroup == idGroup && x.IdVkUser == vkUserIds[idx]);

                    if (subscriber != null)
                    {
                        text = UpdateMessageByVkUser(subscriber.VkUser, text);
                        text = await UpdateMessageByShortUrls(_context, siteUrl, subscriber, text);
                    }

                    await SendMessage(vkApi, message.IsImageFirst, text, images, keyboard, new long[] { vkUserIds[idx] });

                    OnMessageSent(idGroup, vkUserIds.Length, idx + 1);
                }
            }
            else
            {
                int offset = 0;
                do
                {
                    var currentUserIds = (Stepping > vkUserIds.Length - offset ? vkUserIds.Skip(offset) : vkUserIds.Skip(offset).Take(Stepping)).ToArray();
                    await SendMessage(vkApi, message.IsImageFirst, message.Text, images, keyboard, currentUserIds);

                    offset += currentUserIds.Length;
                    if (offset >= vkUserIds.Length)
                        break;

                    OnMessageSent(idGroup, vkUserIds.Length, offset);
                } while (offset < vkUserIds.Length);
            }
        }
    }
}
