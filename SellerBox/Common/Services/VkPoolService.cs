using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SellerBox.Common.Services
{
    public class VkPoolService
    {
        private readonly IConfiguration _configuration;
        private readonly DatabaseContext _context;

        private static Dictionary<long, VkNet.VkApi> GroupsVkApis { get; } = new Dictionary<long, VkNet.VkApi>();
        private static Dictionary<long, VkNet.VkApi> UsersVkApis { get; } = new Dictionary<long, VkNet.VkApi>();

        public VkPoolService(IConfiguration configuration, DatabaseContext dbContext)
        {
            _configuration = configuration;
            _context = dbContext;
        }

        public async Task<VkNet.VkApi> GetGroupVkApi(long idGroup)
        {
            if (GroupsVkApis.ContainsKey(idGroup))
                return GroupsVkApis[idGroup];

            var groupAccessToken = await _context.Groups
                .Where(x => x.IdVk == idGroup)
                .Select(x => x.AccessToken)
                .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(groupAccessToken))
                return null;

            var groupVkApi = new VkNet.VkApi();
            await groupVkApi.AuthorizeAsync(new VkNet.Model.ApiAuthParams()
            {
                AccessToken = groupAccessToken,
                ApplicationId = _configuration.GetValue<ulong>("VkApplicationId"),
                Settings = VkNet.Enums.Filters.Settings.All
            });

            GroupsVkApis.TryAdd(idGroup, groupVkApi);

            return groupVkApi;
        }

        public async Task<VkNet.VkApi> GetUserVkApi(long idVkUser)
        {
            if (UsersVkApis.ContainsKey(idVkUser))
                return UsersVkApis[idVkUser];

            var idUser = await _context.Users
                .Where(x => x.IdVk == idVkUser)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            var userAccessToken = await _context.UserTokens
                .Where(x => x.LoginProvider == "Vkontakte")
                .Where(x => x.UserId == idUser)
                .Where(x => x.Name == "access_token")
                .Select(x => x.Value)
                .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(userAccessToken))
                return null;

            var userVkApi = new VkNet.VkApi();
            try
            {
                await userVkApi.AuthorizeAsync(new VkNet.Model.ApiAuthParams()
                {
                    AccessToken = userAccessToken,
                    ApplicationId = _configuration.GetValue<ulong>("VkApplicationId"),
                    Settings = VkNet.Enums.Filters.Settings.All
                });
            }
            catch { }
            UsersVkApis.TryAdd(idVkUser, userVkApi);

            return userVkApi;
        }
    }
}
