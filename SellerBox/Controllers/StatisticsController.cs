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
        private readonly Tuple<byte, string, Func<DateTime, DateTime, Task<ChartInfoViewModel>>>[] statisticTypes;

        private readonly DatabaseContext _context;
        private readonly UserHelperService _userHelperService;

        public StatisticsController(DatabaseContext context, UserHelperService userHelperService)
        {
            statisticTypes = new Tuple<byte, string, Func<DateTime, DateTime, Task<ChartInfoViewModel>>>[]
            {
                new Tuple<byte, string, Func<DateTime, DateTime, Task<ChartInfoViewModel>>>(0, "Сообщения", GenerateMessagesReport),
                new Tuple<byte, string, Func<DateTime, DateTime, Task<ChartInfoViewModel>>>(1, "Чатбот", GenerateChatScenariosReport),
                new Tuple<byte, string, Func<DateTime, DateTime, Task<ChartInfoViewModel>>>(2, "Сценарии", GenerateScenariosReport),
                new Tuple<byte, string, Func<DateTime, DateTime, Task<ChartInfoViewModel>>>(3, "Цепочки", GenerateChainsReport),
                new Tuple<byte, string, Func<DateTime, DateTime, Task<ChartInfoViewModel>>>(4, "Действия в группе", GenerateGroupActionReport)
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
        public async Task<ChartInfoViewModel> Generate(IndexViewModel indexViewModel)
        {
            if (!DateTime.TryParseExact(indexViewModel.DtStart, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dtStart) ||
                !DateTime.TryParseExact(indexViewModel.DtEnd, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dtEnd))
                return null;
            dtStart = DateTime.SpecifyKind(dtStart, DateTimeKind.Local).ToUniversalTime();
            dtEnd = DateTime.SpecifyKind(dtEnd, DateTimeKind.Local).ToUniversalTime();

            var function = statisticTypes.FirstOrDefault(x => x.Item1 == indexViewModel.StatisticType);
            if (function == null)
                return null;

            return await function.Item3.Invoke(dtStart, dtEnd);
        }

        private async Task<ChartInfoViewModel> GenerateScenariosReport(DateTime dtStart, DateTime dtEnd)
        {
            var groupInfo = _userHelperService.GetSelectedGroup(User);
            var perDayItems = await _context.History_Scenarios
                .Where(x => x.Subscriber.IdGroup == groupInfo.Key)
                .Where(x => x.Dt <= dtEnd && x.Dt >= dtStart)
                .GroupBy(x => x.Dt.Date)
                .Select(x => new ScenariosMessagesPerDayViewModel()
                {
                    Date = x.Key,
                    Count = x.LongCount()
                })
                .ToArrayAsync();

            var model = new ChartInfoViewModel()
            {
                Title = "Статистика по сценариям",
                YLabels = perDayItems.Select(x => x.Date.ToString("dd MMMM yyyyг.")).ToArray(),
                Dataset = new ChartInfoViewModel.ChartData[]
                {
                    new ChartInfoViewModel.ChartData()
                    {
                        BackgroundColor = "rgba(0, 0, 255, 1)",
                        Data = perDayItems.Select(x => x.Count).ToArray(),
                        Label = "Получено сообщений"
                    }
                },
                Legend = new ChartInfoViewModel.ChartLegend[]
                {
                    new ChartInfoViewModel.ChartLegend()
                    {
                        Color = "rgba(0, 0, 255, 1)",
                        Text = "Получено сообщений",
                        Value = perDayItems.Sum(x => x.Count)
                    }
                }
            };

            return model;
        }

        private async Task<ChartInfoViewModel> GenerateMessagesReport(DateTime dtStart, DateTime dtEnd)
        {
            var groupInfo = _userHelperService.GetSelectedGroup(User);
            var perDayItems = await _context.History_Messages
                .Where(x => x.Subscriber.IdGroup == groupInfo.Key)
                .Where(x => x.Dt <= dtEnd && x.Dt >= dtStart)
                .GroupBy(x => x.Dt.Date)
                .Select(x => new MessagesPerDayViewModel()
                {
                    Date = x.Key,
                    Sended = x.LongCount(z => z.IsOutgoingMessage),
                    Received = x.LongCount(z => !z.IsOutgoingMessage)
                })
                .ToArrayAsync();

            var model = new ChartInfoViewModel()
            {
                Title = "Статистика по сообщениям",
                YLabels = perDayItems.Select(x => x.Date.ToString("dd MMMM yyyyг.")).ToArray(),
                Dataset = new ChartInfoViewModel.ChartData[]
                {
                    new ChartInfoViewModel.ChartData()
                    {
                        BackgroundColor = "rgba(255, 0, 0, 1)",
                        Data = perDayItems.Select(x => x.Sended).ToArray(),
                        Label = "Отправлено"
                    },
                    new ChartInfoViewModel.ChartData()
                    {
                        BackgroundColor = "rgba(0, 0, 255, 1)",
                        Data = perDayItems.Select(x => x.Received).ToArray(),
                        Label = "Получено"
                    }
                },
                Legend = new ChartInfoViewModel.ChartLegend[]
                {
                    new ChartInfoViewModel.ChartLegend()
                    {
                        Color = "rgba(255, 0, 0, 1)",
                        Text = "Получено сообщений",
                        Value = perDayItems.Sum(x => x.Received)
                    },
                    new ChartInfoViewModel.ChartLegend()
                    {
                        Color = "rgba(0, 0, 255, 1)",
                        Text = "Отправлено сообщений",
                        Value = perDayItems.Sum(x => x.Sended)
                    }
                }
            };

            return model;
        }

        private async Task<ChartInfoViewModel> GenerateChatScenariosReport(DateTime dtStart, DateTime dtEnd)
        {
            var groupInfo = _userHelperService.GetSelectedGroup(User);

            var perDayItems = await _context.History_SubscribersInChatScenariosContents
                .Where(x => x.Subscriber.IdGroup == groupInfo.Key)
                .Where(x => x.Dt <= dtEnd && x.Dt >= dtStart)
                .GroupBy(x => x.Dt.Date)
                .Select(x => new ChatContentsMessagesPerDayViewModel()
                {
                    Date = x.Key,
                    Count = x.LongCount()
                })
                .ToArrayAsync();
            var model = new ChartInfoViewModel()
            {
                Title = "Статистика по чатботу",
                YLabels = perDayItems.Select(x => x.Date.ToString("dd MMMM yyyyг.")).ToArray(),
                Dataset = new ChartInfoViewModel.ChartData[]
                {
                    new ChartInfoViewModel.ChartData()
                    {
                        BackgroundColor = "rgba(0, 0, 255, 1)",
                        Data = perDayItems.Select(x => x.Count).ToArray(),
                        Label = "Получено сообщений"
                    }
                },
                Legend = new ChartInfoViewModel.ChartLegend[]
                {
                    new ChartInfoViewModel.ChartLegend()
                    {
                        Color = "rgba(0, 0, 255, 1)",
                        Text = "Получено сообщений",
                        Value = perDayItems.Sum(x => x.Count)
                    }
                }
            };
            return model;
        }

        private async Task<ChartInfoViewModel> GenerateChainsReport(DateTime dtStart, DateTime dtEnd)
        {
            Random random = new Random();

            var groupInfo = _userHelperService.GetSelectedGroup(User);
            var perDayItems = await _context.History_SubscribersInChainSteps
                .Where(x => x.Subscriber.IdGroup == groupInfo.Key)
                .Where(x => x.DtAdd <= dtEnd && x.DtAdd >= dtStart)
                .GroupBy(x => x.DtAdd.Date)
                .Select(x => new ChainInfoViewModel()
                {
                    Date = x.Key,
                    MessagesPerDay = x.GroupBy(z => z.ChainStep.Chain.Name).Select(z => new ChainMessagesPerDayViewModel()
                    {
                        Name = z.Key,
                        Count = z.LongCount()
                    }).ToArray()
                })
                .ToArrayAsync();

            var datasetsСount = perDayItems.Any() ? perDayItems.Max(x => x.MessagesPerDay.Length) : 0;

            var model = new ChartInfoViewModel()
            {
                Title = "Статистика по цепочкам",
                YLabels = perDayItems.Select(x => x.Date.ToString("dd MMMM yyyyг.")).ToArray(),
                Dataset = new ChartInfoViewModel.ChartData[datasetsСount],
                Legend = new ChartInfoViewModel.ChartLegend[]
                {
                    new ChartInfoViewModel.ChartLegend()
                    {
                        Color = "rgba(255, 255, 255, 0)",
                        Text = "Всего",
                        Value = perDayItems.Sum(x => x.MessagesPerDay.Sum(z => z.Count))
                    }
                }
            };

            foreach (var item in perDayItems)
            {
                for (int idx = 0; idx < item.MessagesPerDay.Length; idx++)
                {
                    model.Dataset[idx] = new ChartInfoViewModel.ChartData()
                    {
                        Label = item.MessagesPerDay[idx].Name,
                        Stack = item.MessagesPerDay[idx].Name,
                        BackgroundColor = string.Format("#{0:X6}", random.Next(0x1000000)),
                        Data = perDayItems.Select(x => x.MessagesPerDay[idx].Count).ToArray()
                    };
                }
            }

            return model;
        }

        private async Task<ChartInfoViewModel> GenerateGroupActionReport(DateTime dtStart, DateTime dtEnd)
        {
            Random random = new Random();
            string[] backgroundColors = new string[7];
            for (int i = 0; i < 7; i++)
                backgroundColors[i] = string.Format("#{0:X6}", random.Next(0x1000000));

            var groupInfo = _userHelperService.GetSelectedGroup(User);
            var perDayItems = await _context.History_GroupActions
                .Where(x => x.Subscriber.IdGroup == groupInfo.Key)
                .Where(x => x.Dt <= dtEnd && x.Dt >= dtStart)
                .GroupBy(x => x.Dt.Date)
                .Select(x => new GroupActionsPerDayViewModel()
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

            var model = new ChartInfoViewModel()
            {
                Title = "Статистика по группе",
                YLabels = perDayItems.Select(x => x.Date.ToString("dd MMMM yyyyг.")).ToArray(),
                Dataset = new ChartInfoViewModel.ChartData[]
                {
                    new ChartInfoViewModel.ChartData()
                    {
                        BackgroundColor = backgroundColors[0],
                        Data = perDayItems.Select(x => x.JoinGroupCount).ToArray(),
                        Label = "Вступило в группу"
                    },
                    new ChartInfoViewModel.ChartData()
                    {
                        BackgroundColor = backgroundColors[1],
                        Data = perDayItems.Select(x => x.LeaveGroupCount).ToArray(),
                        Label = "Вышло из группы"
                    },
                    new ChartInfoViewModel.ChartData()
                    {
                        BackgroundColor = backgroundColors[2],
                        Data = perDayItems.Select(x => x.BlockMessagingCount).ToArray(),
                        Label = "Запрет сообщений"
                    },
                    new ChartInfoViewModel.ChartData()
                    {
                        BackgroundColor = backgroundColors[3],
                        Data = perDayItems.Select(x => x.AcceptMessagingCount).ToArray(),
                        Label = "Разрешение сообщений"
                    },
                    new ChartInfoViewModel.ChartData()
                    {
                        BackgroundColor = backgroundColors[4],
                        Data = perDayItems.Select(x => x.CancelMessagingCount).ToArray(),
                        Label = "Отмена сообщений"
                    },
                    new ChartInfoViewModel.ChartData()
                    {
                        BackgroundColor = backgroundColors[5],
                        Data = perDayItems.Select(x => x.BlockedCount).ToArray(),
                        Label = "Заблокировано"
                    },
                    new ChartInfoViewModel.ChartData()
                    {
                        BackgroundColor = backgroundColors[6],
                        Data = perDayItems.Select(x => x.UnblockedCount).ToArray(),
                        Label = "Разблокировано"
                    }
                },
                Legend = new ChartInfoViewModel.ChartLegend[]
                {
                    new ChartInfoViewModel.ChartLegend()
                    {
                        Color = backgroundColors[0],
                        Text = "Вступило в группу",
                        Value = perDayItems.Sum(x => x.JoinGroupCount)
                    },
                    new ChartInfoViewModel.ChartLegend()
                    {
                        Color = backgroundColors[1],
                        Text = "Вышло из группы",
                        Value = perDayItems.Sum(x => x.LeaveGroupCount)
                    },
                    new ChartInfoViewModel.ChartLegend()
                    {
                        Color = backgroundColors[2],
                        Text = "Запрет сообщений",
                        Value = perDayItems.Sum(x => x.BlockMessagingCount)
                    },
                    new ChartInfoViewModel.ChartLegend()
                    {
                        Color = backgroundColors[3],
                        Text = "Разрешение сообщений",
                        Value = perDayItems.Sum(x => x.AcceptMessagingCount)
                    },
                    new ChartInfoViewModel.ChartLegend()
                    {
                        Color = backgroundColors[4],
                        Text = "Отмена сообщений",
                        Value = perDayItems.Sum(x => x.CancelMessagingCount)
                    },
                    new ChartInfoViewModel.ChartLegend()
                    {
                        Color = backgroundColors[5],
                        Text = "Заблокировано",
                        Value = perDayItems.Sum(x => x.BlockedCount)
                    },
                    new ChartInfoViewModel.ChartLegend()
                    {
                        Color = backgroundColors[6],
                        Text = "Разблокировано",
                        Value = perDayItems.Sum(x => x.UnblockedCount)
                    }
                }
            };

            return model;
        }
    }
}