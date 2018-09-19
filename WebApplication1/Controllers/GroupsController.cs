using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebApplication1.Common;
using WebApplication1.Common.Services;
using WebApplication1.Models.Database;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class GroupsController : Controller
    {
        private readonly UserHelperService _userHelperService;
        private readonly DatabaseContext _context;
        private readonly VkPoolService _vkPoolService;

        public GroupsController(UserHelperService userManager, DatabaseContext context, VkPoolService vkPoolService)
        {
            _userHelperService = userManager;
            _context = context;
            _vkPoolService = vkPoolService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            DateTime? dtExpiresAt = await _userHelperService.GetVkUserAccessTokenExpiresAt(User);

            string idUser = _userHelperService.GetUserId(User);
            var idVkUser = await _userHelperService.GetUserIdVk(User);

            var vkApi = await _vkPoolService.GetUserVkApi(idVkUser);

            var groups = await vkApi.Groups.GetAsync(new VkNet.Model.RequestParams.GroupsGetParams()
            {
                UserId = idVkUser,
                Extended = true,
                Filter = VkNet.Enums.Filters.GroupsFilters.Administrator
            });
            var models = groups.Select(x => new GroupsViewModel()
            {
                IdVk = x.Id,
                ImageUrl = x.Photo50,
                Name = x.Name,
                Link = $"https://vk.com/club{x.Id}",
                IsConnected = _context.GroupAdmins
                    .Include(y => y.Group)
                    .Any(y => y.IdGroup == x.Id && y.IdUser == idUser && !string.IsNullOrEmpty(y.Group.AccessToken))
            });
            return View(models);
        }

        [HttpGet]
        public async Task<IActionResult> IndexConnected()
        {
            string idUser = _userHelperService.GetUserId(User);

            var models = await _context.GroupAdmins.Include(x => x.Group)
                .Where(x => x.IdUser == idUser && !string.IsNullOrEmpty(x.Group.AccessToken))
                .Select(x => new GroupsViewModel()
                {
                    IdVk = x.Group.IdVk,
                    ImageUrl = new Uri(x.Group.Photo),
                    Name = x.Group.Name,
                    Link = $"https://vk.com/club{x.Group.IdVk}",
                    IsConnected = true
                })
                .ToArrayAsync();
            return View("Index", models);
        }

        [HttpPost]
        public IActionResult Connect(int idGroup)
        {
            string url = $"https://oauth.vk.com/authorize?" +
                $"client_id={Logins.VkApplicationId}" +
                $"&redirect_uri={$"{Logins.SiteUrl}/Groups/ConnectCallback"}" +
                $"&group_ids={idGroup}" +
                "&response_type=code" +
                $"&scope={"messages,manage,photos,docs,wall,stories"}" +
                $"&v={AspNet.Security.OAuth.Vkontakte.VkontakteAuthenticationDefaults.ApiVersion}";
            return Redirect(url);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ConnectCallback(string code)
        {
            string url = $"https://oauth.vk.com/access_token?" +
                $"client_id={Logins.VkApplicationId}" +
                $"&client_secret={Logins.VkApplicationPassword}" +
                $"&redirect_uri={$"{Logins.SiteUrl}/Groups/ConnectCallback"}" +
                $"&code={code}";

            var client = new HttpClient();
            var response = await client.PostAsync(url, null);
            string result = await response.Content.ReadAsStringAsync();

            Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(result);
            var token = obj.Properties().First(x => x.Name.StartsWith("access_token_"));
            var idGroup = long.Parse(token.Name.Split('_').Last());
            string tokenValue = token.Value.ToString();

            string idUser = _userHelperService.GetUserId(User);

            if (!_context.GroupAdmins.Any(x => x.IdUser == idUser && x.IdGroup == idGroup))
            {
                if (!_context.Groups.Any(x => x.IdVk == idGroup))
                {
                    using (var vkGroupApi = new VkNet.VkApi())
                    {
                        await vkGroupApi.AuthorizeAsync(new VkNet.Model.ApiAuthParams()
                        {
                            ApplicationId = Logins.VkApplicationId,
                            Password = Logins.VkApplicationPassword,
                            //AccessToken = tokenValue,
                            Settings = VkNet.Enums.Filters.Settings.All
                        });

                        var groups = await vkGroupApi.Groups.GetByIdAsync(null, idGroup.ToString(), VkNet.Enums.Filters.GroupsFields.Description);

                        var groupInfo = groups.FirstOrDefault();
                        var callbackConfirmationCode = await vkGroupApi.Groups.GetCallbackConfirmationCodeAsync((ulong)idGroup);

                        await _context.Groups.AddAsync(new Groups()
                        {
                            IdVk = idGroup,
                            AccessToken = tokenValue,
                            CallbackConfirmationCode = callbackConfirmationCode,
                            Name = groupInfo.Name,
                            Photo = groupInfo.Photo50.ToString()
                        });
                    }
                }

                await _context.GroupAdmins.AddAsync(new GroupAdmins()
                {
                    IdGroup = idGroup,
                    IdUser = idUser,
                    DtConnect = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();
            }

            var idVkUser = await _userHelperService.GetUserIdVk(User);
            var vkApi = await _vkPoolService.GetUserVkApi(idVkUser);

            var callbackServers = await vkApi.Groups.GetCallbackServersAsync((ulong)idGroup);
            long idServer = -1;
            var callbackServerInfo = callbackServers.FirstOrDefault(x => x.Url == Logins.CallbackServerUrl);
            if (callbackServerInfo == null)
                idServer = await vkApi.Groups.AddCallbackServerAsync((ulong)idGroup, Logins.CallbackServerUrl, Logins.CallbackServerName);
            else
                idServer = callbackServerInfo.Id;

            var callbackProperties = new VkNet.Model.CallbackSettings();
            foreach (var property in callbackProperties.GetType().GetProperties(System.Reflection.BindingFlags.Public))
                property.SetValue(callbackProperties, true);

            await vkApi.Groups.SetCallbackSettingsAsync(new VkNet.Model.RequestParams.CallbackServerParams()
            {
                GroupId = (ulong)idGroup,
                ServerId = idServer,
                CallbackSettings = callbackProperties
            });

            return RedirectToAction(nameof(IndexConnected), "Groups");
        }

        [HttpPost]
        public async Task<IActionResult> Disconnect(int idGroup)
        {
            string idUser = _userHelperService.GetUserId(User);

            var removingGroups = _context.GroupAdmins.Where(x => x.IdUser == idUser && x.IdGroup == idGroup);
            _context.GroupAdmins.RemoveRange(removingGroups);

            var hasAnyConnection = await _context.GroupAdmins.AnyAsync(x => x.IdUser != idUser && x.IdGroup == idGroup);
            if (!hasAnyConnection)
            {
                _context.RemoveRange(_context.SubscribersInChains.Include(x => x.Subscriber).Where(x => x.Subscriber.IdGroup == idGroup));
            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), "Groups");
        }

        [HttpPost]
        public async Task<IActionResult> SetCurrent(int idGroup)
        {
            string idUser = _userHelperService.GetUserId(User);

            var currentUser = _context.Users.First(x => x.Id == idUser);
            currentUser.IdCurrentGroup = idGroup;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(IndexConnected), "Groups");
        }
    }
}