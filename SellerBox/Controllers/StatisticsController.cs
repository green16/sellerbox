using Microsoft.AspNetCore.Mvc;
using SellerBox.Common;
using SellerBox.Common.Helpers;
using SellerBox.Common.Services;
using SellerBox.ViewModels.Statistics;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SellerBox.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class StatisticsController : Controller
    {
        private readonly Tuple<byte, string, StatisticsHelper.ReportGenerator>[] statisticTypes;

        private readonly DatabaseContext _context;
        private readonly UserHelperService _userHelperService;

        public StatisticsController(DatabaseContext context, UserHelperService userHelperService)
        {
            statisticTypes = new Tuple<byte, string, StatisticsHelper.ReportGenerator>[]
            {
                new Tuple<byte, string, StatisticsHelper.ReportGenerator>(0, "Сообщения", StatisticsHelper.GenerateMessagesReport),
                new Tuple<byte, string, StatisticsHelper.ReportGenerator>(1, "Чатбот", StatisticsHelper.GenerateChatScenariosReport),
                new Tuple<byte, string, StatisticsHelper.ReportGenerator>(2, "Сценарии", StatisticsHelper.GenerateScenariosReport),
                new Tuple<byte, string, StatisticsHelper.ReportGenerator>(3, "Цепочки", StatisticsHelper.GenerateChainsReport),
                new Tuple<byte, string, StatisticsHelper.ReportGenerator>(4, "Действия в группе", StatisticsHelper.GenerateGroupActionReport),
                new Tuple<byte, string, StatisticsHelper.ReportGenerator>(5, "Горячие ссылки", StatisticsHelper.GenerateShortUrlsReport)
            };

            _context = context;
            _userHelperService = userHelperService;
        }

        public IActionResult Reports()
        {
            var model = new ReportsViewModel()
            {
                DtStart = DateTime.Now.AddDays(-7).ToString("dd.MM.yyyy HH:mm"),
                DtEnd = DateTime.Now.ToString("dd.MM.yyyy HH:mm")
            };
            ViewBag.StatisticTypes = statisticTypes.ToDictionary(x => x.Item1, x => x.Item2);
            return View(model);
        }

        public async Task<IActionResult> Notifications()
        {
            return View();
        }

        public async Task<IActionResult> CreateNotification()
        {
            return View();
        }

        public async Task<IActionResult> SaveNotification()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ChartInfoViewModel> Generate(ReportsViewModel indexViewModel)
        {
            if (!DateTime.TryParseExact(indexViewModel.DtStart, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dtStart) ||
                !DateTime.TryParseExact(indexViewModel.DtEnd, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dtEnd))
                return null;
            dtStart = DateTime.SpecifyKind(dtStart, DateTimeKind.Local).ToUniversalTime();
            dtEnd = DateTime.SpecifyKind(dtEnd, DateTimeKind.Local).ToUniversalTime().AddMinutes(1);

            var function = statisticTypes.FirstOrDefault(x => x.Item1 == indexViewModel.StatisticType);
            if (function == null)
                return null;

            var groupInfo = _userHelperService.GetSelectedGroup(User);

            return await function.Item3.Invoke(_context, groupInfo.Key, dtStart, dtEnd);
        }
    }
}