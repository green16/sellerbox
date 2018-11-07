using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SellerBox.Common;
using SellerBox.Common.Services;
using SellerBox.Models.Database;
using SellerBox.ViewModels.ShortUrls;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SellerBox.Controllers
{
    [Authorize]
    public class ShortUrlsController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly UserHelperService _userHelperService;

        public ShortUrlsController(DatabaseContext context, UserHelperService userHelperService)
        {
            _context = context;
            _userHelperService = userHelperService;
        }

        [HttpPost]
        public async Task<IActionResult> GetList()
        {
            var groupInfo = _userHelperService.GetSelectedGroup(User);

            var shortLinks = await _context.ShortUrls.Where(x => x.IdGroup == groupInfo.Key).Select(x => new { x.Id, x.Name }).ToDictionaryAsync(x => x.Id, x => x.Name);

            return Json(shortLinks);
        }

        public async Task<IActionResult> Index()
        {
            var groupInfo = _userHelperService.GetSelectedGroup(User);

            var model = await _context.ShortUrls.Where(x => x.IdGroup == groupInfo.Key).Select(x => new IndexViewModel()
            {
                Id = x.Id,
                Name = x.Name,
                RedirectTo = x.RedirectTo
            }).ToArrayAsync();

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var groupInfo = _userHelperService.GetSelectedGroup(User);

            var model = new EditViewModel();

            ViewBag.Chains = await _context.Chains
                .Where(x => x.IdGroup == groupInfo.Key)
                .Select(x => new { x.Id, x.Name })
                .ToDictionaryAsync(x => x.Id, x => x.Name);

            return View(nameof(Edit), model);
        }

        public async Task<IActionResult> Edit(Guid? idShortLink)
        {
            if (!idShortLink.HasValue)
                return RedirectToAction(nameof(Index));

            var shortUrl = await _context.ShortUrls.FindAsync(idShortLink);
            if (shortUrl == null)
                return RedirectToAction(nameof(Index));
            var model = new EditViewModel()
            {
                Id = shortUrl.Id,
                IsSingleClick = shortUrl.IsSingleClick,
                IsSubscriberRequired = shortUrl.IsSubscriberRequired,
                Name = shortUrl.Name,
                RedirectTo = shortUrl.RedirectTo,
                AddToChain = shortUrl.IdChain.HasValue,
                IdChain = shortUrl.IdChain
            };

            var groupInfo = _userHelperService.GetSelectedGroup(User);

            ViewBag.Chains = await _context.Chains
                .Where(x => x.IdGroup == groupInfo.Key)
                .Select(x => new { x.Id, x.Name })
                .ToDictionaryAsync(x => x.Id, x => x.Name);

            return View(nameof(Edit), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            if (!_userHelperService.HasSelectedGroup(User))
                return RedirectToAction(nameof(GroupsController.Index), "Groups");

            var groupInfo = _userHelperService.GetSelectedGroup(User);

            ViewBag.Chains = await _context.Chains
                .Where(x => x.IdGroup == groupInfo.Key)
                .Select(x => new { x.Id, x.Name })
                .ToDictionaryAsync(x => x.Id, x => x.Name);

            return View(nameof(Edit), model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromQuery]Guid? idShortLink)
        {
            if (!idShortLink.HasValue)
                return NotFound();

            var removingShortLink = await _context.ShortUrls.FirstOrDefaultAsync(x => x.Id == idShortLink.Value);
            if (removingShortLink == null)
                return NotFound(idShortLink);

            _context.ShortUrls.RemoveRange(_context.ShortUrls.Where(x => x.Id == idShortLink.Value));
            await _context.SaveChangesAsync();

            return Json(new { state = 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(EditViewModel model)
        {
            if (!ModelState.IsValid)
                return await Edit(model);

            var groupInfo = _userHelperService.GetSelectedGroup(User);

            ShortUrls shortUrl = null;
            if (!model.Id.HasValue)
            {
                shortUrl = new ShortUrls()
                {
                    IdGroup = groupInfo.Key
                };
                await _context.ShortUrls.AddAsync(shortUrl);
            }
            else
                shortUrl = await _context.ShortUrls.FindAsync(model.Id.Value);

            shortUrl.Name = model.Name;
            shortUrl.RedirectTo = model.RedirectTo;
            shortUrl.IsSubscriberRequired = model.IsSubscriberRequired;
            shortUrl.IsSingleClick = model.IsSingleClick;
            shortUrl.IdChain = (model.AddToChain && model.IsSubscriberRequired) ? model.IdChain : null;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
