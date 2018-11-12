using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SellerBox.Models.Database;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SellerBox.Common.Schedulers
{
    public class ShortUrlsScheduler : BackgroundService
    {
        public const int PeriodSeconds = 60;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ShortUrlsScheduler(IServiceScopeFactory serviceScopeFactory) : base()
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
                    Console.WriteLine($"ShortUrlsScheduler started at {DateTime.Now.ToString("HH:mm:ss:ffff")}");
#endif
                    await DoWork(scope.ServiceProvider);
#if DEBUG
                    Console.WriteLine($"ShortUrlsScheduler finished at {DateTime.Now.ToString("HH:mm:ss:ffff")}");
#endif              
                }

                await Task.Delay(TimeSpan.FromSeconds(PeriodSeconds), stoppingToken); //5 seconds delay
            }
            while (!stoppingToken.IsCancellationRequested);
        }

        private async Task DoWork(IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetService<DatabaseContext>();

            var dt = DateTime.UtcNow;

            var shortUrlsScenarios = await dbContext.ShortUrlsScenarios
                .Include(x => x.ShortUrl)
                .Where(x => x.IsEnabled)
                .Where(x => dbContext.SubscribersInChains
                    .Any(y => y.IdChainStep == x.IdCheckingChainContent))
                .ToArrayAsync();//Все сценарии для которых есть люди в проверяемой цепочке

            foreach (var shortUrlScenario in shortUrlsScenarios)
            {
                var subscribersInChain = await dbContext.SubscribersInChains
                    .Include(x => x.Subscriber)
                    .Where(x => x.Subscriber.IdGroup == shortUrlScenario.ShortUrl.IdGroup)
                    .Where(x => x.IdChainStep == shortUrlScenario.IdCheckingChainContent)
                    .Where(x => x.DtAdd.AddSeconds(shortUrlScenario.CheckAfterSeconds) < dt)//Время на переход истекло
                    .Include(x => x.Subscriber.ShortUrlsPassedClicks)
                    .Where(x => x.Subscriber.ShortUrlsPassedClicks.All(y => y.IdShortUrlsScenario != shortUrlScenario.Id))
                    .ToArrayAsync(); // Все подписчики в проверяемой цепочке
                foreach (var subscriberInChain in subscribersInChain)
                {
                    var subscriberShortUrlClick = await dbContext.History_ShortUrlClicks
                        .Where(x => x.Dt > shortUrlScenario.DtCreate)//После создания сценария
                        .Where(x => x.IdSubscriber == subscriberInChain.IdSubscriber)
                        .FirstOrDefaultAsync();

                    var isSubscriber = await dbContext.Subscribers.AnyAsync(x => x.Id == subscriberInChain.IdSubscriber);

                    Guid? IdChainStep = null;

                    if (subscriberShortUrlClick != null) // Если есть переход
                    {
                        if (!shortUrlScenario.CheckIsSubscriber || isSubscriber) // Если есть нет проверки на вступление группу или человек подписан на группу
                        {
                            var isSubscriberNotInChain = await dbContext.SubscribersInChains
                                .Where(x => x.IdSubscriber == subscriberInChain.IdSubscriber)
                                .Include(x => x.ChainStep)
                                .AllAsync(x => x.ChainStep.IdChain != shortUrlScenario.IdGoToChain);
                            if (!isSubscriberNotInChain)
                                IdChainStep = await dbContext.ChainContents
                                    .Where(x => x.IdChain == shortUrlScenario.IdGoToChain)
                                    .OrderBy(x => x.Index)
                                    .Select(x => x.Id)
                                    .FirstOrDefaultAsync();
                        }
                        else if (shortUrlScenario.IdGoToErrorChain3.HasValue) // Если есть проверка на вступление в группу и человек не подписался
                        {
                            var isSubscriberNotInChain = await dbContext.SubscribersInChains
                                .Where(x => x.IdSubscriber == subscriberInChain.IdSubscriber)
                                .Include(x => x.ChainStep)
                                .AllAsync(x => x.ChainStep.IdChain != shortUrlScenario.IdGoToErrorChain3.Value);
                            if (!isSubscriberNotInChain)
                                IdChainStep = await dbContext.ChainContents
                                    .Where(x => x.IdChain == shortUrlScenario.IdGoToErrorChain3.Value)
                                    .OrderBy(x => x.Index)
                                    .Select(x => x.Id)
                                    .FirstOrDefaultAsync();
                        }
                    }
                    else // Если нет перехода
                    {
                        if ((!shortUrlScenario.CheckIsSubscriber || !isSubscriber) && shortUrlScenario.IdGoToErrorChain1.HasValue) // Если нет проверки на вступление группу или человек не подписался
                        {
                            var isSubscriberNotInChain = await dbContext.SubscribersInChains
                                        .Where(x => x.IdSubscriber == subscriberInChain.IdSubscriber)
                                        .Include(x => x.ChainStep)
                                        .AllAsync(x => x.ChainStep.IdChain != shortUrlScenario.IdGoToErrorChain1.Value);
                            if (!isSubscriberNotInChain)
                                IdChainStep = await dbContext.ChainContents
                                    .Where(x => x.IdChain == shortUrlScenario.IdGoToErrorChain1.Value)
                                    .OrderBy(x => x.Index)
                                    .Select(x => x.Id)
                                    .FirstOrDefaultAsync();
                        }
                        else if (shortUrlScenario.CheckIsSubscriber && isSubscriber && shortUrlScenario.IdGoToErrorChain2.HasValue) // Если есть проверка на вступление в группу и человек подписался
                        {
                            var isSubscriberNotInChain = await dbContext.SubscribersInChains
                                .Where(x => x.IdSubscriber == subscriberInChain.IdSubscriber)
                                .Include(x => x.ChainStep)
                                .AllAsync(x => x.ChainStep.IdChain != shortUrlScenario.IdGoToErrorChain2.Value);
                            if (!isSubscriberNotInChain)
                                IdChainStep = await dbContext.ChainContents
                                    .Where(x => x.IdChain == shortUrlScenario.IdGoToErrorChain2.Value)
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

                    await dbContext.ShortUrlsPassedClicks.AddAsync(new ShortUrlsPassedClicks()
                    {
                        Dt = dt,
                        IdShortUrlsScenario = shortUrlScenario.Id,
                        IdSubscriber = subscriberInChain.IdSubscriber
                    });

                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
