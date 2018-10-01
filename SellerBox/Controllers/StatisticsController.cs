using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SellerBox.Common;
using SellerBox.Common.Services;
using SellerBox.ViewModels.Statistics;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SellerBox.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly UserHelperService _userHelperService;

        public StatisticsController(DatabaseContext context, UserHelperService userHelperService)
        {
            _context = context;
            _userHelperService = userHelperService;
        }

        public IActionResult Index()
        {
            var model = new IndexViewModel()
            {
                DtStart = DateTime.Now.AddDays(-7).ToString("dd.MM.yyyy HH:mm"),
                DtEnd = DateTime.Now.ToString("dd.MM.yyyy HH:mm")
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Generate(IndexViewModel indexViewModel)
        {
            if (!DateTime.TryParseExact(indexViewModel.DtStart, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dtStart) ||
                !DateTime.TryParseExact(indexViewModel.DtEnd, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dtEnd))
                return RedirectToAction(nameof(Index), indexViewModel);
            dtStart = DateTime.SpecifyKind(dtStart, DateTimeKind.Local).ToUniversalTime();
            dtEnd = DateTime.SpecifyKind(dtEnd, DateTimeKind.Local).ToUniversalTime();

            switch (indexViewModel.StatisticType)
            {
                case StatisticType.Messages:
                    return await GenerateMessagesReport(dtStart, dtEnd);
                case StatisticType.ChatScenarios:
                    return await GenerateChatScenariosReport(dtStart, dtEnd);
            }
            return RedirectToAction(nameof(Index), indexViewModel);
        }

        private async Task<IActionResult> GenerateMessagesReport(DateTime dtStart, DateTime dtEnd)
        {
            var groupInfo = _userHelperService.GetSelectedGroup(User);
            var perDayItems = await _context.History_Messages
                .Where(x => x.Subscriber.IdGroup == groupInfo.Key)
                .Where(x => x.Dt <= dtEnd.Date && x.Dt >= dtStart.Date)
                .GroupBy(x => x.Dt.Date)
                .Select(x => new MessagesViewModel.MessagesPerDayViewModel()
                {
                    Date = x.Key,
                    Sended = x.LongCount(z => z.IsOutgoingMessage),
                    Received = x.LongCount(z => !z.IsOutgoingMessage)
                })
                .ToArrayAsync();

            var model = new MessagesViewModel()
            {
                TotalSended = perDayItems.Sum(x => x.Sended),
                TotalReceived = perDayItems.Sum(x => x.Received),
                MessagesPerDay = perDayItems
            };

            return View("Messages", model);
        }

        private async Task<IActionResult> GenerateChatScenariosReport(DateTime dtStart, DateTime dtEnd)
        {
            var groupInfo = _userHelperService.GetSelectedGroup(User);

            var model = new ChatContentsViewModel();

            var items = await _context.History_SubscribersInChatScenariosContents
                .Where(x => x.Subscriber.IdGroup == groupInfo.Key)
                .Where(x => x.Dt <= dtEnd.Date && x.Dt >= dtStart.Date)
                .Include(x => x.ChatScenarioContent)
                .Include(x => x.ChatScenarioContent.ChatScenario)
                .GroupBy(x => x.Dt.Date)
                .ToArrayAsync();
            model = new ChatContentsViewModel()
            {
                TotalReceived = items.Sum(x => x.LongCount()),
                MessagesPerChatScenarios = items.Select(x => new ChatContentsViewModel.MessagesPerChatScenario()
                {
                    Date = x.Key,
                    MessagesInChatScenarios = x.GroupBy(y => y.ChatScenarioContent.ChatScenario.Name).Select(y => new ChatContentsViewModel.ChatScenarioInfo()
                    {
                        Name = y.Key,
                        MessagesBySteps = y.GroupBy(z => z.ChatScenarioContent.Step.ToString()).Select(z => new ChatContentsViewModel.ChatScenarioContentInfo()
                        {
                            Step = z.Key,
                            MessagesCount = z.LongCount()
                        }).ToArray()
                    }).ToArray()
                }).ToArray()
            };
            /*
                new Tuple<DateTime, Tuple<string, Tuple<string, long>[]>[]>(
                    x.Key,
                    x.GroupBy(y => y.ChatScenarioContent.ChatScenario.Name)
                    .Select(y => new Tuple<string, Tuple<string, long>[]>(
                              y.Key,
                              y.GroupBy(z => z.ChatScenarioContent.Step.ToString()).Select(z => new Tuple<string, long>(
                                    z.Key,
                                    z.LongCount()))
                                .ToArray()))
                    .ToArray()))
                .ToArray();
                */
            return View("ChatScenarios", model);
        }
    }
}