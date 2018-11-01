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

        public IActionResult Create()
        {
            var model = new EditViewModel();
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
                Name = shortUrl.Name,
                RedirectTo = shortUrl.RedirectTo
            };
            return View(nameof(Edit), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EditViewModel model)
        {
            if (!_userHelperService.HasSelectedGroup(User))
                return RedirectToAction(nameof(GroupsController.Index), "Groups");

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
                return Edit(model);

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

            shortUrl.IsSingleClick = model.IsSingleClick;
            shortUrl.Name = model.Name;
            shortUrl.RedirectTo = model.RedirectTo;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
