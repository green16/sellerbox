using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Common;
using WebApplication1.Common.Helpers;
using WebApplication1.Common.Services;
using WebApplication1.Models.Database;
using WebApplication1.ViewModels.Chains;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class ChainsController : Controller
    {
        private readonly UserHelperService _userHelperService;
        private readonly DatabaseContext _context;

        public ChainsController(DatabaseContext context, UserHelperService userHelperService)
        {
            _context = context;
            _userHelperService = userHelperService;
        }

        // GET: Chains
        public async Task<IActionResult> Index()
        {
            if (!_userHelperService.HasSelectedGroup(User))
                return RedirectToAction(nameof(GroupsController.Index), "Groups");
            var selectedGroup = _userHelperService.GetSelectedGroup(User);

            var model = await _context.Chains
                .Where(x => x.IdGroup == selectedGroup.Key)
                .Select(x => new IndexViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsEnabled = x.IsEnabled,
                    SubscribersInChain = _context.SubscribersInChains.Include(y => y.ChainStep).Count(y => !y.IsSended && y.ChainStep.IdChain == x.Id)
                }).ToListAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ToogleChain([FromQuery]Guid? idChain)
        {
            if (!idChain.HasValue)
                return Json(new { error = 1 });

            var chain = await _context.Chains.FirstOrDefaultAsync(x => x.Id == idChain);
            chain.IsEnabled = !chain.IsEnabled;
            await _context.SaveChangesAsync();

            return Json(new { });
        }

        // GET: Chains/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chains = await _context.Chains
                .SingleOrDefaultAsync(m => m.Id == id);
            if (chains == null)
            {
                return NotFound();
            }

            return View(chains);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            string idUser = _userHelperService.GetUserId(User);
            ChainViewModel newChain = new ChainViewModel();
            if (_userHelperService.HasSelectedGroup(User))
                newChain.IdGroup = _userHelperService.GetSelectedGroup(User).Key;

            ViewBag.Groups = await _context.GroupAdmins
                .Where(x => x.IdUser == idUser)
                .Include(x => x.Group)
                .Select(x => x.Group)
                .ToDictionaryAsync(x => x.IdVk, x => x.Name);
            return View(newChain);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ChainViewModel data)
        {
            string idUser = _userHelperService.GetUserId(User);

            ViewBag.Groups = await _context.GroupAdmins
                .Where(x => x.IdUser == idUser)
                .Include(x => x.Group)
                .Select(x => x.Group)
                .ToDictionaryAsync(x => x.IdVk, x => x.Name);
            return View(nameof(Create), data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveChain(ChainViewModel data)
        {
            if (!ModelState.IsValid)
                return await Create(data);

            var newChain = new Chains()
            {
                IdGroup = data.IdGroup.Value,
                IsEnabled = true,
                Name = data.Name
            };
            await _context.Chains.AddAsync(newChain);
            await _context.SaveChangesAsync();

            data.Id = newChain.Id;
            return RedirectToAction(nameof(Edit), data);
        }

        [HttpPost, HttpGet]
        public async Task<IActionResult> AddMessage(Guid? idChain)
        {
            if (!idChain.HasValue)
                return RedirectToAction(nameof(Index));

            var selectedGroup = _userHelperService.GetSelectedGroup(User);

            var addMessageViewModel = new EditMessageViewModel()
            {
                IdChain = idChain.Value
            };

            ViewBag.Chains = await _context.Chains
                .Where(x => x.IdGroup == selectedGroup.Key)
                .ToDictionaryAsync(x => x.Id, x => x.Name);

            return View("EditMessage", addMessageViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditMessageByIds([FromQuery]Guid? idChain, [FromQuery]Guid? idChainContent)
        {
            if (!idChain.HasValue)
                return RedirectToAction(nameof(Index));
            if (!idChainContent.HasValue)
                return RedirectToAction(nameof(AddMessage), new { idChain });

            if (!_userHelperService.HasSelectedGroup(User))
                return RedirectToAction(nameof(GroupsController.Index), "Groups");
            var selectedGroup = _userHelperService.GetSelectedGroup(User);

            var addMessageViewModel = await _context.ChainContents
                .Where(x => x.Id == idChainContent.Value)
                .Select(x => new EditMessageViewModel()
                {
                    IdChain = idChain.Value,
                    IdChainContent = idChainContent,
                    IdMessage = x.IdMessage,
                    Hours = x.SendAfter.Hours + x.SendAfter.Days * 24,
                    Minutes = (byte)x.SendAfter.Minutes,
                    IdExcludeFromChain = x.IdExcludeFromChain,
                    IdGoToChain = x.IdGoToChain,
                    IsOnlyDayTime = x.IsOnlyDayTime,
                }).FirstOrDefaultAsync();

            if (addMessageViewModel.IdMessage.HasValue)
            {
                var message = await _context.Messages
                    .Include(x => x.Files)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == addMessageViewModel.IdMessage.Value);

                if (message != null)
                {
                    addMessageViewModel.Message = message.Text;
                    addMessageViewModel.IsImageFirst = message.IsImageFirst;
                    var keyboard = VkConnector.Models.Common.Keyboard.Deserialize(message.Keyboard);
                    if (keyboard != null)
                    {
                        addMessageViewModel.Keyboard = new List<List<ViewModels.Shared.MessageButton>>();
                        for (byte rowIdx = 0; rowIdx < keyboard.Buttons.Length; rowIdx++)
                        {
                            var currentRow = keyboard.Buttons[rowIdx];
                            byte colIdx = 0;
                            addMessageViewModel.Keyboard.Add(currentRow.Select(x => new ViewModels.Shared.MessageButton()
                            {
                                ButtonColor = x.Color,
                                Column = colIdx++,
                                CanDelete = colIdx == currentRow.Length,
                                Row = rowIdx,
                                Text = x.Action.Text
                            }).ToList());
                        }
                    }

                    uint idx = 0;
                    addMessageViewModel.Files = message.Files.Select(x => new ViewModels.Shared.FileModel()
                    {
                        Id = x.IdFile,
                        Name = _context.Files.Where(y => y.Id == x.IdFile).Select(y => y.Name).FirstOrDefault(),
                        Index = idx++
                    }).ToList();
                }
            }

            ViewBag.Chains = await _context.Chains
                .Where(x => x.IdGroup == selectedGroup.Key)
                .ToDictionaryAsync(x => x.Id, x => x.Name);
            return View("EditMessage", addMessageViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMessage(EditMessageViewModel editMessageViewModel)
        {
            var selectedGroup = _userHelperService.GetSelectedGroup(User);

            ViewBag.Chains = await _context.Chains
                .Where(x => x.IdGroup == selectedGroup.Key)
                .ToDictionaryAsync(x => x.Id, x => x.Name);
            return View(nameof(EditMessage), editMessageViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> RemoveMessage(string idChainContent)
        {
            if (string.IsNullOrEmpty(idChainContent) || !Guid.TryParse(idChainContent, out Guid id))
                return RedirectToAction(nameof(Index), "Chains");

            _context.ChainContents.RemoveRange(_context.ChainContents.Where(x => x.Id == id));
            await _context.SaveChangesAsync();

            return Json(new { state = 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveMessage(EditMessageViewModel editMessageViewModel)
        {
            if (!ModelState.IsValid)
                return await EditMessage(editMessageViewModel);

            var groupInfo = _userHelperService.GetSelectedGroup(User);

            Messages message = null;
            if (editMessageViewModel.IdMessage.HasValue && await _context.Messages.AnyAsync(x => x.Id == editMessageViewModel.IdMessage.Value))
            {
                message = await _context.Messages.FirstOrDefaultAsync(x => x.Id == editMessageViewModel.IdMessage.Value);
                message.Text = editMessageViewModel.Message;
                message.Keyboard = editMessageViewModel.GetVkKeyboard()?.Serialize();
                await _context.SaveChangesAsync();

                if (editMessageViewModel.Files != null && editMessageViewModel.Files.Any())
                {
                    IEnumerable<Guid> newIdFiles = editMessageViewModel.Files.Select(x => x.Id);
                    if (message.Files != null && message.Files.Any())
                        newIdFiles = newIdFiles.Except(message.Files.Select(x => x.IdFile));
                    DbHelper.AddFilesInMessage(_context, message.Id, newIdFiles);
                }
            }
            else
                message = await DbHelper.AddMessage(_context, groupInfo.Key, editMessageViewModel.Message, editMessageViewModel.GetVkKeyboard(), editMessageViewModel.IsImageFirst, editMessageViewModel.Files.Select(x => x.Id));

            ChainContents chainContent = null;
            if (editMessageViewModel.IdChainContent.HasValue)
                chainContent = await _context.ChainContents.FirstOrDefaultAsync(x => x.Id == editMessageViewModel.IdChainContent.Value);
            if (chainContent == null)
            {
                chainContent = new ChainContents()
                {
                    IdChain = editMessageViewModel.IdChain,
                    Index = await _context.ChainContents
                        .Where(x => x.IdChain == editMessageViewModel.IdChain)
                        .OrderByDescending(x => x.Index)
                        .Select(x => x.Index)
                        .FirstOrDefaultAsync() + 1
                };
                await _context.ChainContents.AddAsync(chainContent);
            }
            chainContent.SendAfterSeconds = editMessageViewModel.Hours * 60 * 60 + editMessageViewModel.Minutes * 60;
            chainContent.IsOnlyDayTime = editMessageViewModel.IsOnlyDayTime;
            chainContent.IdExcludeFromChain = editMessageViewModel.IdExcludeFromChain;
            chainContent.IdGoToChain = editMessageViewModel.IdGoToChain;

            chainContent.IdMessage = message.Id;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(EditById), new
            {
                idChain = editMessageViewModel.IdChain
            });
        }

        public async Task<IActionResult> EditById(Guid? idChain)
        {
            if (idChain == null)
                return NotFound();

            var model = await _context.Chains
                .Where(x => x.Id == idChain.Value)
                .Select(x => new ChainViewModel()
                {
                    Id = x.Id,
                    IdGroup = x.IdGroup,
                    Name = x.Name
                }).FirstOrDefaultAsync();

            return RedirectToAction(nameof(Edit), model);
        }

        public async Task<IActionResult> Edit(ChainViewModel data)
        {
            if (data == null)
                return NotFound();

            data.Content = await DbHelper.GetChainContents(_context, data.Id)
                .Select(x => new ChainContentIndexViewModel()
                {
                    Id = x.Id,
                    ExcludeFromChainName = x.ExcludeFromChain == null ? null : x.ExcludeFromChain.Name,
                    GoToChainName = x.GoToChain == null ? null : x.GoToChain.Name,
                    IsOnlyDayTime = x.IsOnlyDayTime,
                    MessageText = x.Message == null ? null : x.Message.Text,
                    SendAfterHours = x.SendAfter.Days * 24 + x.SendAfter.Hours,
                    SendAfterMinutes = x.SendAfter.Minutes,
                    SubscribersInChainContent = _context.SubscribersInChains.Count(y => !y.IsSended && y.IdChainStep == x.Id)
                }).ToArrayAsync();

            ViewBag.Chains = await _context.Chains.Where(x => x.IdGroup == data.IdGroup.Value).ToArrayAsync();

            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromQuery]Guid? idChain)
        {
            if (!idChain.HasValue)
                return NotFound(idChain);

            _context.Chains.RemoveRange(_context.Chains.Where(x => x.Id == idChain.Value));

            var removingChainContents = _context.ChainContents.Where(x => x.IdChain == idChain.Value);
            var removingChainContentsIds = removingChainContents.Select(x => x.Id);
            foreach (var removingChainContentsId in removingChainContentsIds)
            {
                _context.SubscribersInChains.RemoveRange(_context.SubscribersInChains.Where(x => x.IdChainStep == removingChainContentsId));
                _context.CheckedSubscribersInRepostScenarios.RemoveRange(_context.CheckedSubscribersInRepostScenarios.Where(x => x.RepostScenario.IdCheckingChainContent == removingChainContentsId));

                Guid? idMessage = await _context.ChainContents.Where(x => x.Id == removingChainContentsId).Select(x => x.IdMessage).FirstOrDefaultAsync();
                if (idMessage.HasValue)
                    DbHelper.RemoveMessage(_context, idMessage.Value);
            }
            _context.ChainContents.RemoveRange(removingChainContents);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
