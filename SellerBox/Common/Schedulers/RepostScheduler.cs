using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SellerBox.Models.Database;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SellerBox.Common.Schedulers
{
    public class RepostScheduler : BackgroundService
    {
        public const int PeriodSeconds = 60;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RepostScheduler(IServiceScopeFactory serviceScopeFactory) : base()
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
#if DEBUG
                    Console.WriteLine($"RepostScheduler started at {DateTime.Now.ToString("HH:mm:ss:ffff")}");
#endif
                    await RepostScenarios(scope.ServiceProvider);
#if DEBUG
                    Console.WriteLine($"RepostScheduler finished at {DateTime.Now.ToString("HH:mm:ss:ffff")}");
#endif              
                }

                await Task.Delay(TimeSpan.FromSeconds(PeriodSeconds), stoppingToken); //5 seconds delay
            }
            while (!stoppingToken.IsCancellationRequested);
        }

        private async Task RepostScenarios(IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetService<DatabaseContext>();

            DateTime dt = DateTime.UtcNow;

            var repostScenarios = await dbContext.RepostScenarios
                .Where(x => x.IsEnabled)
                .Include(x => x.WallPost)
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
                    var subscriberRepost = await dbContext.SubscriberReposts
                        .Where(x => !x.IsProcessed)//Ещё не обработаны
                                                   //.Where(x => x.DtRepost >= subscriberInChain.DtAdd) //Репосты после добавления в ChainContent
                        .Where(x => x.IdSubscriber == subscriberInChain.IdSubscriber)
                        .Where(x => repostScenario.CheckAllPosts ||//Проверять все посты
                            (repostScenario.CheckLastPosts && dbContext.WallPosts
                                                                    .OrderByDescending(y => y.DtAdd)
                                                                    .Select(y => y.Id)
                                                                    .ToArray()
                                                                    .IndexOf(x.Id) <= repostScenario.LastPostsCount.Value) || // или последние N
                            (!repostScenario.CheckLastPosts && x.IdPost == repostScenario.IdPost.Value)) //или конкретный пост
                        .FirstOrDefaultAsync();

                    bool isSubscriber = await dbContext.Subscribers.AnyAsync(x => x.Id == subscriberInChain.IdSubscriber);

                    Guid? IdChainStep = null;

                    if (subscriberRepost != null) // Если есть репост
                    {
                        if (!repostScenario.CheckIsSubscriber || isSubscriber) // Если есть нет проверки на вступление группу или человек подписан на группу
                        {
                            var isSubscriberNotInChain = await dbContext.SubscribersInChains
                                .Where(x => x.IdSubscriber == subscriberInChain.IdSubscriber)
                                .Include(x => x.ChainStep)
                                .AllAsync(x => x.ChainStep.IdChain != repostScenario.IdGoToChain.Value);
                            if (!isSubscriberNotInChain)
                                IdChainStep = await dbContext.ChainContents
                                    .Where(x => x.IdChain == repostScenario.IdGoToChain.Value)
                                    .OrderBy(x => x.Index)
                                    .Select(x => x.Id)
                                    .FirstOrDefaultAsync();
                        }
                        else if (repostScenario.IdGoToErrorChain3.HasValue) // Если есть проверка на вступление в группу и человек не подписался
                        {
                            var isSubscriberNotInChain = await dbContext.SubscribersInChains
                                .Where(x => x.IdSubscriber == subscriberInChain.IdSubscriber)
                                .Include(x => x.ChainStep)
                                .AllAsync(x => x.ChainStep.IdChain != repostScenario.IdGoToErrorChain3.Value);
                            if (!isSubscriberNotInChain)
                                IdChainStep = await dbContext.ChainContents
                                    .Where(x => x.IdChain == repostScenario.IdGoToErrorChain3.Value)
                                    .OrderBy(x => x.Index)
                                    .Select(x => x.Id)
                                    .FirstOrDefaultAsync();
                        }
                    }
                    else // Если нет репоста
                    {
                        if ((!repostScenario.CheckIsSubscriber || !isSubscriber) && repostScenario.IdGoToErrorChain1.HasValue) // Если нет проверки на вступление группу или человек не подписался
                        {
                            var isSubscriberNotInChain = await dbContext.SubscribersInChains
                                        .Where(x => x.IdSubscriber == subscriberInChain.IdSubscriber)
                                        .Include(x => x.ChainStep)
                                        .AllAsync(x => x.ChainStep.IdChain != repostScenario.IdGoToErrorChain1.Value);
                            if (!isSubscriberNotInChain)
                                IdChainStep = await dbContext.ChainContents
                                    .Where(x => x.IdChain == repostScenario.IdGoToErrorChain1.Value)
                                    .OrderBy(x => x.Index)
                                    .Select(x => x.Id)
                                    .FirstOrDefaultAsync();
                        }
                        else if (repostScenario.CheckIsSubscriber && isSubscriber && repostScenario.IdGoToErrorChain2.HasValue) // Если есть проверка на вступление в группу и человек подписался
                        {
                            var isSubscriberNotInChain = await dbContext.SubscribersInChains
                                .Where(x => x.IdSubscriber == subscriberInChain.IdSubscriber)
                                .Include(x => x.ChainStep)
                                .AllAsync(x => x.ChainStep.IdChain != repostScenario.IdGoToErrorChain2.Value);
                            if (!isSubscriberNotInChain)
                                IdChainStep = await dbContext.ChainContents
                                    .Where(x => x.IdChain == repostScenario.IdGoToErrorChain2.Value)
                                    .OrderBy(x => x.Index)
                                    .Select(x => x.Id)
                                    .FirstOrDefaultAsync();
                        }
                    }

                    if (IdChainStep.HasValue)
                    {
                        await dbContext.SubscribersInChains.AddAsync(new SubscribersInChains()
                        {
                            IdSubscriber = subscriberInChain.IdSubscriber,
                            DtAdd = dt,
                            IdChainStep = IdChainStep.Value
                        });
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
