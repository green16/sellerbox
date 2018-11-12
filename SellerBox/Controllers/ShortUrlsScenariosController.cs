using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SellerBox.Common;
using SellerBox.Common.Helpers;
using SellerBox.Common.Services;
using SellerBox.Models.Database;
using SellerBox.ViewModels.ShortUrlsScenarios;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SellerBox.Controllers
{
    [Authorize]
    public class ShortUrlsScenariosController : Controller
    {
        private readonly UserHelperService _userHelperService;
        private readonly DatabaseContext _context;

        public ShortUrlsScenariosController(DatabaseContext context, UserHelperService userHelperService)
        {
            _context = context;
            _userHelperService = userHelperService;
        }

        public async Task<IActionResult> Index()
        {
            if (!_userHelperService.HasSelectedGroup(User))
                return RedirectToAction(nameof(GroupsController.Index), "Groups");
            var selectedGroup = _userHelperService.GetSelectedGroup(User);

            var model = await _context.ShortUrlsScenarios
                .Where(x => x.ShortUrl.IdGroup == selectedGroup.Key)
                .Select(x => new IndexViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsEnabled = x.IsEnabled,
                    CheckingChainName = x.CheckingChainContent.Chain.Name,
                    CheckIsSubscriber = x.CheckIsSubscriber,
                    MessageIndex = x.CheckingChainContent.Index,
                    MessageText = x.CheckingChainContent.Message.Text,
                    ShortUrlName = x.ShortUrl.Name
                }).ToArrayAsync();

            return View(nameof(Index), model);
        }

        public async Task<IActionResult> Create()
        {
            if (!_userHelperService.HasSelectedGroup(User))
                return RedirectToAction(nameof(GroupsController.Index), "Groups");

            var model = new EditViewModel();

            var selectedGroup = _userHelperService.GetSelectedGroup(User);

            ViewBag.Chains = await _context.Chains
                .Where(x => x.IdGroup == selectedGroup.Key)
                .ToDictionaryAsync(x => x.Id, x => x.Name);

            ViewBag.ShortUrls = await _context.ShortUrls
                .Where(x => x.IdGroup == selectedGroup.Key)
                .OrderByDescending(x => x.DtAdd)
                .ToDictionaryAsync(x => x.Id, x => x.Name);

            return View("Edit", model);
        }

        public async Task<IActionResult> Edit(Guid? idShortUrlScenario)
        {
            if (!idShortUrlScenario.HasValue)
                RedirectToAction(nameof(Index));

            var selectedGroup = _userHelperService.GetSelectedGroup(User);

            var model = await _context.ShortUrlsScenarios
                .Where(x => x.Id == idShortUrlScenario.Value)
                .Select(x => new EditViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    CheckIsSubscriber = x.CheckIsSubscriber,
                    CheckAfterHours = x.CheckAfter.Hours + x.CheckAfter.Days * 24,
                    CheckAfterMinutes = (byte)x.CheckAfter.Minutes,
                    IdCheckingChain = x.CheckingChainContent.IdChain,
                    IdCheckingChainContent = x.IdCheckingChainContent,
                    IdGoToChain = x.IdGoToChain,
                    IdGoToErrorChain1 = x.IdGoToErrorChain1,
                    IdGoToErrorChain2 = x.IdGoToErrorChain2,
                    IdGoToErrorChain3 = x.IdGoToErrorChain3,
                    IdShortUrl = x.IdShortUrl
                }).FirstOrDefaultAsync();

            ViewBag.Chains = await _context.Chains
                .Where(x => x.IdGroup == selectedGroup.Key)
                .ToDictionaryAsync(x => x.Id, x => x.Name);

            if (model.IdCheckingChain.HasValue)
                ViewBag.ChainContents = await DbHelper.GetChainContents(_context, model.IdCheckingChain.Value).ToDictionaryAsync(x => x.Id, x => x.Message.TextPart);

            ViewBag.ShortUrls = await _context.ShortUrls
                .Where(x => x.IdGroup == selectedGroup.Key)
                .OrderByDescending(x => x.DtAdd)
                .ToDictionaryAsync(x => x.Id, x => x.Name);

            return View("Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            var selectedGroup = _userHelperService.GetSelectedGroup(User);

            ViewBag.Chains = await _context.Chains
                .Where(x => x.IdGroup == selectedGroup.Key)
                .ToDictionaryAsync(x => x.Id, x => x.Name);

            if (model.IdCheckingChain.HasValue)
                ViewBag.ChainContents = await DbHelper.GetChainContents(_context, model.IdCheckingChain.Value).ToDictionaryAsync(x => x.Id, x => x.Message.TextPart);

            ViewBag.ShortUrls = await _context.ShortUrls
                .Where(x => x.IdGroup == selectedGroup.Key)
                .OrderByDescending(x => x.DtAdd)
                .ToDictionaryAsync(x => x.Id, x => x.Name);

            return View("Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(EditViewModel model)
        {
            if (!ModelState.IsValid)
                return await Edit(model);

            var selectedGroup = _userHelperService.GetSelectedGroup(User);

            ShortUrlsScenarios newShortUrlsScenario = null;
            if (model.Id.HasValue)
                newShortUrlsScenario = await _context.ShortUrlsScenarios.FirstOrDefaultAsync(x => x.Id == model.Id.Value);

            if (newShortUrlsScenario == null)
            {
                newShortUrlsScenario = new ShortUrlsScenarios()
                {
                    IsEnabled = true,
                    DtCreate = DateTime.UtcNow
                };
                await _context.ShortUrlsScenarios.AddAsync(newShortUrlsScenario);
            }

            newShortUrlsScenario.Name = model.Name;
            newShortUrlsScenario.CheckAfterSeconds = model.CheckAfterHours * 60 * 60 + model.CheckAfterMinutes * 60;

            newShortUrlsScenario.IdShortUrl = model.IdShortUrl;

            newShortUrlsScenario.IdCheckingChainContent = model.IdCheckingChainContent.Value;
            newShortUrlsScenario.IdGoToChain = model.IdGoToChain;
            newShortUrlsScenario.IdGoToErrorChain1 = model.IdGoToErrorChain1;

            newShortUrlsScenario.CheckIsSubscriber = model.CheckIsSubscriber;
            newShortUrlsScenario.IdGoToErrorChain2 = model.CheckIsSubscriber ? model.IdGoToErrorChain2 : null;
            newShortUrlsScenario.IdGoToErrorChain3 = model.CheckIsSubscriber ? model.IdGoToErrorChain3 : null;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromQuery]Guid? idRepostScenario)
        {
            if (!idRepostScenario.HasValue)
                return NotFound(idRepostScenario);

            var removingShortUrlsScenario = await _context.ShortUrlsScenarios.FirstOrDefaultAsync(x => x.Id == idRepostScenario.Value);
            if (removingShortUrlsScenario == null)
                return NotFound(idRepostScenario);

            _context.ShortUrlsScenarios.Remove(removingShortUrlsScenario);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> GetChainContents([FromBody]Guid idChain)
        {
            var chainContents = DbHelper.GetChainContents(_context, idChain).OrderBy(x => x.Index).ToDictionaryAsync(x => x.Id, x => x.Message.TextPart);
            return Json(await chainContents);
        }

        [HttpPost]
        public async Task<IActionResult> ToogleIsEnabled([FromQuery]Guid? idShortUrlScenario)
        {
            if (!idShortUrlScenario.HasValue)
                return Json(new { error = 1 });

            var scenario = await _context.ShortUrlsScenarios.FirstOrDefaultAsync(x => x.Id == idShortUrlScenario.Value);
            scenario.IsEnabled = !scenario.IsEnabled;
            await _context.SaveChangesAsync();

            return Json(new { error = 0, scenario.IsEnabled });
        }
    }
}
