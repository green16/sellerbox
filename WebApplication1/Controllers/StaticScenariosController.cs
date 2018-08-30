using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Common;
using WebApplication1.Common.Helpers;
using WebApplication1.Common.Services;
using WebApplication1.Models.Database;
using WebApplication1.ViewModels.StaticScenarios;

namespace WebApplication1.Controllers
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

            ViewBag.IsBirthdayEnabled = (await _context.BirthdayScenarios.FirstOrDefaultAsync(x => x.IdGroup == groupInfo.Key))?.IsEnabled ?? false;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ToogleBirthdayIsEnabled()
        {
            var groupInfo = _userHelperService.GetSelectedGroup(User);

            BirthdayScenarios birthdayScenario = await _context.BirthdayScenarios.FirstOrDefaultAsync(x => x.IdGroup == groupInfo.Key);
            birthdayScenario.IsEnabled = !birthdayScenario.IsEnabled;
            await _context.SaveChangesAsync();

            return Json(new { birthdayScenario.IsEnabled });
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
                    Files = message.Files.Select(x => new ViewModels.Shared.FileModel()
                    {
                        Id = x.IdFile,
                        Name = _context.Files.FirstOrDefault(y => y.Id == x.IdFile)?.Name,
                        Index = idx++
                    }).ToList()
                };
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
                message.Keyboard = model.GetVkKeyboard()?.Serialize();
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
    }
}