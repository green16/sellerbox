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
        private readonly Tuple<byte, string, Func<DateTime, DateTime, Task<IActionResult>>>[] statisticTypes;

        private readonly DatabaseContext _context;
        private readonly UserHelperService _userHelperService;

        public StatisticsController(DatabaseContext context, UserHelperService userHelperService)
        {
            statisticTypes = new Tuple<byte, string, Func<DateTime, DateTime, Task<IActionResult>>>[]
            {
                new Tuple<byte, string, Func<DateTime, DateTime, Task<IActionResult>>>(0, "Сообщения", GenerateMessagesReport),
                new Tuple<byte, string, Func<DateTime, DateTime, Task<IActionResult>>>(1, "Чатбот", GenerateChatScenariosReport),
                new Tuple<byte, string, Func<DateTime, DateTime, Task<IActionResult>>>(2, "Сценарии", GenerateScenariosReport),
                new Tuple<byte, string, Func<DateTime, DateTime, Task<IActionResult>>>(3, "Цепочки", GenerateChainsReport),
                new Tuple<byte, string, Func<DateTime, DateTime, Task<IActionResult>>>(4, "Действия в группе", GenerateGroupActionReport)
            };

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
            ViewBag.StatisticTypes = statisticTypes.ToDictionary(x => x.Item1, x => x.Item2);
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

            var function = statisticTypes.FirstOrDefault(x => x.Item1 == indexViewModel.StatisticType);
            if (function == null)
                return RedirectToAction(nameof(Index), indexViewModel);

            return await function.Item3.Invoke(dtStart, dtEnd);
        }

        private async Task<IActionResult> GenerateScenariosReport(DateTime dtStart, DateTime dtEnd)
        {
            var groupInfo = _userHelperService.GetSelectedGroup(User);
            var perDayItems = await _context.History_Scenarios
                .Where(x => x.Subscriber.IdGroup == groupInfo.Key)
                .Where(x => x.Dt <= dtEnd && x.Dt >= dtStart)
                .GroupBy(x => x.Dt.Date)
                .Select(x => new ScenariosViewModel.MessagesPerDayViewModel()
                {
                    Date = x.Key,
                    Count = x.LongCount()
                })
                .ToArrayAsync();

            var model = new ScenariosViewModel()
            {
                TotalReceived = perDayItems.Sum(x => x.Count),
                MessagesPerDay = perDayItems
            };

            return View("Scenarios", model);
        }

        private async Task<IActionResult> GenerateMessagesReport(DateTime dtStart, DateTime dtEnd)
        {
            var groupInfo = _userHelperService.GetSelectedGroup(User);
            var perDayItems = await _context.History_Messages
                .Where(x => x.Subscriber.IdGroup == groupInfo.Key)
                .Where(x => x.Dt <= dtEnd && x.Dt >= dtStart)
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

            var items = await _context.History_SubscribersInChatScenariosContents
                .Where(x => x.Subscriber.IdGroup == groupInfo.Key)
                .Where(x => x.Dt <= dtEnd && x.Dt >= dtStart)
                .GroupBy(x => x.Dt.Date)
                .Select(x => new ChatContentsViewModel.MessagesPerDayViewModel()
                {
                    Date = x.Key,
                    Count = x.LongCount()
                })
                .ToArrayAsync();
            var model = new ChatContentsViewModel()
            {
                TotalReceived = items.Sum(x => x.Count),
                MessagesPerDay = items
            };
            return View("ChatScenarios", model);
        }

        private async Task<IActionResult> GenerateChainsReport(DateTime dtStart, DateTime dtEnd)
        {
            var groupInfo = _userHelperService.GetSelectedGroup(User);
            var perDayItems = await _context.History_SubscribersInChainSteps
                .Where(x => x.Subscriber.IdGroup == groupInfo.Key)
                .Where(x => x.DtAdd <= dtEnd && x.DtAdd >= dtStart)
                .GroupBy(x => x.DtAdd.Date)
                .Select(x => new ChainsViewModel.ChainInfoViewModel()
                {
                    Date = x.Key,
                    MessagesPerDay = x.GroupBy(z => z.ChainStep.Chain.Name).Select(z => new ChainsViewModel.MessagesPerDayViewModel()
                    {
                        Name = z.Key,
                        Count = z.LongCount()
                    }).ToArray()
                })
                .ToArrayAsync();

            var model = new ChainsViewModel()
            {
                TotalReceived = perDayItems.Sum(x => x.MessagesPerDay.Sum(z => z.Count)),
                ChainInfo = perDayItems
            };

            return View("Chains", model);
        }

        private async Task<IActionResult> GenerateGroupActionReport(DateTime dtStart, DateTime dtEnd)
        {
            var groupInfo = _userHelperService.GetSelectedGroup(User);
            var perDayItems = await _context.History_GroupActions
                .Where(x => x.Subscriber.IdGroup == groupInfo.Key)
                .Where(x => x.Dt <= dtEnd && x.Dt >= dtStart)
                .GroupBy(x => x.Dt.Date)
                .Select(x => new GroupActionsViewModel.ActionsPerDayViewModel()
                {
                    Date = x.Key,
                    AcceptMessagingCount = x.LongCount(y => y.ActionType == Models.Database.Common.GroupActionTypes.AcceptMessaging),
                    BlockedCount = x.LongCount(y => y.ActionType == Models.Database.Common.GroupActionTypes.Blocked),
                    BlockMessagingCount = x.LongCount(y => y.ActionType == Models.Database.Common.GroupActionTypes.BlockMessaging),
                    CancelMessagingCount = x.LongCount(y => y.ActionType == Models.Database.Common.GroupActionTypes.CancelMessaging),
                    JoinGroupCount = x.LongCount(y => y.ActionType == Models.Database.Common.GroupActionTypes.JoinGroup),
                    LeaveGroupCount = x.LongCount(y => y.ActionType == Models.Database.Common.GroupActionTypes.LeaveGroup),
                    UnblockedCount = x.LongCount(y => y.ActionType == Models.Database.Common.GroupActionTypes.Unblocked)
                })
                .ToArrayAsync();

            var model = new GroupActionsViewModel()
            {
                ActionsPerDay = perDayItems,
                TotalAcceptMessagingCount = perDayItems.Sum(x => x.AcceptMessagingCount),
                TotalBlockedCount = perDayItems.Sum(x => x.BlockedCount),
                TotalBlockMessagingCount = perDayItems.Sum(x => x.BlockMessagingCount),
                TotalCancelMessagingCount = perDayItems.Sum(x => x.CancelMessagingCount),
                TotalJoinGroupCount = perDayItems.Sum(x => x.JoinGroupCount),
                TotalLeaveGroupCount = perDayItems.Sum(x => x.LeaveGroupCount),
                TotalUnblockedCount = perDayItems.Sum(x => x.UnblockedCount)
            };

            return View("GroupActions", model);
        }
    }
}