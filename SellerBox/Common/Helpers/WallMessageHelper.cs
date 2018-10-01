using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SellerBox.Common.Helpers
{
    public class WallMessageHelper
    {
        public static readonly Dictionary<string, string> AvailableKeywords = new Dictionary<string, string>()
            {
                { "%USERS%", "Фамилия Имя" }
            };

        private readonly DatabaseContext _context;

        public event EventHandler<MessageSentEventArgs> MessageSent;
        protected virtual void OnMessageSent(int idGroup, int total, int current) => MessageSent?.Invoke(this, new MessageSentEventArgs(idGroup, total, current));

        public int Stepping { get; set; } = 100;

        private static bool HasKeywords(string text)
        {
            return AvailableKeywords.Any(x => text.Contains(x.Key));
        }

        private static async Task<string> UpdateMessage(DatabaseContext _context, string text, params long[] idVkUsers)
        {
            if (!HasKeywords(text))
                return text;

            var vkUsers = await _context.VkUsers
                .Where(x => idVkUsers.Contains(x.IdVk))
                .Select(x => string.Format("*{0} ({1} {2})", x.IdVk, x.FirstName, x.LastName))
                .ToArrayAsync();

            if (vkUsers == null || !vkUsers.Any())
                return text;

            return text.Replace("%USERS%", string.Join(", ", vkUsers));
        }

        public WallMessageHelper(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<long> SendWallMessage(VkNet.VkApi vkApi, long idGroup, Guid idMessage, params long[] vkUserIds)
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

            string text = await UpdateMessage(_context, message.Text, vkUserIds);

            var postId = await vkApi.Wall.PostAsync(new VkNet.Model.RequestParams.WallPostParams()
            {
                Attachments = (!attachments?.Any() ?? true) ? null : attachments,
                Message = text,
                OwnerId = -idGroup,
                FromGroup = true
            });// VkConnector.Methods.Wall.Post(userAccessToken, idGroup, text, attachments);

            await _context.WallPosts.AddAsync(new Models.Database.WallPosts()
            {
                DtAdd = DateTime.UtcNow,
                IdGroup = idGroup,
                IdVk = postId,
                Text = text
            });
            await _context.SaveChangesAsync();

            return postId;
        }
    }
}
