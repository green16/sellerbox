using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SellerBox.Common.Helpers;
using SellerBox.Common.Services;
using SellerBox.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SellerBox.Common.Schedulers
{
    public class TextScenariosScheduler : BackgroundService
    {
        public const int PeriodSeconds = 60;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public TextScenariosScheduler(IServiceScopeFactory serviceScopeFactory) : base()
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
                    Console.WriteLine($"TextScenariosScheduler started at {DateTime.Now.ToString("HH:mm:ss:ffff")}");
#endif
                    await TextScenarios(scope.ServiceProvider);
#if DEBUG
                    Console.WriteLine($"TextScenariosScheduler finished at {DateTime.Now.ToString("HH:mm:ss:ffff")}");
#endif              
                }

                await Task.Delay(TimeSpan.FromSeconds(PeriodSeconds), stoppingToken); //5 seconds delay
            }
            while (!stoppingToken.IsCancellationRequested);
        }

        private async Task TextScenarios(IServiceProvider serviceProvider)
        {
            var _configuration = serviceProvider.GetService<IConfiguration>();
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
                        Dt = dt
                    });
                    NotifierService.AddNotifyEvent(new NotifierService.NotifyEvent()
                    {
                        Dt = DateTime.UtcNow,
                        IdGroup = subscriberInChain.ChainStep.Chain.IdGroup,
                        IdElement = nextChainContent.Id,
                        IdSubscriber = subscriberInChain.IdSubscriber,
                        SourceType = 2
                    });
                }

                await dbContext.SaveChangesAsync();
            }

            foreach (var item in messagesToSend)
            {
                var idGroup = await dbContext.Messages.Where(x => x.Id == item.Key).Select(x => x.IdGroup).FirstOrDefaultAsync();

                var vkApi = await _vkPoolService.GetGroupVkApi(idGroup);

                var messageHelper = new MessageHelper(_configuration, dbContext);
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
