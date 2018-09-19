using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Common;
using WebApplication1.Common.Helpers;
using WebApplication1.Common.Services;
using WebApplication1.Models.Database;
using WebApplication1.ViewModels.Reposts;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class RepostsController : Controller
    {
        private readonly UserHelperService _userHelperService;
        private readonly DatabaseContext _context;
        private readonly VkPoolService _vkPoolService;

        public RepostsController(DatabaseContext context, UserHelperService userHelperService, VkPoolService vkPoolService)
        {
            _context = context;
            _userHelperService = userHelperService;
            _vkPoolService = vkPoolService;
        }

        public async Task<IActionResult> Index()
        {
            if (!_userHelperService.HasSelectedGroup(User))
                return RedirectToAction(nameof(GroupsController.Index), "Groups");
            var selectedGroup = _userHelperService.GetSelectedGroup(User);

            var model = await _context.RepostScenarios
                .Include(x => x.WallPost)
                .Where(x => x.WallPost.IdGroup == selectedGroup.Key)
                .Include(x => x.CheckingChainContent)
                .Include(x => x.CheckingChainContent.Chain)
                .Include(x => x.CheckingChainContent.Message)
                .Select(x => new IndexRepostScenarioViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    IdCheckingChain = x.CheckingChainContent.IdChain,
                    CheckingChainName = x.CheckingChainContent.Chain.Name,
                    HasGoToChain = x.IdGoToChain.HasValue,
                    HasGoToChain2 = x.IdGoToChain2.HasValue,
                    GoToChainName = x.GoToChain != null ? x.GoToChain.Name : string.Empty,
                    GoToChain2Name = x.GoToChain2 != null ? x.GoToChain2.Name : string.Empty,
                    MessageIndex = x.CheckingChainContent.Index,
                    MessageTextPart = x.CheckingChainContent.Message.TextPart,
                    PostLink = new Uri($"https://vk.com/wall-{selectedGroup.Key}_{x.WallPost.IdVk}")
                }).ToArrayAsync();

            return View("Index", model);
        }

        public async Task<IActionResult> Create()
        {
            if (!_userHelperService.HasSelectedGroup(User))
                return RedirectToAction(nameof(GroupsController.Index), "Groups");

            var model = new EditRepostScenarioViewModel();

            var selectedGroup = _userHelperService.GetSelectedGroup(User);

            ViewBag.Chains = await _context.Chains
                .Where(x => x.IdGroup == selectedGroup.Key)
                .ToDictionaryAsync(x => x.Id, x => x.Name);

            ViewBag.Posts = await _context.WallPosts
                .Where(x => x.IdGroup == selectedGroup.Key)
                .OrderByDescending(x => x.DtAdd)
                .ToDictionaryAsync(x => x.Id, x => $"{x.DtAdd:dd.MM.yyyy HH.mm}: {x.TextPart}");

            return View("Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EditRepostScenarioViewModel model)
        {
            if (!_userHelperService.HasSelectedGroup(User))
                return RedirectToAction(nameof(GroupsController.Index), "Groups");

            var selectedGroup = _userHelperService.GetSelectedGroup(User);

            ViewBag.Chains = await _context.Chains
                .Where(x => x.IdGroup == selectedGroup.Key)
                .ToDictionaryAsync(x => x.Id, x => x.Name);

            ViewBag.Posts = await _context.WallPosts
                .Where(x => x.IdGroup == selectedGroup.Key)
                .OrderByDescending(x => x.DtAdd)
                .ToDictionaryAsync(x => x.Id, x => $"{x.DtAdd:dd.MM.yyyy HH.mm}: {x.TextPart}");

            return View("Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditRepostScenarioViewModel model)
        {
            var selectedGroup = _userHelperService.GetSelectedGroup(User);

            ViewBag.Chains = await _context.Chains
                .Where(x => x.IdGroup == selectedGroup.Key)
                .ToDictionaryAsync(x => x.Id, x => x.Name);

            if (model.IdCheckingChain.HasValue)
                ViewBag.ChainContents = await _context.ChainContents
                    .Where(x => x.IdChain == model.IdCheckingChain.Value)
                    .Include(x => x.Message)
                    .ToDictionaryAsync(x => x.Id, x => x.Message.TextPart);

            ViewBag.Posts = await _context.WallPosts
                .Where(x => x.IdGroup == selectedGroup.Key)
                .OrderByDescending(x => x.DtAdd)
                .ToDictionaryAsync(x => x.Id, x => $"{x.DtAdd:dd.MM.yyyy HH.mm}: {x.TextPart}");

            return View("Edit", model);
        }

        public async Task<IActionResult> EditById(Guid? idRepostScenario)
        {
            if (!idRepostScenario.HasValue)
                return NotFound();

            var model = await _context.RepostScenarios
                .Include(x => x.CheckingChainContent)
                .Where(x => x.Id == idRepostScenario.Value)
                .Select(x => new EditRepostScenarioViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    CheckAfterHours = x.CheckAfter.Hours,
                    CheckAfterMinutes = (byte)x.CheckAfter.Minutes,
                    CheckAllPosts = x.CheckAllPosts,
                    CheckLastPosts = x.CheckLastPosts,
                    LastPostsCount = x.LastPostsCount,
                    IdCheckingChain = x.CheckingChainContent.IdChain,
                    IdCheckingChainContent = x.IdCheckingChainContent,
                    IdGoToChain = x.IdGoToChain,
                    IdGoToChain2 = x.IdGoToChain2,
                    IdPost = x.IdPost
                }).FirstOrDefaultAsync();

            var selectedGroup = _userHelperService.GetSelectedGroup(User);

            ViewBag.Chains = await _context.Chains
                .Where(x => x.IdGroup == selectedGroup.Key)
                .ToDictionaryAsync(x => x.Id, x => x.Name);

            if (model.IdCheckingChain.HasValue)
                ViewBag.ChainContents = await DbHelper.GetChainContents(_context, model.IdCheckingChain.Value).ToDictionaryAsync(x => x.Id, x => x.Message.TextPart);

            ViewBag.Posts = await _context.WallPosts
                .Where(x => x.IdGroup == selectedGroup.Key)
                .OrderByDescending(x => x.DtAdd)
                .ToDictionaryAsync(x => x.Id, x => $"{x.DtAdd:dd.MM.yyyy HH.mm}: {x.TextPart}");
            return View("Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(EditRepostScenarioViewModel model)
        {
            if (!ModelState.IsValid)
                return await Edit(model);

            var selectedGroup = _userHelperService.GetSelectedGroup(User);

            RepostScenarios newRepostScenario = null;
            if (model.Id.HasValue)
                newRepostScenario = await _context.RepostScenarios.FirstOrDefaultAsync(x => x.Id == model.Id.Value);

            if (newRepostScenario == null)
            {
                newRepostScenario = new RepostScenarios()
                {
                    IsEnabled = true
                };
                await _context.RepostScenarios.AddAsync(newRepostScenario);
            }

            newRepostScenario.Name = model.Name;
            newRepostScenario.CheckAfterSeconds = model.CheckAfterHours * 60 * 60 + model.CheckAfterMinutes * 60;
            if (!model.CheckLastPosts)
            {
                newRepostScenario.CheckLastPosts = false;
                newRepostScenario.CheckAllPosts = false;
                newRepostScenario.LastPostsCount = null;
                newRepostScenario.IdPost = model.IdPost;
            }
            else
            {
                newRepostScenario.CheckLastPosts = true;
                newRepostScenario.CheckAllPosts = model.CheckAllPosts;
                newRepostScenario.LastPostsCount = model.CheckAllPosts ? null : model.LastPostsCount;
                newRepostScenario.IdPost = null;
            }
            newRepostScenario.IdCheckingChainContent = model.IdCheckingChainContent.Value;
            newRepostScenario.IdGoToChain = model.IdGoToChain;
            newRepostScenario.IdGoToChain2 = model.IdGoToChain2;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromQuery]Guid? idRepostScenario)
        {
            if (!idRepostScenario.HasValue)
                return NotFound(idRepostScenario);

            RepostScenarios removingRepostScenario = await _context.RepostScenarios.FirstOrDefaultAsync(x => x.Id == idRepostScenario.Value);
            if (removingRepostScenario == null)
                return NotFound(idRepostScenario);

            _context.RepostScenarios.Remove(removingRepostScenario);
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
        public async Task<IActionResult> RefreshPosts()
        {
            DateTime? dtExpiresAt = await _userHelperService.GetVkUserAccessTokenExpiresAt(User);
            string accessToken = await _userHelperService.GeVktUserAccessToken(User);
            if (string.IsNullOrEmpty(accessToken) || !dtExpiresAt.HasValue || dtExpiresAt < DateTime.Now)
                return Json(new { error = 1, redirectUrl = "/ExternalLogin/Account?provider=Vkontakte&returnUrl=/Reposts/Index" });

            var selectedGroup = _userHelperService.GetSelectedGroup(User);

            var existsPostsIds = await _context.WallPosts
                .Where(x => x.IdGroup == selectedGroup.Key)
                .OrderByDescending(x => x.DtAdd)
                .Select(x => x.IdVk)
                .ToArrayAsync();

            var vkApi = await _vkPoolService.GetGroupVkApi(selectedGroup.Key);

            var posts = await vkApi.Wall.GetAsync(new VkNet.Model.RequestParams.WallGetParams()
            {
                OwnerId = selectedGroup.Key
            });
            var newWallPosts = posts.WallPosts
                .Where(x => !existsPostsIds.Contains(x.Id.Value))
                .Select(x => new WallPosts()
                {
                    DtAdd = DateTime.UtcNow,
                    IdGroup = selectedGroup.Key,
                    IdVk = x.Id.Value,
                    Text = x.Text
                });

            await _context.WallPosts.AddRangeAsync(newWallPosts);
            await _context.SaveChangesAsync();

            var allPosts = await _context.WallPosts
                .Where(x => x.IdGroup == selectedGroup.Key)
                .OrderByDescending(x => x.DtAdd)
                .ToDictionaryAsync(x => x.Id, x => x.TextPart);

            return Json(new { error = 0, posts = allPosts });
        }
    }
}