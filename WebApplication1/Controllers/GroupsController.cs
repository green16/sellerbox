using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VkConnector.Models.Objects;
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

        public GroupsController(UserHelperService userManager, DatabaseContext context)
        {
            _userHelperService = userManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            DateTime? dtExpiresAt = await _userHelperService.GetVkUserAccessTokenExpiresAt(User);
            string accessToken = await _userHelperService.GeVktUserAccessToken(User);
            if (string.IsNullOrEmpty(accessToken) || !dtExpiresAt.HasValue || dtExpiresAt < DateTime.Now)
                return RedirectToAction("ExternalLogin", "Account", new { provider = "Vkontakte", returnUrl = @"/Groups/Index" });

            string idUser = _userHelperService.GetUserId(User);
            int idVkUser = await _userHelperService.GetUserIdVk(User);

            var groups = await VkConnector.Methods.Groups.Get(accessToken, idVkUser);
            var models = groups.Select(x => new GroupsViewModel()
            {
                IdVk = x.Id,
                ImageUrl = x.PhotoSquare50,
                Name = x.Name,
                Link = $"{VkConnector.Methods.Base.VkUrl}/club{x.Id}",
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
                    Link = $"{VkConnector.Methods.Base.VkUrl}/club{x.Group.IdVk}",
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
            int idGroup = int.Parse(token.Name.Split('_').Last());
            string tokenValue = token.Value.ToString();

            string idUser = _userHelperService.GetUserId(User);

            if (!_context.GroupAdmins.Any(x => x.IdUser == idUser && x.IdGroup == idGroup))
            {
                if (!_context.Groups.Any(x => x.IdVk == idGroup))
                {
                    Group groupInfo = await VkConnector.Methods.Groups.GetById(tokenValue, idGroup);
                    string callbackConfirmationCode = await VkConnector.Methods.Groups.GetCallbackConfirmationCode(tokenValue, idGroup);

                    await _context.Groups.AddAsync(new Groups()
                    {
                        IdVk = idGroup,
                        AccessToken = tokenValue,
                        CallbackConfirmationCode = callbackConfirmationCode,
                        Name = groupInfo.Name,
                        Photo = groupInfo.PhotoSquare50.ToString()
                    });
                }

                await _context.GroupAdmins.AddAsync(new GroupAdmins()
                {
                    IdGroup = idGroup,
                    IdUser = idUser,
                    DtConnect = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();
            }

            var callbackServers = await VkConnector.Methods.Groups.GetCallbackServers(tokenValue, idGroup);
            int idServer = -1;
            var callbackServerInfo = callbackServers.FirstOrDefault(x => x.Url == Logins.CallbackServerUrl);
            if (callbackServerInfo == null)
                idServer = await VkConnector.Methods.Groups.AddCallbackServer(tokenValue, idGroup, Logins.CallbackServerName, Logins.CallbackServerUrl);
            else
                idServer = callbackServerInfo.IdServer;

            await VkConnector.Methods.Groups.SetCallbackSettings(tokenValue, idGroup, idServer);

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