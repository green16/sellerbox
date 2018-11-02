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
                .Where(x => x.Dt < dtEnd && x.Dt >= dtStart)
                .GroupBy(x => x.Dt.Date)
                .Select(x => new ScenariosMessagesPerDayViewModel()
                {
                    Date = x.Key,
                    Count = x.Count()
                })
                .ToArrayAsync();

            var model = new ChartInfoViewModel()
            {
                Title = "Статистика по сценариям",
                YLabels = perDayItems.Select(x => x.Date.ToString("dd MMMM yyyyг.")).ToArray(),
                Dataset = new ChartInfoViewModel.ChartData[]
                {
                    new ChartInfoViewModel.ChartData("Получено сообщений", "rgba(0, 0, 255, 1)", perDayItems.Select(x => x.Count).ToArray())
                },
                Legend = new ChartInfoViewModel.ChartLegend[]
                {
                    new ChartInfoViewModel.ChartLegend("Получено сообщений", "rgba(0, 0, 255, 1)", perDayItems.Sum(x => x.Count))
                }
            };

            return model;
        }

        public static async Task<ChartInfoViewModel> GenerateShortUrlsReport(DatabaseContext _context, long idGroup, DateTime dtStart, DateTime dtEnd)
        {
            var perDayItems = await _context.History_ShortUrlClicks
                .Where(x => x.Subscriber.IdGroup == idGroup)
                .Where(x => x.Dt < dtEnd && x.Dt >= dtStart)
                .GroupBy(x => x.Dt.Date)
                .Select(x => new ShortUrlsInfoViewModel()
                {
                    Date = x.Key,
                    ShortUrlsPerDay = x.GroupBy(z => z.ShortUrl.Name).Select(z => new ShortUrlsPerDayViewModel()
                    {
                        Name = z.Key,
                        Count = z.Count()
                    }).ToArray()
                })
                .ToArrayAsync();

            var datasetsСount = perDayItems.Any() ? perDayItems.Max(x => x.ShortUrlsPerDay.Length) : 0;

            var model = new ChartInfoViewModel()
            {
                Title = "Статистика по горячим ссылкам",
                YLabels = perDayItems.Select(x => x.Date.ToString("dd MMMM yyyyг.")).ToArray(),
                Dataset = perDayItems.SelectMany(x =>
                {
                    int shortUrlsPerDayIdx = 0;
                    return x.ShortUrlsPerDay.Select(y =>
                    {
                        var result = new ChartInfoViewModel.ChartData(
                            x.ShortUrlsPerDay[shortUrlsPerDayIdx].Name,
                            string.Format("#{0:X6}", random.Next(0x1000000)),
                            perDayItems.Select(z => z.ShortUrlsPerDay[shortUrlsPerDayIdx].Count).ToArray());
                        shortUrlsPerDayIdx++;
                        return result;
                    });
                }).ToArray(),
                Legend = new ChartInfoViewModel.ChartLegend[]
                {
                    new ChartInfoViewModel.ChartLegend("Всего", "rgba(255, 255, 255, 0)", perDayItems.Sum(x => x.ShortUrlsPerDay.Sum(z => z.Count)))
                }
            };

            return model;
        }

        public static async Task<ChartInfoViewModel> GenerateMessagesReport(DatabaseContext _context, long idGroup, DateTime dtStart, DateTime dtEnd)
        {
            var perDayItems = await _context.History_Messages
                .Where(x => x.Subscriber.IdGroup == idGroup)
                .Where(x => x.Dt < dtEnd && x.Dt >= dtStart)
                .GroupBy(x => x.Dt.Date)
                .Select(x => new MessagesPerDayViewModel()
                {
                    Date = x.Key,
                    Sended = x.Count(z => z.IsOutgoingMessage),
                    Received = x.Count(z => !z.IsOutgoingMessage)
                })
                .ToArrayAsync();

            var model = new ChartInfoViewModel()
            {
                Title = "Статистика по сообщениям",
                YLabels = perDayItems.Select(x => x.Date.ToString("dd MMMM yyyyг.")).ToArray(),
                Dataset = new ChartInfoViewModel.ChartData[]
                {
                    new ChartInfoViewModel.ChartData("Отправлено", "rgba(255, 0, 0, 1)", perDayItems.Select(x => x.Sended).ToArray()),
                    new ChartInfoViewModel.ChartData("Получено", "rgba(0, 0, 255, 1)", perDayItems.Select(x => x.Received).ToArray())
                },
                Legend = new ChartInfoViewModel.ChartLegend[]
                {
                    new ChartInfoViewModel.ChartLegend("Получено сообщений", "rgba(255, 0, 0, 1)", perDayItems.Sum(x => x.Received)),
                    new ChartInfoViewModel.ChartLegend("Отправлено сообщений", "rgba(0, 0, 255, 1)", perDayItems.Sum(x => x.Sended))
                }
            };

            return model;
        }

        public static async Task<ChartInfoViewModel> GenerateChatScenariosReport(DatabaseContext _context, long idGroup, DateTime dtStart, DateTime dtEnd)
        {
            var perDayItems = await _context.History_SubscribersInChatScenariosContents
                .Where(x => x.Subscriber.IdGroup == idGroup)
                .Where(x => x.Dt < dtEnd && x.Dt >= dtStart)
                .GroupBy(x => x.Dt.Date)
                .Select(x => new ChatContentsMessagesPerDayViewModel()
                {
                    Date = x.Key,
                    Count = x.Count()
                })
                .ToArrayAsync();
            var model = new ChartInfoViewModel()
            {
                Title = "Статистика по чатботу",
                YLabels = perDayItems.Select(x => x.Date.ToString("dd MMMM yyyyг.")).ToArray(),
                Dataset = new ChartInfoViewModel.ChartData[]
                {
                    new ChartInfoViewModel.ChartData("Получено сообщений", "rgba(0, 0, 255, 1)", perDayItems.Select(x => x.Count).ToArray())
                },
                Legend = new ChartInfoViewModel.ChartLegend[]
                {
                    new ChartInfoViewModel.ChartLegend("Получено сообщений", "rgba(0, 0, 255, 1)", perDayItems.Sum(x => x.Count))
                }
            };
            return model;
        }

        public static async Task<ChartInfoViewModel> GenerateChainsReport(DatabaseContext _context, long idGroup, DateTime dtStart, DateTime dtEnd)
        {
            var perDayItems = await _context.History_SubscribersInChainSteps
                .Where(x => x.Subscriber.IdGroup == idGroup)
                .Where(x => x.Dt < dtEnd && x.Dt >= dtStart)
                .GroupBy(x => x.Dt.Date)
                .Select(x => new ChainInfoViewModel()
                {
                    Date = x.Key,
                    MessagesPerDay = x.GroupBy(z => z.ChainStep.Chain.Name).Select(z => new ChainMessagesPerDayViewModel()
                    {
                        Name = z.Key,
                        Count = z.Count()
                    }).ToArray()
                })
                .ToArrayAsync();

            var model = new ChartInfoViewModel()
            {
                Title = "Статистика по цепочкам",
                YLabels = perDayItems.Select(x => x.Date.ToString("dd MMMM yyyyг.")).ToArray(),
                Dataset = perDayItems.SelectMany(x =>
                {
                    int messagePerDayIdx = 0;
                    return x.MessagesPerDay.Select(y =>
                    {
                        var result = new ChartInfoViewModel.ChartData(
                            x.MessagesPerDay[messagePerDayIdx].Name, 
                            string.Format("#{0:X6}", random.Next(0x1000000)), 
                            perDayItems.Select(z => z.MessagesPerDay[messagePerDayIdx].Count).ToArray());
                        messagePerDayIdx++;
                        return result;
                    });
                }).ToArray(),
                Legend = new ChartInfoViewModel.ChartLegend[]
                {
                    new ChartInfoViewModel.ChartLegend("Всего", "rgba(255, 255, 255, 0)", perDayItems.Sum(x => x.MessagesPerDay.Sum(z => z.Count)))
                }
            };

            return model;
        }

        public static async Task<ChartInfoViewModel> GenerateGroupActionReport(DatabaseContext _context, long idGroup, DateTime dtStart, DateTime dtEnd)
        {
            string[] backgroundColors = new string[7];
            for (int i = 0; i < 7; i++)
                backgroundColors[i] = string.Format("#{0:X6}", random.Next(0x1000000));

            var perDayItems = await _context.History_GroupActions
                .Where(x => x.IdGroup == idGroup)
                .Where(x => x.Dt < dtEnd && x.Dt >= dtStart)
                .GroupBy(x => x.Dt.Date)
                .Select(x => new GroupActionsPerDayViewModel()
                {
                    Date = x.Key,
                    AcceptMessagingCount = x.Count(y => y.ActionType == (int)Models.Database.Common.GroupActionTypes.AcceptMessaging),
                    BlockedCount = x.Count(y => y.ActionType == (int)Models.Database.Common.GroupActionTypes.Blocked),
                    BlockMessagingCount = x.Count(y => y.ActionType == (int)Models.Database.Common.GroupActionTypes.BlockMessaging),
                    CancelMessagingCount = x.Count(y => y.ActionType == (int)Models.Database.Common.GroupActionTypes.CancelMessaging),
                    JoinGroupCount = x.Count(y => y.ActionType == (int)Models.Database.Common.GroupActionTypes.JoinGroup),
                    LeaveGroupCount = x.Count(y => y.ActionType == (int)Models.Database.Common.GroupActionTypes.LeaveGroup),
                    UnblockedCount = x.Count(y => y.ActionType == (int)Models.Database.Common.GroupActionTypes.Unblocked)
                })
                .ToArrayAsync();

            var model = new ChartInfoViewModel()
            {
                Title = "Статистика по группе",
                YLabels = perDayItems.Select(x => x.Date.ToString("dd MMMM yyyyг.")).ToArray(),
                Dataset = new ChartInfoViewModel.ChartData[]
                {
                    new ChartInfoViewModel.ChartData("Вступило в группу", backgroundColors[0], perDayItems.Select(x => x.JoinGroupCount).ToArray()),
                    new ChartInfoViewModel.ChartData("Вышло из группы", backgroundColors[1], perDayItems.Select(x => x.LeaveGroupCount).ToArray()),
                    new ChartInfoViewModel.ChartData("Запрет сообщений", backgroundColors[2], perDayItems.Select(x => x.BlockMessagingCount).ToArray()),
                    new ChartInfoViewModel.ChartData("Разрешение сообщений", backgroundColors[3], perDayItems.Select(x => x.AcceptMessagingCount).ToArray()),
                    new ChartInfoViewModel.ChartData("Отмена сообщений", backgroundColors[4], perDayItems.Select(x => x.CancelMessagingCount).ToArray()),
                    new ChartInfoViewModel.ChartData("Заблокировано", backgroundColors[5], perDayItems.Select(x => x.BlockedCount).ToArray()),
                    new ChartInfoViewModel.ChartData("Разблокировано", backgroundColors[6], perDayItems.Select(x => x.UnblockedCount).ToArray())
                },
                Legend = new ChartInfoViewModel.ChartLegend[]
                {
                    new ChartInfoViewModel.ChartLegend("Вступило в группу", backgroundColors[0], perDayItems.Sum(x => x.JoinGroupCount)),
                    new ChartInfoViewModel.ChartLegend("Вышло из группы", backgroundColors[1], perDayItems.Sum(x => x.LeaveGroupCount)),
                    new ChartInfoViewModel.ChartLegend("Запрет сообщений", backgroundColors[2], perDayItems.Sum(x => x.BlockMessagingCount)),
                    new ChartInfoViewModel.ChartLegend("Разрешение сообщений", backgroundColors[3], perDayItems.Sum(x => x.AcceptMessagingCount)),
                    new ChartInfoViewModel.ChartLegend("Отмена сообщений", backgroundColors[4], perDayItems.Sum(x => x.CancelMessagingCount)),
                    new ChartInfoViewModel.ChartLegend("Заблокировано", backgroundColors[5], perDayItems.Sum(x => x.BlockedCount)),
                    new ChartInfoViewModel.ChartLegend("Разблокировано", backgroundColors[6], perDayItems.Sum(x => x.UnblockedCount))
                }
            };

            return model;
        }
    }
}
