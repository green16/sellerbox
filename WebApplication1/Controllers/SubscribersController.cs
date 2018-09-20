using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Common;
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
        private readonly VkPoolService _vkPoolService;

        public SubscribersController(UserHelperService userManager, DatabaseContext context, VkPoolService vkPoolService)
        {
            _userHelperService = userManager;
            _context = context;
            _vkPoolService = vkPoolService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, string sortExpression = nameof(AllSubscribersViewModel.DtAdd))
        {
            if (!_userHelperService.HasSelectedGroup(User))
                return RedirectToAction(nameof(GroupsController.Index), "Groups");

            var groupInfo = _userHelperService.GetSelectedGroup(User);

            var subscribers = _context.Subscribers
                .Include(x => x.VkUser)
                .Where(x => x.IdGroup == groupInfo.Key && !x.IsUnsubscribed)
                .Select(x => new AllSubscribersViewModel()
                {
                    Id = x.Id,
                    IdVk = x.IdVkUser,
                    Birthday = x.VkUser.Birthday,
                    City = x.VkUser.City,
                    Country = x.VkUser.Country,
                    DtAdd = x.DtAdd,
                    FIO = $"{x.VkUser.LastName} {x.VkUser.FirstName}",
                    Link = x.VkUser.Link,
                    Photo = x.VkUser.PhotoSquare50,
                    Sex = x.VkUser.Sex,
                    IsSubscriber = x.IsSubscribedToGroup,
                    IsChatAllowed = x.IsChatAllowed
                });

            var model = await PagingList.CreateAsync(subscribers, nameof(Index), "Subscribers", 20, page, sortExpression, nameof(AllSubscribersViewModel.DtAdd));

            ViewBag.IdGroup = groupInfo.Key;
            ViewBag.GroupName = groupInfo.Value;
            ViewBag.LinkGroup = $"https://vk.com/public{groupInfo.Key}";
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Members(int page = 1, string sortExpression = nameof(MemberIndexViewModel.DtAdd))
        {
            if (!_userHelperService.HasSelectedGroup(User))
                return RedirectToAction(nameof(GroupsController.Index), "Groups");

            var groupInfo = _userHelperService.GetSelectedGroup(User);

            var members = _context.Subscribers
                .Where(x => x.IdGroup == groupInfo.Key && !x.IsUnsubscribed)
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
                    Link = x.VkUser.Link,
                    Photo = x.VkUser.PhotoSquare50,
                    Sex = x.VkUser.Sex,
                    IsSubscriber = x.IsSubscribedToGroup
                });

            var model = await PagingList.CreateAsync(members, nameof(Members), "Subscribers", 20, page, sortExpression, nameof(MemberIndexViewModel.DtAdd));

            ViewBag.IdGroup = groupInfo.Key;
            ViewBag.GroupName = groupInfo.Value;
            ViewBag.LinkGroup = $"https://vk.com/public{groupInfo.Key}";
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Subscribers(int page = 1, string sortExpression = nameof(SubscriberIndexViewModel.DtAdd))
        {
            if (!_userHelperService.HasSelectedGroup(User))
                return RedirectToAction(nameof(GroupsController.Index), "Groups");

            var groupInfo = _userHelperService.GetSelectedGroup(User);

            var subscribers = _context.Subscribers
                .Where(x => x.IdGroup == groupInfo.Key && !x.IsUnsubscribed)
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
                    Link = x.VkUser.Link,
                    Photo = x.VkUser.PhotoSquare50,
                    Sex = x.VkUser.Sex,
                    IsChatAllowed = x.IsChatAllowed
                });

            var model = await PagingList.CreateAsync(subscribers, nameof(Subscribers), "Subscibers", 20, page, sortExpression, nameof(SubscriberIndexViewModel.DtAdd));

            ViewBag.IdGroup = groupInfo.Key;
            ViewBag.GroupName = groupInfo.Value;
            ViewBag.LinkGroup = $"https://vk.com/public{groupInfo.Key}";
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Unmembered(int page = 1, string sortExpression = nameof(UnmemberedIndexViewModel.DtAdd))
        {
            if (!_userHelperService.HasSelectedGroup(User))
                return RedirectToAction(nameof(GroupsController.Index), "Groups");

            var groupInfo = _userHelperService.GetSelectedGroup(User);
            var members = _context.Subscribers
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
                    Link = x.VkUser.Link,
                    Photo = x.VkUser.PhotoSquare50,
                    Sex = x.VkUser.Sex,
                    IsSubscriber = x.IsSubscribedToGroup
                });

            var model = await PagingList.CreateAsync(members, nameof(Unmembered), "Subscribers", 20, page, sortExpression, nameof(UnmemberedIndexViewModel.DtAdd));

            ViewBag.IdGroup = groupInfo.Key;
            ViewBag.GroupName = groupInfo.Value;
            ViewBag.LinkGroup = $"https://vk.com/public{groupInfo.Key}";
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Unsubscribed(int page = 1, string sortExpression = nameof(UnsubscribedIndexViewModel.DtAdd))
        {
            if (!_userHelperService.HasSelectedGroup(User))
                return RedirectToAction(nameof(GroupsController.Index), "Groups");

            var groupInfo = _userHelperService.GetSelectedGroup(User);

            var unsubscribed = _context.Subscribers
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
                    Link = x.VkUser.Link,
                    Photo = x.VkUser.PhotoSquare50,
                    Sex = x.VkUser.Sex,
                    DtUnsubscribe = x.DtUnsubscribe.Value,
                    IsChatAllowed = x.IsChatAllowed
                });

            var model = await PagingList.CreateAsync(unsubscribed, nameof(Unsubscribed), "Subscribers", 20, page, sortExpression, nameof(UnsubscribedIndexViewModel.DtAdd));

            ViewBag.IdGroup = groupInfo.Key;
            ViewBag.GroupName = groupInfo.Value;
            ViewBag.LinkGroup = $"https://vk.com/public{groupInfo.Key}";
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

            var vkApi = await _vkPoolService.GetGroupVkApi(selectedGroup.Key);

            var vkUser = (await vkApi.Users.GetAsync(new long[] { subscriber.IdVkUser }, VkNet.Enums.Filters.ProfileFields.BirthDate |
                VkNet.Enums.Filters.ProfileFields.City |
                VkNet.Enums.Filters.ProfileFields.Country |
                VkNet.Enums.Filters.ProfileFields.FirstName |
                VkNet.Enums.Filters.ProfileFields.LastName |
                VkNet.Enums.Filters.ProfileFields.Nickname |
                VkNet.Enums.Filters.ProfileFields.Domain |
                VkNet.Enums.Filters.ProfileFields.Photo50 |
                VkNet.Enums.Filters.ProfileFields.Photo400Orig |
                VkNet.Enums.Filters.ProfileFields.Sex |
                VkNet.Enums.Filters.ProfileFields.Blacklisted)).FirstOrDefault();
            subscriber.VkUser.Update(vkUser);
            subscriber.IsBlocked = vkUser.Blacklisted;

            await _context.SaveChangesAsync();

            return await Card(idSubscriber);
        }

        [HttpGet]
        public async Task RefreshAllFromVk()
        {
            var allSubscribers = await _context.Subscribers.Include(x => x.VkUser).ToArrayAsync();
            int offset = 0, count = 100;
            do
            {
                var currentSubscribers = allSubscribers.Skip(offset).Take(count);

                var groupInfo = _userHelperService.GetSelectedGroup(User);
                var vkApi = await _vkPoolService.GetGroupVkApi(groupInfo.Key);

                var vkUsers = await vkApi.Users.GetAsync(currentSubscribers.Select(x => x.IdVkUser), VkNet.Enums.Filters.ProfileFields.BirthDate |
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
                foreach (var vkUser in vkUsers)
                {
                    var currentSubscriber = currentSubscribers.FirstOrDefault(x => x.IdVkUser == vkUser.Id);
                    currentSubscriber.VkUser.Update(vkUser);
                    currentSubscriber.IsBlocked = vkUser.Blacklisted;
                }
                await _context.SaveChangesAsync();

                offset += currentSubscribers.Count();
                if (offset >= allSubscribers.Length)
                    break;

            } while (offset < allSubscribers.Length);
        }

        [HttpPost]
        public async Task<IActionResult> CheckIsChatAllowed([FromBody]long idVkUser)
        {
            if (!_userHelperService.HasSelectedGroup(User))
                return RedirectToAction(nameof(GroupsController.Index), "Groups");

            var groupInfo = _userHelperService.GetSelectedGroup(User);
            var vkApi = await _vkPoolService.GetGroupVkApi(groupInfo.Key);

            bool isAllowed = await vkApi.Messages.IsMessagesFromGroupAllowedAsync((ulong)groupInfo.Key, (ulong)idVkUser);

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
            var vkApi = await _vkPoolService.GetGroupVkApi(groupInfo.Key);

            bool isSubscribed = (await vkApi.Groups.IsMemberAsync(groupInfo.Key.ToString(), idVkUser, Enumerable.Empty<long>(), false)).FirstOrDefault()?.Member ?? false;

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
