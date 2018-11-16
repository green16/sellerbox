using Microsoft.EntityFrameworkCore;
using SellerBox.Common.Services;
using SellerBox.Models.Database;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SellerBox.Common.Helpers
{
    public static class VkHelper
    {
        public static DateTime? BirtdayConvert(string birthday)
        {
            if (string.IsNullOrWhiteSpace(birthday))
                return null;
            string[] dateParts = birthday.Split('.');
            if (DateTime.TryParseExact(birthday, new string[] { "d.M.yyyy", "d.M" }, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime result))
            {
                if (dateParts.Length == 2 && result.Year == DateTime.UtcNow.Year)
                    return result.AddYears(-result.Year + 1);
                return result;
            }
            return null;
        }

        public static async Task<Subscribers> CreateSubscriber(DatabaseContext _context, VkPoolService _vkPoolService, long idGroup, long? idVkUser)
        {
            if (!idVkUser.HasValue || idVkUser.Value <= 0)
                return null;
            var subscriber = await _context.Subscribers.FirstOrDefaultAsync(x => x.IdGroup == idGroup && x.IdVkUser == idVkUser.Value);
            if (subscriber != null)
                return subscriber;

            var vkUser = await _context.VkUsers.FirstOrDefaultAsync(x => x.IdVk == idVkUser.Value);
            if (vkUser == null)
            {
                try
                {
                    var vkApi = await _vkPoolService.GetGroupVkApi(idGroup);
                    if (vkApi == null)
                        return null;

                    var users = await vkApi.Users.GetAsync(new long[] { idVkUser.Value }, VkNet.Enums.Filters.ProfileFields.BirthDate |
                        VkNet.Enums.Filters.ProfileFields.City |
                        VkNet.Enums.Filters.ProfileFields.Country |
                        VkNet.Enums.Filters.ProfileFields.FirstName |
                        VkNet.Enums.Filters.ProfileFields.LastName |
                        VkNet.Enums.Filters.ProfileFields.Nickname |
                        VkNet.Enums.Filters.ProfileFields.Domain |
                        VkNet.Enums.Filters.ProfileFields.Photo50 |
                        VkNet.Enums.Filters.ProfileFields.Photo400Orig |
                        VkNet.Enums.Filters.ProfileFields.Sex |
                        VkNet.Enums.Filters.ProfileFields.Blacklisted);

                    if (users == null || !users.Any())
                        return null;

                    vkUser = VkUsers.FromUser(users.FirstOrDefault());
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при запросе users.get с id={idVkUser}{Environment.NewLine}{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                    return null;
                }

                _context.VkUsers.Update(vkUser);
            }

            subscriber = new Subscribers()
            {
                IdVkUser = vkUser.IdVk,
                IdGroup = idGroup,
                DtAdd = DateTime.UtcNow
            };
            await _context.Subscribers.AddAsync(subscriber);
            await _context.SaveChangesAsync();

            return subscriber;
        }
    }
}
