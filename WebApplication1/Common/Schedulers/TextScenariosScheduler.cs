using Microsoft.EntityFrameworkCore;
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

namespace WebApplication1.Common.Schedulers
{
    public class TextScenariosScheduler : IHostedService
    {
        public const int PeriodSeconds = 60;
        private Task _executingTask;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();

        public TextScenariosScheduler(IServiceScopeFactory serviceScopeFactory) : base()
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
            Console.WriteLine($"TextScenariosScheduler started at {DateTime.Now.ToString("HH:mm:ss:ffff")}");
#endif
            await TextScenarios(serviceProvider);
#if DEBUG
            Console.WriteLine($"TextScenariosScheduler finished at {DateTime.Now.ToString("HH:mm:ss:ffff")}");
#endif
        }

        private async Task TextScenarios(IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetService<DatabaseContext>();
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

                var messageHelper = new MessageHelper(dbContext);
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
    }
}
