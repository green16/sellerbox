using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApplication1.Models.Database;

namespace WebApplication1.Common.Schedulers
{
    public class RepostScheduler : IHostedService
    {
        public const int PeriodSeconds = 60;
        private Task _executingTask;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();

        public RepostScheduler(IServiceScopeFactory serviceScopeFactory) : base()
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
            Console.WriteLine($"RepostScheduler started at {DateTime.Now.ToString("HH:mm:ss:ffff")}");
#endif
            await RepostScenarios(serviceProvider);
#if DEBUG
            Console.WriteLine($"RepostScheduler finished at {DateTime.Now.ToString("HH:mm:ss:ffff")}");
#endif
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
    }
}
