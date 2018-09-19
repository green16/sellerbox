using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApplication1.Common.Helpers;
using WebApplication1.Common.Services;
using WebApplication1.Models.Database;

namespace WebApplication1.Common
{
    public class Scheduler : IHostedService
    {
        public const int PeriodSeconds = 60;
        private Task _executingTask;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();

        public Scheduler(IServiceScopeFactory serviceScopeFactory) : base()
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            // Store the task we're executing
            _executingTask = ExecuteAsync(_stoppingCts.Token);

            // If the task is completed then return it,
            // this will bubble cancellation and failure to the caller
            if (_executingTask.IsCompleted)
            {
                return _executingTask;
            }

            // Otherwise it's running
            return Task.CompletedTask;
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop called without start
            if (_executingTask == null)
            {
                return;
            }

            try
            {
                // Signal cancellation to the executing method
                _stoppingCts.Cancel();
            }
            finally
            {
                // Wait until the task completes or the stop token triggers
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }

        protected virtual async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                await Process();

                await Task.Delay(TimeSpan.FromSeconds(PeriodSeconds), stoppingToken); //5 seconds delay
            }
            while (!stoppingToken.IsCancellationRequested);
        }

        protected async Task Process()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                await ProcessInScope(scope.ServiceProvider);
            }
        }

        public async Task ProcessInScope(IServiceProvider serviceProvider)
        {
#if DEBUG
            Console.WriteLine($"Execute at {DateTime.Now.ToString("HH:mm:ss:ffff")}");
#endif
            await RepostScenarios(serviceProvider);
            await TextScenarios(serviceProvider);
            await BirthdayScenarios(serviceProvider);
            //await WallBirthdayScenarios(serviceProvider);
#if DEBUG
            Console.WriteLine($"Finished at {DateTime.Now.ToString("HH:mm:ss:ffff")}");
#endif
        }

        private async Task TextScenarios(IServiceProvider serviceProvider)
        {
            DatabaseContext dbContext = serviceProvider.GetService<DatabaseContext>();
            var _vkPoolService = serviceProvider.GetService<VkPoolService>();

            DateTime dt = DateTime.UtcNow;
            bool isDayTime = DateTime.Now.Hour > 8 && DateTime.Now.Hour < 20;

            var subscribersInChains = await dbContext.SubscribersInChains
                .Include(x => x.ChainStep)
                .Include(x => x.ChainStep.Chain)
                .Include(x => x.ChainStep.Chain.Group)
                .Include(x => x.ChainStep.Chain.Group.GroupAdmins)
                .Where(x => x.ChainStep.Chain.Group.GroupAdmins.Any())
                .Where(x => x.DtAdd.AddSeconds(x.ChainStep.SendAfterSeconds) < dt)
                .Where(x => !x.ChainStep.IsOnlyDayTime || isDayTime)
                .Where(x => x.ChainStep.Chain.IsEnabled)
                .Include(x => x.Subscriber)
                .ToArrayAsync();

            var messagesToSend = new Dictionary<Guid, List<Subscribers>>();
            foreach (SubscribersInChains subscriberInChain in subscribersInChains)
            {
                var nextChainContent = await dbContext.ChainContents
                    .Where(x => x.IdChain == subscriberInChain.ChainStep.IdChain)
                    .Where(x => x.Index > subscriberInChain.ChainStep.Index)
                    .OrderBy(x => x.Index)
                    .FirstOrDefaultAsync(x => x.Index > subscriberInChain.ChainStep.Index);

                if (nextChainContent == null)
                {
                    if (subscriberInChain.IsSended)
                        continue;
                    subscriberInChain.IsSended = true;
                }

                if (subscriberInChain.ChainStep.IdExcludeFromChain.HasValue)
                {
                    var removingSubscriberInChain = await dbContext.SubscribersInChains
                        .Where(x => x.IdSubscriber == subscriberInChain.IdSubscriber)
                        .Include(x => x.ChainStep)
                        .Where(x => x.ChainStep.IdChain == subscriberInChain.ChainStep.IdExcludeFromChain.Value)
                        .FirstOrDefaultAsync();

                    if (removingSubscriberInChain != null)
                        dbContext.SubscribersInChains.Remove(removingSubscriberInChain);
                }
                if (subscriberInChain.Subscriber.IsChatAllowed.HasValue && subscriberInChain.Subscriber.IsChatAllowed.Value)
                    if (messagesToSend.ContainsKey(subscriberInChain.ChainStep.IdMessage.Value))
                        messagesToSend[subscriberInChain.ChainStep.IdMessage.Value].Add(subscriberInChain.Subscriber);
                    else
                        messagesToSend.Add(subscriberInChain.ChainStep.IdMessage.Value, new List<Subscribers>() { subscriberInChain.Subscriber });

                if (subscriberInChain.ChainStep.IdGoToChain.HasValue)
                    nextChainContent = await dbContext.ChainContents
                        .Where(x => x.IdChain == subscriberInChain.ChainStep.IdGoToChain.Value)
                        .OrderBy(x => x.Index)
                        .FirstOrDefaultAsync();

                if (nextChainContent != null)
                {
                    subscriberInChain.IsSended = false;
                    subscriberInChain.IdChainStep = nextChainContent.Id;
                    subscriberInChain.DtAdd = dt;

                    await dbContext.History_SubscribersInChainSteps.AddAsync(new History_SubscribersInChainSteps()
                    {
                        IdChainStep = nextChainContent.Id,
                        IdSubscriber = subscriberInChain.IdSubscriber,
                        DtAdd = dt
                    });
                }

                await dbContext.SaveChangesAsync();
            }

            foreach (var item in messagesToSend)
            {
                var idGroup = await dbContext.Messages.Where(x => x.Id == item.Key).Select(x => x.IdGroup).FirstOrDefaultAsync();

                var vkApi = await _vkPoolService.GetGroupVkApi(idGroup);

                MessageHelper messageHelper = new MessageHelper(dbContext);
                await messageHelper.SendMessages(vkApi, idGroup, item.Key, item.Value.Select(x => x.IdVkUser).ToArray());
                await dbContext.History_Messages.AddRangeAsync(item.Value.Select(x => new History_Messages()
                {
                    IdMessage = item.Key,
                    IdSubscriber = x.Id,
                    IsOutgoingMessage = true,
                    Dt = dt
                })).ContinueWith(result => dbContext.SaveChanges());
            }
        }

        private async Task RepostScenarios(IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetService<DatabaseContext>();

            DateTime dt = DateTime.UtcNow;

            var repostScenarios = await dbContext.RepostScenarios
                .Where(x => x.IsEnabled)
                .Include(x => x.WallPost)
                .Include(x => x.WallPost.Group)
                .Include(x => x.WallPost.Group.GroupAdmins)
                .Where(x => x.WallPost.Group.GroupAdmins.Any())
                .Where(x => dbContext.SubscribersInChains
                    .Any(y => y.IdChainStep == x.IdCheckingChainContent))
                    .ToArrayAsync();//Все репостные сценарии для которых есть люди в проверяемой цепочке

            foreach (RepostScenarios repostScenario in repostScenarios)
            {
                var subscribersInChain = await dbContext.SubscribersInChains
                    .Include(x => x.Subscriber)
                    .Where(x => x.Subscriber.IdGroup == repostScenario.WallPost.IdGroup)
                    .Where(x => x.IdChainStep == repostScenario.IdCheckingChainContent)
                    .Where(x => x.DtAdd.AddSeconds(repostScenario.CheckAfterSeconds) < dt)//Время на репост истекло
                    .Include(x => x.Subscriber.CheckedSubscribersInRepostScenarios)
                    .Where(x => x.Subscriber.CheckedSubscribersInRepostScenarios.All(y => y.IdRepostScenario != repostScenario.Id))
                    .ToArrayAsync(); // Все подписчики в проверяемой цепочке
                foreach (var subscriberInChain in subscribersInChain)
                {
                    var repost = await dbContext.SubscriberReposts
                        .Where(x => !x.IsProcessed)//Ещё не обработаны
                        .Where(x => x.DtRepost >= subscriberInChain.DtAdd) //Репосты после добавления в ChainContent
                        .Where(x => x.IdSubscriber == subscriberInChain.IdSubscriber)
                        .Where(x => repostScenario.CheckAllPosts ||//Проверять все посты
                            (repostScenario.CheckLastPosts && dbContext.WallPosts
                                                                    .OrderByDescending(y => y.DtAdd)
                                                                    .Select(y => y.Id)
                                                                    .ToArray()
                                                                    .IndexOf(x.Id) <= repostScenario.LastPostsCount.Value) || // или последние N
                            (!repostScenario.CheckLastPosts && x.IdPost == repostScenario.IdPost.Value))//или конкретный пост
                        .FirstOrDefaultAsync();
                    if (repost == null) //если нет репоста
                    {
                        if (repostScenario.IdGoToChain2.HasValue)
                        {
                            bool isSubscriberNotInChain = await dbContext.SubscribersInChains
                                .Where(x => x.IdSubscriber == subscriberInChain.IdSubscriber)
                                .Include(x => x.ChainStep)
                                .AllAsync(x => x.ChainStep.IdChain != repostScenario.IdGoToChain2.Value);

                            if (isSubscriberNotInChain)
                            {
                                await dbContext.SubscribersInChains.AddAsync(new SubscribersInChains()
                                {
                                    IdSubscriber = subscriberInChain.IdSubscriber,
                                    DtAdd = dt,
                                    IdChainStep = await dbContext.ChainContents
                                        .Where(x => x.IdChain == repostScenario.IdGoToChain2.Value)
                                        .OrderBy(x => x.Index)
                                        .Select(x => x.Id)
                                        .FirstOrDefaultAsync()
                                });
                                await dbContext.SaveChangesAsync();
                            }
                        }
                        else continue;
                    }
                    else
                    {
                        if (repostScenario.IdGoToChain.HasValue)
                        {
                            bool isSubscriberNotInChain = await dbContext.SubscribersInChains
                                .Where(x => x.IdSubscriber == subscriberInChain.IdSubscriber)
                                .Include(x => x.ChainStep)
                                .AllAsync(x => x.ChainStep.IdChain != repostScenario.IdGoToChain.Value);

                            if (isSubscriberNotInChain)
                            {
                                await dbContext.SubscribersInChains.AddAsync(new SubscribersInChains()
                                {
                                    IdSubscriber = subscriberInChain.IdSubscriber,
                                    DtAdd = DateTime.UtcNow,
                                    IdChainStep = await dbContext.ChainContents
                                        .Where(x => x.IdChain == repostScenario.IdGoToChain.Value)
                                        .OrderBy(x => x.Index)
                                        .Select(x => x.Id)
                                        .FirstOrDefaultAsync()
                                });
                            }
                        }

                        repost.IsProcessed = true;
                    }

                    await dbContext.CheckedSubscribersInRepostScenarios.AddAsync(new CheckedSubscribersInRepostScenarios()
                    {
                        DtCheck = dt,
                        IdRepostScenario = repostScenario.Id,
                        IdSubscriber = subscriberInChain.IdSubscriber,
                    });

                    await dbContext.SaveChangesAsync();
                }
            }
        }

        private async Task BirthdayScenarios(IServiceProvider serviceProvider)
        {
            var _context = serviceProvider.GetService<DatabaseContext>();
            var _vkPoolService = serviceProvider.GetService<VkPoolService>();

            DateTime dt = DateTime.Now;

            var birthdayScenarios = await _context.BirthdayScenarios
                .Where(x => x.IsEnabled)
                .Include(x => x.Group)
                .Include(x => x.Group.GroupAdmins)
                .Where(x => x.Group.GroupAdmins.Any() && x.SendAt <= dt.Hour)
                .ToArrayAsync();
            if (!birthdayScenarios.Any())
                return;

            foreach (var birthdayScenario in birthdayScenarios)
            {
                var subscribers = await _context.Subscribers
                    .Where(x => x.IdGroup == birthdayScenario.IdGroup)
                    .Where(x => !_context.History_Birthday.Any(y => y.Id == x.Id && y.DtSend.Year == dt.Year))
                    .Include(x => x.VkUser)
                    .Where(x => x.VkUser.Birthday.HasValue && x.VkUser.Birthday.Value.Month == dt.Date.Month && x.VkUser.Birthday.Value.Day == dt.Date.AddDays(birthdayScenario.DaysBefore).Day)
                    .Where(x => x.IsChatAllowed.HasValue && x.IsChatAllowed.Value)
                    .ToArrayAsync();
                if (!subscribers.Any())
                    continue;

                var idGroup = (await _context.Messages
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == birthdayScenario.IdMessage)).IdGroup;

                var vkApi = await _vkPoolService.GetGroupVkApi(idGroup);

                MessageHelper messageHelper = new MessageHelper(_context);
                var tasks = new Task[]
                {
                    messageHelper.SendMessages(vkApi, idGroup, birthdayScenario.IdMessage, subscribers.Select(x => x.IdVkUser).ToArray()),
                    new Task(async () =>
                    {
                        var addingTasks = new Task[]
                        {
                            _context.History_Birthday.AddRangeAsync(subscribers.Select(x => new History_Birthday()
                            {
                                DtSend = dt,
                                IdSubscriber = x.Id,
                                IdGroup = birthdayScenario.IdGroup
                            })),
                            _context.History_Messages.AddRangeAsync(subscribers.Select(x => new History_Messages()
                            {
                                Dt = dt,
                                IdMessage = birthdayScenario.IdMessage,
                                IdSubscriber = x.Id
                            }))
                        };

                        await Task.WhenAll(addingTasks).ContinueWith(x => _context.SaveChanges());
                    })
                };

                await Task.WhenAll(tasks);
            }
        }

        private async Task WallBirthdayScenarios(IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetService<DatabaseContext>();
            var _vkPoolService = serviceProvider.GetService<VkPoolService>();

            DateTime dt = DateTime.Now;

            var wallBirthdayScenarios = await dbContext.BirthdayWallScenarios
                .Where(x => x.IsEnabled)
                .Include(x => x.Group)
                .Include(x => x.Group.GroupAdmins)
                .Where(x => x.Group.GroupAdmins.Any() && x.SendAt <= dt.Hour)
                .ToArrayAsync();

            if (!wallBirthdayScenarios.Any())
                return;

            foreach (var wallBirthdayScenario in wallBirthdayScenarios)
            {
                var vkUsersIds = await dbContext.Subscribers
                    .Where(x => x.IdGroup == wallBirthdayScenario.IdGroup)
                    .Where(x => !dbContext.History_BirthdayWall.Any(y => y.IdVkUser == x.IdVkUser && y.DtSend.Year == dt.Year))
                    .Include(x => x.VkUser)
                    .Where(x => x.VkUser.Birthday.HasValue && x.VkUser.Birthday.Value.Month == dt.Date.Month)
                    .Where(x => x.IsChatAllowed.HasValue && x.IsChatAllowed.Value)
                    .Select(x => x.IdVkUser).ToArrayAsync();
                if (!vkUsersIds.Any())
                    continue;

                var idGroup = await dbContext.Messages.Where(x => x.Id == wallBirthdayScenario.IdMessage).Select(x => x.IdGroup).FirstOrDefaultAsync();

                var idGroupAdmin = await dbContext.GroupAdmins.Where(x => x.IdGroup == idGroup).Select(x => x.User.IdVk).FirstOrDefaultAsync();
                var vkApi = await _vkPoolService.GetUserVkApi(idGroupAdmin);

                WallMessageHelper wallMessageHelper = new WallMessageHelper(dbContext);
                await wallMessageHelper.SendWallMessage(vkApi, idGroup, wallBirthdayScenario.IdMessage, vkUsersIds);
                await dbContext.History_BirthdayWall.AddRangeAsync(vkUsersIds.Select(x => new History_BirthdayWall()
                {
                    DtSend = dt,
                    IdVkUser = x,
                    IdGroup = wallBirthdayScenario.IdGroup
                })).ContinueWith(result => dbContext.SaveChanges());
            }
        }
    }
}
