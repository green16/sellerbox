using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Common;
using WebApplication1.Common.Helpers;
using WebApplication1.Common.Services;
using WebApplication1.Models.Database;
using WebApplication1.ViewModels.Subscribers;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class SubscribersController : Controller
    {
        private readonly UserHelperService _userHelperService;
        private readonly DatabaseContext _context;

        public SubscribersController(UserHelperService userManager, DatabaseContext context)
        {
            _userHelperService = userManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, string sortExpression = nameof(AllSubscribersViewModel.DtAdd))
        {
            if (!_userHelperService.HasSelectedGroup(User))
                return RedirectToAction(nameof(GroupsController.Index), "Groups");

            var groupInfo = _userHelperService.GetSelectedGroup(User);
            ViewBag.GroupName = groupInfo.Value;
            var subscribers = DbHelper.GetSubscribersInGroup(_context, groupInfo.Key)
                .Select(x => new AllSubscribersViewModel()
                {
                    Id = x.Id,
                    IdVk = x.IdVkUser,
                    Birthday = x.VkUser.Birthday,
                    City = x.VkUser.City,
                    Country = x.VkUser.Country,
                    DtAdd = x.DtAdd,
                    FIO = $"{x.VkUser.LastName} {x.VkUser.FirstName}",
                    Link = new Uri(x.VkUser.Link),
                    Photo = new Uri(x.VkUser.PhotoSquare50),
                    Sex = x.VkUser.Sex,
                    IsSubscriber = x.IsSubscribedToGroup,
                    IsChatAllowed = x.IsChatAllowed
                });

            var model = await PagingList.CreateAsync(subscribers, nameof(Index), "Subscribers", 20, page, sortExpression, nameof(AllSubscribersViewModel.DtAdd));

            ViewBag.IdGroup = groupInfo.Key;
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Members(int page = 1, string sortExpression = nameof(MemberIndexViewModel.DtAdd))
        {
            if (!_userHelperService.HasSelectedGroup(User))
                return RedirectToAction(nameof(GroupsController.Index), "Groups");

            var groupInfo = _userHelperService.GetSelectedGroup(User);
            ViewBag.GroupName = groupInfo.Value;
            var members = DbHelper.GetSubscribersInGroup(_context, groupInfo.Key)
                .Where(x => x.IsChatAllowed.HasValue && x.IsChatAllowed.Value)
                .Select(x => new MemberIndexViewModel()
                {
                    Id = x.Id,
                    IdVk = x.IdVkUser,
                    Birthday = x.VkUser.Birthday,
                    City = x.VkUser.City,
                    Country = x.VkUser.Country,
                    DtAdd = x.DtAdd,
                    FIO = $"{x.VkUser.LastName} {x.VkUser.FirstName}",
                    Link = new Uri(x.VkUser.Link),
                    Photo = new Uri(x.VkUser.PhotoSquare50),
                    Sex = x.VkUser.Sex,
                    IsSubscriber = x.IsSubscribedToGroup
                });

            var model = await PagingList.CreateAsync(members, nameof(Members), "Subscribers", 20, page, sortExpression, nameof(MemberIndexViewModel.DtAdd));
            ViewBag.IdGroup = groupInfo.Key;
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Subscribers(int page = 1, string sortExpression = nameof(SubscriberIndexViewModel.DtAdd))
        {
            if (!_userHelperService.HasSelectedGroup(User))
                return RedirectToAction(nameof(GroupsController.Index), "Groups");

            var groupInfo = _userHelperService.GetSelectedGroup(User);
            ViewBag.GroupName = groupInfo.Value;

            var subscribers = DbHelper.GetSubscribersInGroup(_context, groupInfo.Key)
                .Where(x => x.IsSubscribedToGroup.HasValue && x.IsSubscribedToGroup.Value)
                .Select(x => new SubscriberIndexViewModel()
                {
                    Id = x.Id,
                    IdVk = x.IdVkUser,
                    Birthday = x.VkUser.Birthday,
                    City = x.VkUser.City,
                    Country = x.VkUser.Country,
                    DtAdd = x.DtAdd,
                    FIO = $"{x.VkUser.LastName} {x.VkUser.FirstName}",
                    Link = new Uri(x.VkUser.Link),
                    Photo = new Uri(x.VkUser.PhotoSquare50),
                    Sex = x.VkUser.Sex,
                    IsChatAllowed = x.IsChatAllowed
                });

            var model = await PagingList.CreateAsync(subscribers, nameof(Subscribers), "Subscibers", 20, page, sortExpression, nameof(SubscriberIndexViewModel.DtAdd));
            ViewBag.IdGroup = groupInfo.Key;
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Unmembered(int page = 1, string sortExpression = nameof(UnmemberedIndexViewModel.DtAdd))
        {
            if (!_userHelperService.HasSelectedGroup(User))
                return RedirectToAction(nameof(GroupsController.Index), "Groups");

            var groupInfo = _userHelperService.GetSelectedGroup(User);
            var members = _context.Subscribers
                .Include(x => x.VkUser)
                .Where(x => x.IdGroup == groupInfo.Key && x.IsChatAllowed.HasValue && !x.IsChatAllowed.Value)
                .Select(x => new UnmemberedIndexViewModel()
                {
                    Id = x.Id,
                    IdVk = x.IdVkUser,
                    Birthday = x.VkUser.Birthday,
                    City = x.VkUser.City,
                    Country = x.VkUser.Country,
                    DtAdd = x.DtAdd,
                    FIO = $"{x.VkUser.LastName} {x.VkUser.FirstName}",
                    Link = new Uri(x.VkUser.Link),
                    Photo = new Uri(x.VkUser.PhotoSquare50),
                    Sex = x.VkUser.Sex,
                    IsSubscriber = x.IsSubscribedToGroup
                });

            var model = await PagingList.CreateAsync(members, nameof(Unmembered), "Subscribers", 20, page, sortExpression, nameof(UnmemberedIndexViewModel.DtAdd));
            ViewBag.IdGroup = groupInfo.Key;
            ViewBag.GroupName = groupInfo.Value;
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Unsubscribed(int page = 1, string sortExpression = nameof(UnsubscribedIndexViewModel.DtAdd))
        {
            if (!_userHelperService.HasSelectedGroup(User))
                return RedirectToAction(nameof(GroupsController.Index), "Groups");

            KeyValuePair<int, string> groupInfo = _userHelperService.GetSelectedGroup(User);
            ViewBag.GroupName = groupInfo.Value;
            var unsubscribed = _context.Subscribers
                .Include(x => x.VkUser)
                .Where(x => x.IdGroup == groupInfo.Key && x.IsUnsubscribed && x.DtUnsubscribe.HasValue)
                .Select(x => new UnsubscribedIndexViewModel()
                {
                    Id = x.Id,
                    IdVk = x.IdVkUser,
                    Birthday = x.VkUser.Birthday,
                    City = x.VkUser.City,
                    Country = x.VkUser.Country,
                    DtAdd = x.DtAdd,
                    FIO = $"{x.VkUser.LastName} {x.VkUser.FirstName}",
                    Link = new Uri(x.VkUser.Link),
                    Photo = new Uri(x.VkUser.PhotoSquare50),
                    Sex = x.VkUser.Sex,
                    DtUnsubscribe = x.DtUnsubscribe.Value,
                    IsChatAllowed = x.IsChatAllowed
                });

            var model = await PagingList.CreateAsync(unsubscribed, nameof(Unsubscribed), "Subscribers", 20, page, sortExpression, nameof(UnsubscribedIndexViewModel.DtAdd));

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Card(Guid idSubscriber)
        {
            var subscriber = await _context.Subscribers
                .Include(x => x.VkUser)
                .Where(x => x.Id == idSubscriber)
                .Select(x => new SubscriberCardViewModel()
                {
                    Birthday = x.VkUser.Birthday,
                    City = x.VkUser.City,
                    Country = x.VkUser.Country,
                    DtAdd = x.DtAdd,
                    DtUnsubscribe = x.DtUnsubscribe,
                    IdVk = x.IdVkUser,
                    IsBlocked = x.IsBlocked,
                    IsChatAllowed = x.IsChatAllowed,
                    IsSubscribedToGroup = x.IsSubscribedToGroup,
                    IsUnsubscribed = x.IsUnsubscribed,
                    Link = x.VkUser.Link,
                    Name = $"{x.VkUser.LastName} {x.VkUser.FirstName}",
                    Photo = x.VkUser.PhotoOrig400,
                    Sex = x.VkUser.Sex
                }).FirstOrDefaultAsync();
            subscriber.Segments = await _context.SubscribersInSegments
                .Where(x => x.IdSubscriber == idSubscriber)
                .Include(x => x.Segment)
                .Select(x => x.Segment.Name)
                .ToArrayAsync();

            ViewBag.IdSubscriber = idSubscriber;
            return View(nameof(Card), subscriber);
        }

        public async Task<IActionResult> RefreshFromVk(Guid idSubscriber)
        {
            var subscriber = await _context.Subscribers
                .Include(x => x.VkUser)
                .FirstOrDefaultAsync(x => x.Id == idSubscriber);

            var selectedGroup = _userHelperService.GetSelectedGroup(User);
            string groupAccessToken = _context.Groups.FirstOrDefault(x => x.IdVk == selectedGroup.Key)?.AccessToken;
            var vkUser = (await VkConnector.Methods.Users.Get(groupAccessToken, subscriber.IdVkUser)).FirstOrDefault();
            subscriber.VkUser.Update(vkUser);
            subscriber.IsBlocked = vkUser.IsBlacklisted;
            await _context.SaveChangesAsync();
            return await Card(idSubscriber);
        }

        [HttpGet]
        public async Task RefreshAllFromVk()
        {
            var allSubscribers = await _context.Subscribers.Include(x => x.VkUser).ToArrayAsync();
            foreach (var subscriber in allSubscribers)
            {
                string groupAccessToken = _context.Groups.FirstOrDefault(x => x.IdVk == subscriber.IdGroup)?.AccessToken;
                var vkUser = (await VkConnector.Methods.Users.Get(groupAccessToken, subscriber.IdVkUser)).FirstOrDefault();
                subscriber.VkUser.Update(vkUser);
                subscriber.IsBlocked = vkUser.IsBlacklisted;
                await _context.SaveChangesAsync();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CheckIsChatAllowed([FromBody]int idVkUser)
        {
            if (!_userHelperService.HasSelectedGroup(User))
                return RedirectToAction(nameof(GroupsController.Index), "Groups");

            var groupInfo = _userHelperService.GetSelectedGroup(User);
            string groupAccessToken = await _context.Groups.Where(x => x.IdVk == groupInfo.Key).Select(x => x.AccessToken).FirstOrDefaultAsync();

            bool isAllowed = await VkConnector.Methods.Messages.IsMessagesFromGroupAllowed(groupAccessToken, groupInfo.Key, idVkUser);

            Subscribers subscriber = await _context.Subscribers.FirstOrDefaultAsync(x => x.IdGroup == groupInfo.Key && x.IdVkUser == idVkUser);
            subscriber.IsChatAllowed = isAllowed;
            await _context.SaveChangesAsync();

            return Json(new { isAllowed });
        }

        [HttpPost]
        public async Task<IActionResult> CheckIsSubscriber([FromBody]int idVkUser)
        {
            if (!_userHelperService.HasSelectedGroup(User))
                return RedirectToAction(nameof(GroupsController.Index), "Groups");

            var groupInfo = _userHelperService.GetSelectedGroup(User);
            string groupAccessToken = await _context.Groups.Where(x => x.IdVk == groupInfo.Key).Select(x => x.AccessToken).FirstOrDefaultAsync();

            bool isSubscribed = await VkConnector.Methods.Groups.IsMember(groupAccessToken, groupInfo.Key, idVkUser);

            Subscribers subscriber = await _context.Subscribers.FirstOrDefaultAsync(x => x.IdGroup == groupInfo.Key && x.IdVkUser == idVkUser);
            subscriber.IsSubscribedToGroup = isSubscribed;
            await _context.SaveChangesAsync();

            return Json(new { isSubscribed });
        }

        [HttpGet]
        public async Task UpdateUserNames()
        {
            var allUsers = await _context.VkUsers.ToArrayAsync();
            foreach (var user in allUsers)
            {
                var index = user.FirstName.IndexOf(' ');
                if (index == -1)
                    continue;

                user.FirstName = new string(user.FirstName.Skip(index + 1).ToArray());
                user.LastName = new string(user.FirstName.Take(index).ToArray());

                await _context.SaveChangesAsync();
            }
        }
    }
}
