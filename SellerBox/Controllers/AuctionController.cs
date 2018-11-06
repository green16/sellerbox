using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SellerBox.Common;
using SellerBox.Common.Services;
using SellerBox.Models.Database;
using System;
using System.Threading.Tasks;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace SellerBox.Controllers
{
    [Authorize]
    public class AuctionController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly UserHelperService _userHelperService;
        private readonly IConfiguration _configuration;

        public AuctionController(IConfiguration configuration, DatabaseContext context, UserHelperService userHelperService, VkPoolService vkPoolService)
        {
            _configuration = configuration;
            _context = context;
            _userHelperService = userHelperService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(_context.Auctions);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.AppId = _configuration.GetValue<ulong>("VkApplicationId");
            ViewBag.OwnerId = -_userHelperService.GetSelectedGroup(User).Key;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Auction auction)
        {
            await _context.Auctions.AddAsync(auction);
            var idVkUser = await _userHelperService.GetUserIdVk(User);
            var groupId = _userHelperService.GetSelectedGroup(User).Key;
            var vkAppId = _configuration.GetValue<ulong>("VkApplicationId");
            var token = await _userHelperService.GeVktUserAccessToken(User);
            Console.WriteLine($"id user = {idVkUser}");
            Console.WriteLine($"id user = {groupId}");

            try
            {
                var vkApi = new VkApi();
                vkApi.Authorize(new ApiAuthParams
                {
                    ApplicationId = vkAppId,
                    UserId = idVkUser,
                    AccessToken = token
                });
                var post = vkApi.Wall.Post(new WallPostParams
                {
                    OwnerId = -groupId,
                    Message = auction.Name + "\r\n" + auction.Description + "\r\n" +
                              $"начальна€ цена - {auction.StartPrice}, минимальный шаг - {auction.PriceStep}, врем€ на повышение ставок - {auction.EndDate} \r\n" +
                              "ѕредлагайте свою цену в комментари€х \r\n",
                    FromGroup = true
                });
                _context.SaveChanges();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Create2(Auction auction)
        {
            await _context.Auctions.AddAsync(auction);
            var vkappId = _configuration.GetValue<ulong>("VkApplicationId");
            var groupId = _userHelperService.GetSelectedGroup(User).Key;
            string email = "";
            string password = "";
            Settings settings = Settings.All;

            var api = new VkApi();
            api.Authorize(new ApiAuthParams
            {
                ApplicationId = vkappId,
                Login = email,
                Password = password,
                Settings = settings
            });
            var groups = api.Groups.Get(new GroupsGetParams
            {
                UserId = api.UserId,
                Filter = GroupsFilters.Administrator
            });
            var post = api.Wall.Post(new WallPostParams
            {
                OwnerId = -groupId,
                FromGroup = true,
                Message = "test"
            });
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (!id.HasValue)
                return NotFound();

            var auction = await _context.Auctions.FindAsync(id);
            if (auction == null)
                return RedirectToAction(nameof(Index));

            return View(auction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind] Auction auction)
        {
            if (id != auction.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Auctions.Update(auction);
                return RedirectToAction(nameof(Index));
            }

            return View(auction);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var auction = await _context.Auctions.FindAsync(id);
            if (auction == null)
                return NotFound();

            return View(auction);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var auction = await _context.Auctions.FindAsync(id);
            _context.Auctions.Remove(auction);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}