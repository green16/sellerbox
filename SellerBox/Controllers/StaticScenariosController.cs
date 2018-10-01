using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SellerBox.Common;
using SellerBox.Common.Helpers;
using SellerBox.Common.Services;
using SellerBox.Models.Database;
using SellerBox.ViewModels.Shared;
using SellerBox.ViewModels.StaticScenarios;

namespace SellerBox.Controllers
{
    [Authorize]
    public class StaticScenariosController : Controller
    {
        private readonly UserHelperService _userHelperService;
        private readonly DatabaseContext _context;

        public StaticScenariosController(DatabaseContext context, UserHelperService userHelperService)
        {
            _context = context;
            _userHelperService = userHelperService;
        }

        public async Task<IActionResult> Index()
        {
            var groupInfo = _userHelperService.GetSelectedGroup(User);

            bool? isBirthdayEnabled = (await _context.BirthdayScenarios.FirstOrDefaultAsync(x => x.IdGroup == groupInfo.Key))?.IsEnabled;
            ViewBag.HasBirthday = isBirthdayEnabled != null;
            ViewBag.IsBirthdayEnabled = isBirthdayEnabled ?? false;

            bool? isBirthdayWallEnabled = (await _context.BirthdayWallScenarios.FirstOrDefaultAsync(x => x.IdGroup == groupInfo.Key))?.IsEnabled;
            ViewBag.HasBirthdayWall = isBirthdayWallEnabled != null;
            ViewBag.IsBirthdayWallEnabled = isBirthdayWallEnabled ?? false;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ToogleBirthdayIsEnabled()
        {
            var groupInfo = _userHelperService.GetSelectedGroup(User);

            BirthdayScenarios birthdayScenario = await _context.BirthdayScenarios.FirstOrDefaultAsync(x => x.IdGroup == groupInfo.Key);
            if (birthdayScenario == null)
                return null;
            birthdayScenario.IsEnabled = !birthdayScenario.IsEnabled;
            await _context.SaveChangesAsync();

            return Json(new { birthdayScenario.IsEnabled });
        }

        [HttpPost]
        public async Task<IActionResult> ToogleBirthdayWallIsEnabled()
        {
            var groupInfo = _userHelperService.GetSelectedGroup(User);

            BirthdayWallScenarios birthdayWallScenario = await _context.BirthdayWallScenarios.FirstOrDefaultAsync(x => x.IdGroup == groupInfo.Key);
            if (birthdayWallScenario == null)
                return null;
            birthdayWallScenario.IsEnabled = !birthdayWallScenario.IsEnabled;
            await _context.SaveChangesAsync();

            return Json(new { birthdayWallScenario.IsEnabled });
        }

        [HttpGet]
        public async Task<IActionResult> Birthday()
        {
            var groupInfo = _userHelperService.GetSelectedGroup(User);

            BirthdayScenarios scenario = await _context.BirthdayScenarios.FirstOrDefaultAsync(x => x.IdGroup == groupInfo.Key);

            BirthdaySchedulerViewModel model = null;
            if (scenario != null)
            {
                Messages message = await _context.Messages
                    .Include(x => x.Files)
                    .FirstOrDefaultAsync(x => x.Id == scenario.IdMessage);
                uint idx = 0;
                model = new BirthdaySchedulerViewModel()
                {
                    IdMessage = scenario.IdMessage,
                    DaysBefore = scenario.DaysBefore,
                    SendAt = scenario.SendAt,
                    Message = message.Text,
                    IsImageFirst = message.IsImageFirst,
                    Files = message.Files.Select(x => new FileModel()
                    {
                        Id = x.IdFile,
                        Name = _context.Files.FirstOrDefault(y => y.Id == x.IdFile)?.Name,
                        Index = idx++
                    }).ToList()
                };

                var keyboard = string.IsNullOrWhiteSpace(message.Keyboard) ? null : Newtonsoft.Json.JsonConvert.DeserializeObject<VkNet.Model.Keyboard.MessageKeyboard>(message.Keyboard);
                if (keyboard != null)
                {
                    model.Keyboard = new List<List<MessageButton>>();
                    byte currentRowIdx = 0;
                    foreach (var currentRow in keyboard.Buttons)
                    {
                        byte colIdx = 0;
                        model.Keyboard.Add(currentRow.Select(x => new MessageButton()
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
            }
            else
                model = new BirthdaySchedulerViewModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Birthday(BirthdaySchedulerViewModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveBirthday(BirthdaySchedulerViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Birthday", model);

            var groupInfo = _userHelperService.GetSelectedGroup(User);

            Messages message = null;
            if (model.IdMessage.HasValue)
            {
                message = await _context.Messages
                    .Include(x => x.Files)
                    .FirstOrDefaultAsync(x => x.Id == model.IdMessage.Value);

                message.Text = model.Message;

                var keyboard = model.GetVkKeyboard();
                message.Keyboard = keyboard == null ? null : Newtonsoft.Json.JsonConvert.SerializeObject(keyboard);

                var newFilesInMessage = model.Files.Select(x => x.Id)
                    .Except(message.Files.Select(x => x.IdFile))
                    .Select(x => new FilesInMessage()
                    {
                        IdFile = x,
                        IdMessage = message.Id
                    });

                await _context.FilesInMessage.AddRangeAsync(newFilesInMessage);
            }
            else
                message = await DbHelper.AddMessage(_context, groupInfo.Key, model.Message, model.GetVkKeyboard(), model.IsImageFirst, model.Files.Select(x => x.Id));


            BirthdayScenarios scenario = _context.BirthdayScenarios.Include(x => x.Message).FirstOrDefault(x => x.IdGroup == groupInfo.Key);

            if (scenario == null)
            {
                scenario = new BirthdayScenarios()
                {
                    IdGroup = groupInfo.Key,
                    IsEnabled = true
                };
                await _context.BirthdayScenarios.AddAsync(scenario);
            }
            if (!model.IdMessage.HasValue)
                scenario.IdMessage = message.Id;
            scenario.DaysBefore = model.DaysBefore;
            scenario.SendAt = model.SendAt;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> BirthdayWall()
        {
            var groupInfo = _userHelperService.GetSelectedGroup(User);

            BirthdayWallScenarios scenario = await _context.BirthdayWallScenarios.FirstOrDefaultAsync(x => x.IdGroup == groupInfo.Key);

            BirthdayWallSchedulerViewModel model = null;
            if (scenario != null)
            {
                Messages message = await _context.Messages
                    .Include(x => x.Files)
                    .FirstOrDefaultAsync(x => x.Id == scenario.IdMessage);

                uint idx = 0;
                model = new BirthdayWallSchedulerViewModel()
                {
                    IdMessage = scenario.IdMessage,
                    SendAt = scenario.SendAt,

                    Message = message.Text,
                    Files = message.Files.Select(y => new FileModel()
                    {
                        Id = y.IdFile,
                        Name = _context.Files.FirstOrDefault(z => y.IdFile == z.Id)?.Name,
                        Index = idx++
                    }).ToList()
                };
            }
            else
                model = new BirthdayWallSchedulerViewModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BirthdayWall(BirthdayWallSchedulerViewModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveBirthdayWall(BirthdayWallSchedulerViewModel model)
        {
            if (!ModelState.IsValid)
                return View("BirthdayWall", model);

            var groupInfo = _userHelperService.GetSelectedGroup(User);
            var idUser = await _userHelperService.GetUserIdVk(User);

            Messages message = null;
            if (model.IdMessage.HasValue)
            {
                message = await _context.Messages
                    .Include(x => x.Files)
                    .FirstOrDefaultAsync(x => x.Id == model.IdMessage.Value);

                message.Text = model.Message;
                var newFilesInMessage = model.Files.Select(x => x.Id)
                    .Except(message.Files.Select(x => x.IdFile))
                    .Select(x => new FilesInMessage()
                    {
                        IdFile = x,
                        IdMessage = message.Id
                    });

                await _context.FilesInMessage.AddRangeAsync(newFilesInMessage);
            }
            else
                message = await DbHelper.AddMessage(_context, groupInfo.Key, model.Message, null, false, model.Files.Select(x => x.Id));

            BirthdayWallScenarios scenario = _context.BirthdayWallScenarios.Include(x => x.Message).FirstOrDefault(x => x.IdGroup == groupInfo.Key);

            if (scenario == null)
            {
                scenario = new BirthdayWallScenarios()
                {
                    IdGroup = groupInfo.Key,
                    IsEnabled = true
                };
                await _context.BirthdayWallScenarios.AddAsync(scenario);
            }
            if (!model.IdMessage.HasValue)
                scenario.IdMessage = message.Id;

            scenario.SendAt = model.SendAt;
            scenario.IdVkUser = idUser;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}