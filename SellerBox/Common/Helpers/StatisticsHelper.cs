using Microsoft.EntityFrameworkCore;
using SellerBox.ViewModels.Statistics;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SellerBox.Common.Helpers
{
    public static class StatisticsHelper
    {
        private static readonly Random random = new Random();

        public delegate Task<ChartInfoViewModel> ReportGenerator(DatabaseContext _context, long idGroup, DateTime dtStart, DateTime dtEnd);

        public static async Task<ChartInfoViewModel> GenerateScenariosReport(DatabaseContext _context, long idGroup, DateTime dtStart, DateTime dtEnd)
        {
            var perDayItems = await _context.History_Scenarios
                .Where(x => x.Subscriber.IdGroup == idGroup)
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

        public static async Task<ChartInfoViewModel> GenerateShortUrlsReport(DatabaseContext _context, long idGroup, DateTime dtStart, DateTime dtEnd)
        {
            var perDayItems = await _context.History_ShortUrlClicks
                .Where(x => x.Subscriber.IdGroup == idGroup)
                .Where(x => x.Dt <= dtEnd && x.Dt >= dtStart)
                .GroupBy(x => x.Dt.Date)
                .Select(x => new ShortUrlsInfoViewModel()
                {
                    Date = x.Key,
                    ShortUrlsPerDay = x.GroupBy(z => z.ShortUrl.Name).Select(z => new ShortUrlsPerDayViewModel()
                    {
                        Name = z.Key,
                        Count = z.LongCount()
                    }).ToArray()
                })
                .ToArrayAsync();

            var datasetsСount = perDayItems.Any() ? perDayItems.Max(x => x.ShortUrlsPerDay.Length) : 0;

            var model = new ChartInfoViewModel()
            {
                Title = "Статистика по горячим ссылкам",
                YLabels = perDayItems.Select(x => x.Date.ToString("dd MMMM yyyyг.")).ToArray(),
                Dataset = new ChartInfoViewModel.ChartData[datasetsСount],
                Legend = new ChartInfoViewModel.ChartLegend[]
                {
                    new ChartInfoViewModel.ChartLegend()
                    {
                        Color = "rgba(255, 255, 255, 0)",
                        Text = "Всего",
                        Value = perDayItems.Sum(x => x.ShortUrlsPerDay.Sum(z => z.Count))
                    }
                }
            };

            foreach (var item in perDayItems)
            {
                for (int idx = 0; idx < item.ShortUrlsPerDay.Length; idx++)
                {
                    model.Dataset[idx] = new ChartInfoViewModel.ChartData()
                    {
                        Label = item.ShortUrlsPerDay[idx].Name,
                        Stack = item.ShortUrlsPerDay[idx].Name,
                        BackgroundColor = string.Format("#{0:X6}", random.Next(0x1000000)),
                        Data = perDayItems.Select(x => x.ShortUrlsPerDay[idx].Count).ToArray()
                    };
                }
            }

            return model;
        }

        public static async Task<ChartInfoViewModel> GenerateMessagesReport(DatabaseContext _context, long idGroup, DateTime dtStart, DateTime dtEnd)
        {
            var perDayItems = await _context.History_Messages
                .Where(x => x.Subscriber.IdGroup == idGroup)
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

        public static async Task<ChartInfoViewModel> GenerateChatScenariosReport(DatabaseContext _context, long idGroup, DateTime dtStart, DateTime dtEnd)
        {
            var perDayItems = await _context.History_SubscribersInChatScenariosContents
                .Where(x => x.Subscriber.IdGroup == idGroup)
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

        public static async Task<ChartInfoViewModel> GenerateChainsReport(DatabaseContext _context, long idGroup, DateTime dtStart, DateTime dtEnd)
        {
            var perDayItems = await _context.History_SubscribersInChainSteps
                .Where(x => x.Subscriber.IdGroup == idGroup)
                .Where(x => x.Dt <= dtEnd && x.Dt >= dtStart)
                .GroupBy(x => x.Dt.Date)
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

        public static async Task<ChartInfoViewModel> GenerateGroupActionReport(DatabaseContext _context, long idGroup, DateTime dtStart, DateTime dtEnd)
        {
            string[] backgroundColors = new string[7];
            for (int i = 0; i < 7; i++)
                backgroundColors[i] = string.Format("#{0:X6}", random.Next(0x1000000));

            var perDayItems = await _context.History_GroupActions
                .Where(x => x.IdGroup == idGroup)
                .Where(x => x.Dt <= dtEnd && x.Dt >= dtStart)
                .GroupBy(x => x.Dt.Date)
                .Select(x => new GroupActionsPerDayViewModel()
                {
                    Date = x.Key,
                    AcceptMessagingCount = x.LongCount(y => y.ActionType == (int)Models.Database.Common.GroupActionTypes.AcceptMessaging),
                    BlockedCount = x.LongCount(y => y.ActionType == (int)Models.Database.Common.GroupActionTypes.Blocked),
                    BlockMessagingCount = x.LongCount(y => y.ActionType == (int)Models.Database.Common.GroupActionTypes.BlockMessaging),
                    CancelMessagingCount = x.LongCount(y => y.ActionType == (int)Models.Database.Common.GroupActionTypes.CancelMessaging),
                    JoinGroupCount = x.LongCount(y => y.ActionType == (int)Models.Database.Common.GroupActionTypes.JoinGroup),
                    LeaveGroupCount = x.LongCount(y => y.ActionType == (int)Models.Database.Common.GroupActionTypes.LeaveGroup),
                    UnblockedCount = x.LongCount(y => y.ActionType == (int)Models.Database.Common.GroupActionTypes.Unblocked)
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
