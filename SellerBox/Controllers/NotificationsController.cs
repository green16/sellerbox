using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SellerBox.Common;
using SellerBox.Common.Services;
using SellerBox.Models.Database;
using SellerBox.ViewModels.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SellerBox.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly UserHelperService _userHelperService;

        private readonly Dictionary<int, string> NotificationElementTypes = new Dictionary<int, string>()
        {
            { 0, "Сценарий" },
            { 1, "Цепочка" },
            { 2, "Шаг цепочки" }
        };
        private readonly Dictionary<int, string> NotificationTypes = new Dictionary<int, string>()
        {
            { 0, "Сообщение в вк" },
            { 1, "Электронная почта" }
        };


        public NotificationsController(DatabaseContext context, UserHelperService userHelperService)
        {
            _context = context;
            _userHelperService = userHelperService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.NotificationElementTypes = NotificationElementTypes;
            ViewBag.NotificationTypes = NotificationTypes;

            var groupInfo = _userHelperService.GetSelectedGroup(User);

            var notifications = await _context.Notifications
                .Where(x => x.IdGroup == groupInfo.Key)
                .Select(x => new IndexViewModel()
                {
                    Id = x.Id,
                    DtCreate = x.DtCreate.ToLocalTime(),
                    IsEnabled = x.IsEnabled,
                    Name = x.Name,
                    ElementType = x.ElementType,
                    IdElement = Guid.Parse(x.IdElement),
                    NotificationType = x.NotificationType,
                    NotifyTo = x.NotifyTo
                })
                .ToArrayAsync();

            var model = new List<IndexViewModel>();

            if (notifications.Length == 0)
                return View(model);

            model.AddRange(notifications);

            var scenariosNames = await _context.Scenarios.Where(x => model.Where(y => y.ElementType == 0).Select(y => y.IdElement).Contains(x.Id)).Select(x => new { x.Id, x.Name }).ToDictionaryAsync(x => x.Id, x => x.Name);
            var chainsNames = await _context.Chains.Where(x => model.Where(y => y.ElementType == 1).Select(y => y.IdElement).Contains(x.Id)).Select(x => new { x.Id, x.Name }).ToDictionaryAsync(x => x.Id, x => x.Name);
            var chainStepsNames = await _context.ChainContents.Where(x => model.Where(y => y.ElementType == 2).Select(y => y.IdElement).Contains(x.Id)).Select(x => new { x.Id, x.Chain.Name, x.Index }).ToDictionaryAsync(x => x.Id, x => string.Concat(x.Name, " - шаг ", x.Index));
            var vkNames = await _context.VkUsers.Where(x => model.Where(y => y.NotificationType == 0).Select(y => long.Parse(y.NotifyTo)).Contains(x.IdVk)).Select(x => new { x.IdVk, x.FirstName, x.LastName }).ToDictionaryAsync(x => x.IdVk, x => string.Concat(x.FirstName, " ", x.LastName));
            foreach (var item in model)
            {
                switch (item.ElementType)
                {
                    case 0:
                        {
                            item.ElementName = scenariosNames[item.IdElement];
                            break;
                        }
                    case 1:
                        {
                            item.ElementName = chainsNames[item.IdElement];
                            break;
                        }
                    case 2:
                        {
                            item.ElementName = chainStepsNames[item.IdElement];
                            break;
                        }
                }
                switch (item.NotificationType)
                {
                    case 0:
                        {
                            item.NotifyToUrl = string.Concat("https://vk.com/id", item.NotifyTo);
                            item.NotifyTo = vkNames[long.Parse(item.NotifyTo)];
                            break;
                        }
                    case 1:
                        {
                            item.NotifyToUrl = "mailto:" + item.NotifyTo;
                            break;
                        }
                }
            }

            return View(model);
        }

        public IActionResult Create()
        {
            var model = new EditViewModel();

            ViewBag.NotificationElementTypes = NotificationElementTypes;
            ViewBag.NotificationTypes = NotificationTypes;
            return View("Edit", model);
        }

        public async Task<IActionResult> EditById(Guid? idNotification)
        {
            if (!idNotification.HasValue)
                return RedirectToAction(nameof(Index));

            var notification = await _context.Notifications.FindAsync(idNotification);
            if (notification == null)
                return RedirectToAction(nameof(Create));
            var model = new EditViewModel()
            {
                Id = idNotification,
                ElementType = notification.ElementType,
                IdElement = notification.IdElement,
                Name = notification.Name,
                NotificationType = notification.NotificationType,
                NotifyTo = notification.NotifyTo
            };

            ViewBag.Elements = await GetElements(model.ElementType.Value);
            ViewBag.NotificationElementTypes = NotificationElementTypes;
            ViewBag.NotificationTypes = NotificationTypes;
            return View("Edit", model);
        }

        private async Task<Dictionary<Guid, string>> GetElements(int elementType)
        {
            var groupInfo = _userHelperService.GetSelectedGroup(User);
            Dictionary<Guid, string> result = null;
            switch (elementType)
            {
                case 0:
                    {
                        result = await _context.Scenarios.Where(x => x.IdGroup == groupInfo.Key).ToDictionaryAsync(x => x.Id, x => x.Name);
                        break;
                    }
                case 1:
                    {
                        result = await _context.Chains.Where(x => x.IdGroup == groupInfo.Key).ToDictionaryAsync(x => x.Id, x => x.Name);
                        break;
                    }
                case 2:
                    {
                        result = await _context.ChainContents.Where(x => x.Chain.IdGroup == groupInfo.Key).Select(x => new { x.Id, Name = string.Concat(x.Chain.Name, " - шаг ", x.Index) }).ToDictionaryAsync(x => x.Id, x => x.Name);
                        break;
                    }
            }

            return result;
        }

        [HttpPost]
        public async Task<IActionResult> GetElements([FromQuery]int? elementType)
        {
            if (!elementType.HasValue)
                return Json(new { error = 1 });

            var elements = await GetElements(elementType.Value);

            return Json(elements);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(EditViewModel editViewModel)
        {
            var groupInfo = _userHelperService.GetSelectedGroup(User);

            Notifications notification = null;
            if (editViewModel.Id.HasValue)
                notification = await _context.Notifications.FindAsync(editViewModel.Id);
            if (notification == null)
                notification = new Notifications()
                {
                    DtCreate = DateTime.UtcNow,
                };
            notification.ElementType = editViewModel.ElementType.Value;
            notification.IdGroup = groupInfo.Key;
            notification.IdElement = editViewModel.IdElement;
            notification.IsEnabled = true;
            notification.Name = editViewModel.Name;
            notification.NotificationType = editViewModel.NotificationType.Value;

            switch (editViewModel.NotificationType)
            {
                case 0:
                    {
                        notification.NotifyTo = (await _userHelperService.GetUserIdVk(User)).ToString();
                        break;
                    }
                case 1:
                    {
                        notification.NotifyTo = editViewModel.NotifyTo;
                        break;
                    }
            }

            _context.Update(notification);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
