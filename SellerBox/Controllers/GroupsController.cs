using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SellerBox.Common;
using SellerBox.Common.Services;
using SellerBox.Models.Database;
using SellerBox.ViewModels;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using VkNet.Utils;

namespace SellerBox.Controllers
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
            if (vkApi == null)
                return RedirectToAction(nameof(AccountController.ExternalLogin), "Account", new { provider = "Vkontakte", returnUrl = "/Groups" });
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

            foreach (var connectedGroupModel in models.Where(x => x.IsConnected))
            {
                var connectedGroup = await _context.Groups.FirstAsync(x => x.IdVk == connectedGroupModel.IdVk);
                connectedGroup.Update(groups.First(x => x.Id == connectedGroup.IdVk));
            }
            await _context.SaveChangesAsync();

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
                $"&scope={"messages,manage,photos,docs"}" +
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

            string result = null;
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(url, null);
                result = await response.Content.ReadAsStringAsync();

                Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(result);
                //TODO: Добавить проверку на null у token
                var token = obj.Properties().FirstOrDefault(x => x.Name.StartsWith("access_token_"));
                var idGroup = long.Parse(token.Name.Split('_').Last());
                string tokenValue = token.Value.ToString();
                
                var group = await _context.Groups.Where(x => x.IdVk == idGroup).FirstOrDefaultAsync();
                if (group == null)
                {
                    group = new Groups()
                    {
                        IdVk = idGroup
                    };
                    await _context.Groups.AddAsync(group);
                }

                group.AccessToken = tokenValue;

                using (var vkGroupApi = new VkNet.VkApi())
                {
                    await vkGroupApi.AuthorizeAsync(new VkNet.Model.ApiAuthParams()
                    {
                        AccessToken = tokenValue,
                        Settings = VkNet.Enums.Filters.Settings.Groups
                    });

                    group.CallbackConfirmationCode = await vkGroupApi.Groups.GetCallbackConfirmationCodeAsync((ulong)idGroup);

                    var groups = await vkGroupApi.Groups.GetByIdAsync(null, idGroup.ToString(), VkNet.Enums.Filters.GroupsFields.Description);
                    var groupInfo = groups.FirstOrDefault();

                    group.Name = groupInfo.Name;
                    group.Photo = groupInfo.Photo50.ToString();
                }
                await _context.SaveChangesAsync();

                var idUser = _userHelperService.GetUserId(User);

                var groupAdmin = await _context.GroupAdmins.Where(x => x.IdUser == idUser && x.IdGroup == idGroup).FirstOrDefaultAsync();
                if (groupAdmin == null)
                {
                    groupAdmin = new GroupAdmins()
                    {
                        IdGroup = idGroup,
                        IdUser = idUser,
                        DtConnect = DateTime.UtcNow
                    };
                    await _context.GroupAdmins.AddAsync(groupAdmin);
                }
                else
                    groupAdmin.DtConnect = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                url = $"https://api.vk.com/method/groups.getCallbackServers?" +
                    $"access_token={tokenValue}" +
                    $"&group_id={idGroup}" +
                    $"&v={AspNet.Security.OAuth.Vkontakte.VkontakteAuthenticationDefaults.ApiVersion}";

                response = await client.PostAsync(url, null);
                result = await response.Content.ReadAsStringAsync();
                var callbackServers = new VkResponse(result).ToVkCollectionOf<VkNet.Model.CallbackServerItem>(x => x);

                long idServer = -1;
                var callbackServerInfo = callbackServers.FirstOrDefault(x => x.Url == Logins.CallbackServerUrl);
                if (callbackServerInfo == null)
                {
                    url = $"https://api.vk.com/method/groups.addCallbackServer?" +
                        $"access_token={tokenValue}" +
                        $"&group_id={idGroup}" +
                        $"&url={Logins.CallbackServerUrl}" +
                        $"&title={Logins.CallbackServerName}" +
                        $"&v={AspNet.Security.OAuth.Vkontakte.VkontakteAuthenticationDefaults.ApiVersion}";
                    response = await client.PostAsync(url, null);
                    result = await response.Content.ReadAsStringAsync();

                    var json = Newtonsoft.Json.Linq.JObject.Parse(result);
                    idServer = new VkResponse(json[propertyName: "response"]) { RawJson = result }[key: "server_id"];
                }
                else
                    idServer = callbackServerInfo.Id;

                var callbackProperties = new VkNet.Model.CallbackSettings();
                foreach (var property in callbackProperties.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                    property.SetValue(callbackProperties, true);
                var parameters = VkNet.Model.RequestParams.CallbackServerParams.ToVkParameters(new VkNet.Model.RequestParams.CallbackServerParams()
                {
                    GroupId = (ulong)idGroup,
                    ServerId = idServer,
                    CallbackSettings = callbackProperties
                });
                parameters.Add("access_token", tokenValue);
                parameters.Add("v", AspNet.Security.OAuth.Vkontakte.VkontakteAuthenticationDefaults.ApiVersion);
                url = "https://api.vk.com/method/groups.setCallbackSettings";

                HttpContent content = new FormUrlEncodedContent(parameters);
                response = await client.PostAsync(url, content);
                result = await response.Content.ReadAsStringAsync();
            }
            return RedirectToAction(nameof(IndexConnected), "Groups");
        }

        //TODO переписать нафиг
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