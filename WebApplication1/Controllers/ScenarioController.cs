using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Common;
using WebApplication1.Common.Helpers;
using WebApplication1.Common.Services;
using WebApplication1.Models.Database;
using WebApplication1.ViewModels.Scenarios;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class ScenarioController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly UserHelperService _userHelperService;

        public ScenarioController(DatabaseContext context, UserHelperService userHelperService)
        {
            _userHelperService = userHelperService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!_userHelperService.HasSelectedGroup(User))
                return RedirectToAction(nameof(GroupsController.Index), "Groups");

            ScenarioViewModel newScenario = new ScenarioViewModel()
            {
                IsStrictMatch = true
            };

            var selectedGroup = _userHelperService.GetSelectedGroup(User);
            ViewBag.Chains = await _context.Chains
                .Where(x => x.IdGroup == selectedGroup.Key)
                .ToDictionaryAsync(x => x.Id, x => x.Name);
            return View("Edit", newScenario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ScenarioViewModel newScenario)
        {
            if (!_userHelperService.HasSelectedGroup(User))
                return RedirectToAction(nameof(GroupsController.Index), "Groups");

            var selectedGroup = _userHelperService.GetSelectedGroup(User);
            ViewBag.Chains = await _context.Chains
                .Where(x => x.IdGroup == selectedGroup.Key)
                .ToDictionaryAsync(x => x.Id, x => x.Name);
            return View("Edit", newScenario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ScenarioViewModel model)
        {
            if (!_userHelperService.HasSelectedGroup(User))
                return RedirectToAction(nameof(GroupsController.Index), "Groups");

            var selectedGroup = _userHelperService.GetSelectedGroup(User);
            ViewBag.Chains = await _context.Chains
                .Where(x => x.IdGroup == selectedGroup.Key)
                .ToDictionaryAsync(x => x.Id, x => x.Name);
            return View("Edit", model);
        }

        public async Task<IActionResult> Edit(Guid? idScenario)
        {
            if (idScenario == null)
                return NotFound();

            Scenarios scenario = DbHelper.GetScenario(_context, idScenario.Value);

            ScenarioViewModel model = new ScenarioViewModel()
            {
                IdScenario = idScenario,
                Name = scenario.Name,
                Action = scenario.Action,
                IdChain = scenario.IdChain,
                IdChain2 = scenario.IdChain2,
                IdErrorMessage = scenario.IdErrorMessage,
                IdMessage = scenario.IdMessage,
                IsStrictMatch = scenario.IsStrictMatch,
                InputMessage = scenario.InputMessage
            };

            if (scenario.IdErrorMessage.HasValue)
            {
                var message = await _context.Messages
                    .Include(x => x.Files)
                    .FirstOrDefaultAsync(x => x.Id == scenario.IdErrorMessage.Value);

                if (message != null)
                {
                    uint idx = 0;

                    model.ErrorMessage.IsImageFirst = message.IsImageFirst;
                    model.ErrorMessage.Message = message.Text;

                    var keyboard = string.IsNullOrWhiteSpace(message.Keyboard) ? null : Newtonsoft.Json.JsonConvert.DeserializeObject<VkNet.Model.Keyboard.MessageKeyboard>(message.Keyboard);
                    if (keyboard != null)
                    {
                        model.ErrorMessage.Keyboard = new List<List<ViewModels.Shared.MessageButton>>();
                        byte currentRowIdx = 0;
                        foreach (var currentRow in keyboard.Buttons)
                        {
                            byte colIdx = 0;
                            model.ErrorMessage.Keyboard.Add(currentRow.Select(x => new ViewModels.Shared.MessageButton()
                            {
                                ButtonColor = x.Color.ToString(),
                                Column = colIdx++,
                                CanDelete = colIdx == currentRow.Count(),
                                Row = currentRowIdx,
                                Text = x.Action.Label
                            }).ToList());
                            currentRowIdx++;
                        }
                    }

                    model.ErrorMessage.Files = message.Files.Select(x => new ViewModels.Shared.FileModel()
                    {
                        Id = x.IdFile,
                        Name = _context.Files.FirstOrDefault(y => y.Id == x.IdFile)?.Name,
                        Index = idx++,
                        PropertiesPrefix = nameof(ScenarioViewModel.ErrorMessage)
                    }).ToList();
                }
            }
            if (scenario.IdMessage.HasValue)
            {
                var message = await _context.Messages
                    .Include(x => x.Files)
                    .FirstOrDefaultAsync(x => x.Id == scenario.IdMessage.Value);

                if (message != null)
                {
                    uint idx = 0;
                    model.Message.IsImageFirst = message.IsImageFirst;
                    model.Message.Message = message.Text;

                    var keyboard = string.IsNullOrWhiteSpace(message.Keyboard) ? null : Newtonsoft.Json.JsonConvert.DeserializeObject<VkNet.Model.Keyboard.MessageKeyboard>(message.Keyboard);
                    if (keyboard != null)
                    {
                        model.Message.Keyboard = new List<List<ViewModels.Shared.MessageButton>>();
                        byte currentRowIdx = 0;
                        foreach (var currentRow in keyboard.Buttons)
                        {
                            byte colIdx = 0;
                            model.Message.Keyboard.Add(currentRow.Select(x => new ViewModels.Shared.MessageButton()
                            {
                                ButtonColor = x.Color.ToString(),
                                Column = colIdx++,
                                CanDelete = colIdx == currentRow.Count(),
                                Row = currentRowIdx,
                                Text = x.Action.Label
                            }).ToList());
                            currentRowIdx++;
                        }
                    }

                    model.Message.Files = message.Files.Select(x => new ViewModels.Shared.FileModel()
                    {
                        Id = x.IdFile,
                        Name = _context.Files.Where(y => y.Id == x.IdFile).Select(y => y.Name).FirstOrDefault(),
                        Index = idx++,
                        PropertiesPrefix = nameof(ScenarioViewModel.Message)
                    }).ToList();
                }
            }

            var selectedGroup = _userHelperService.GetSelectedGroup(User);
            ViewBag.Chains = await _context.Chains
                .Where(x => x.IdGroup == selectedGroup.Key)
                .ToDictionaryAsync(x => x.Id, x => x.Name);

            return View("Edit", model);
        }

        public async Task<IActionResult> Index()
        {
            if (!_userHelperService.HasSelectedGroup(User))
                return RedirectToAction(nameof(GroupsController.Index), "Groups");

            KeyValuePair<long, string> selectedGroup = _userHelperService.GetSelectedGroup(User);
            var scenarios = await _context.Scenarios
                .Include(x => x.Chain)
                .Where(x => x.IdGroup == selectedGroup.Key)
                .Select(x => new IndexViewModel()
                {
                    Id = x.Id,
                    Action = x.Action,
                    ChainName = x.Chain == null ? "" : x.Chain.Name,
                    InputMessage = x.InputMessage,
                    IsEnabled = x.IsEnabled,
                    IsStrictMatch = x.IsStrictMatch,
                    Name = x.Name
                }).ToArrayAsync();

            return View(scenarios);
        }

        [HttpPost]
        public async Task<IActionResult> ToogleIsStrictMatch([FromQuery]Guid? idScenario)
        {
            if (!idScenario.HasValue)
                return Json(new { error = 1 });

            var scenario = await _context.Scenarios.FirstOrDefaultAsync(x => x.Id == idScenario.Value);
            scenario.IsStrictMatch = !scenario.IsStrictMatch;
            await _context.SaveChangesAsync();

            return Json(new { });
        }

        [HttpPost]
        public async Task<IActionResult> ToogleIsEnabled([FromQuery]Guid? idScenario)
        {
            if (!idScenario.HasValue)
                return Json(new { error = 1 });

            var scenario = await _context.Scenarios.FirstOrDefaultAsync(x => x.Id == idScenario.Value);
            scenario.IsEnabled = !scenario.IsEnabled;
            await _context.SaveChangesAsync();

            return Json(new { error = 0, scenario.IsEnabled });
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromQuery]Guid? idScenario)
        {
            if (!idScenario.HasValue)
                return NotFound(idScenario);
            var removingScenario = DbHelper.GetScenario(_context, idScenario.Value);
            if (removingScenario == null)
                return NotFound(idScenario);

            if (removingScenario.IdErrorMessage.HasValue)
                DbHelper.RemoveMessage(_context, removingScenario.IdErrorMessage.Value);

            if (removingScenario.IdMessage.HasValue)
                DbHelper.RemoveMessage(_context, removingScenario.IdMessage.Value);

            _context.Scenarios.Remove(removingScenario);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(ScenarioViewModel model)
        {
            if (!ModelState.IsValid)
                return await Edit(model);

            var groupInfo = _userHelperService.GetSelectedGroup(User);

            Scenarios scenario = null;
            if (model.IdScenario.HasValue)
                scenario = DbHelper.GetScenario(_context, model.IdScenario.Value);
            if (scenario == null)
            {
                scenario = new Scenarios
                {
                    IsEnabled = true,
                    IdGroup = groupInfo.Key
                };
                await _context.Scenarios.AddAsync(scenario);
            }
            scenario.Action = model.Action;

            switch (model.Action)
            {
                case Models.Database.Common.ScenarioActions.Message:
                    {
                        scenario.IdChain = null;
                        scenario.IdChain2 = null;
                        break;
                    }
                case Models.Database.Common.ScenarioActions.AddToChain:
                    {
                        scenario.IdChain = model.IdChain;
                        scenario.IdChain2 = null;
                        break;
                    }
                case Models.Database.Common.ScenarioActions.RemoveFromChain:
                    {
                        scenario.IdChain = null;
                        scenario.IdChain2 = model.IdChain2;
                        break;
                    }
                case Models.Database.Common.ScenarioActions.ChangeChain:
                    {
                        scenario.IdChain = model.IdChain;
                        scenario.IdChain2 = model.IdChain2;
                        break;
                    }
            }

            scenario.InputMessage = model.InputMessage;
            scenario.Name = model.Name;
            scenario.IsStrictMatch = model.IsStrictMatch;

            if (model.Message?.HasMessage ?? false)
            {
                if (model.IdMessage.HasValue)
                {
                    scenario.Message.Text = model.Message.Message;

                    var keyboard = model.Message.GetVkKeyboard();
                    scenario.Message.Keyboard = keyboard == null ? null : Newtonsoft.Json.JsonConvert.SerializeObject(keyboard);
                    
                    var message = await _context.Messages
                        .Include(x => x.Files)
                        .FirstOrDefaultAsync(x => x.Id == model.IdMessage.Value);

                    if (model.Message.Files != null && model.Message.Files.Any())
                    {
                        IEnumerable<Guid> newIdFiles = model.Message.Files.Select(x => x.Id);
                        if (message.Files != null && message.Files.Any())
                            newIdFiles = newIdFiles.Except(message.Files.Select(x => x.IdFile));
                        DbHelper.AddFilesInMessage(_context, model.IdMessage.Value, newIdFiles);
                    }
                }
                else
                {
                    var message = await DbHelper.AddMessage(_context, groupInfo.Key, model.Message.Message, model.Message.GetVkKeyboard(), model.Message.IsImageFirst, model.Message.Files.Select(x => x.Id));
                    scenario.IdMessage = message.Id;
                }

            }
            else if (scenario.Message != null)
            {
                scenario.IdMessage = null;
                DbHelper.RemoveMessage(_context, scenario.IdMessage.Value);
            }

            if (model.ErrorMessage?.HasMessage ?? false)
            {
                if (model.Action == Models.Database.Common.ScenarioActions.Message)
                {
                    DbHelper.RemoveMessage(_context, scenario.IdErrorMessage.Value);
                    scenario.IdErrorMessage = null;
                }
                else if (model.IdErrorMessage.HasValue)
                {
                    scenario.ErrorMessage.Text = model.ErrorMessage.Message;

                    var keyboard = model.ErrorMessage.GetVkKeyboard();
                    scenario.ErrorMessage.Keyboard = keyboard == null ? null : Newtonsoft.Json.JsonConvert.SerializeObject(keyboard);

                    var message = await _context.Messages
                        .Include(x => x.Files)
                        .FirstOrDefaultAsync(x => x.Id == model.IdErrorMessage.Value);

                    if (model.Message.Files != null && model.Message.Files.Any())
                    {
                        IEnumerable<Guid> newIdFiles = model.Message.Files.Select(x => x.Id);
                        if (message.Files != null && message.Files.Any())
                            newIdFiles = newIdFiles.Except(message.Files.Select(x => x.IdFile));
                        DbHelper.AddFilesInMessage(_context, message.Id, newIdFiles);
                    }
                }
                else
                {
                    var message = await DbHelper.AddMessage(_context, groupInfo.Key, model.ErrorMessage.Message, model.ErrorMessage.GetVkKeyboard(), model.ErrorMessage.IsImageFirst, model.ErrorMessage.Files.Select(x => x.Id));
                    scenario.IdErrorMessage = message.Id;
                }
            }
            else if (scenario.ErrorMessage != null)
            {
                scenario.IdErrorMessage = null;
                DbHelper.RemoveMessage(_context, scenario.IdErrorMessage.Value);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}