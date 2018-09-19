using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models.Database;
using WebApplication1.Models.Database.Common;

namespace WebApplication1.Common.Helpers
{
    public static class CallbackHelper
    {
        public static async Task<Guid?> ReplyToMessage(DatabaseContext dbContext, long idGroup, Guid idSubscriber, VkNet.Model.Message message)
        {
            if (!string.IsNullOrWhiteSpace(message.Text))
            {
                var scenario = await dbContext.Scenarios
                    .Where(x => x.IsEnabled)
                    .Where(x => x.IdGroup == idGroup)
                    .Include(x => x.Group)
                    .Include(x => x.Group.GroupAdmins)
                    .Where(x => x.Group.GroupAdmins.Any())
                    .FirstOrDefaultAsync(x => (x.IsStrictMatch && x.InputMessage.ToLower() == message.Text.ToLower()) || (!x.IsStrictMatch && message.Text.ToLower().Contains(x.InputMessage.ToLower())));

                if (scenario != null)
                {
                    await dbContext.History_Scenarios.AddAsync(new History_Scenarios()
                    {
                        IdScenario = scenario.Id,
                        IdSubscriber = idSubscriber,
                        Dt = DateTime.UtcNow
                    });
                    await dbContext.SaveChangesAsync();

                    return await ScenarioReply(dbContext, idGroup, idSubscriber, message, scenario);
                }
            }
            //Тут бот или картинки

            return null;
        }

        private static async Task AddSubscriberToChain(DatabaseContext dbContext, Guid idSubscriber, Guid idChain)
        {
            var firstChainStepId = await dbContext.ChainContents.Where(x => x.IdChain == idChain).OrderBy(x => x.Index).Select(x => x.Id).FirstOrDefaultAsync();
            if (firstChainStepId == default(Guid))
                return;

            await dbContext.SubscribersInChains.AddAsync(new SubscribersInChains()
            {
                IdSubscriber = idSubscriber,
                IdChainStep = firstChainStepId,
                DtAdd = DateTime.UtcNow
            });

            await dbContext.History_SubscribersInChainSteps.AddAsync(new History_SubscribersInChainSteps()
            {
                IdChainStep = firstChainStepId,
                IdSubscriber = idSubscriber,
                DtAdd = DateTime.UtcNow
            });

            await dbContext.SaveChangesAsync();
        }

        private static async Task<Guid?> ScenarioReply(DatabaseContext dbContext, long idGroup, Guid idSubscriber, VkNet.Model.Message message, Scenarios scenario)
        {
            if (scenario.Action == ScenarioActions.Message)
                return scenario.IdMessage;

            bool isSubscriberInChain = !scenario.IdChain.HasValue ? false : dbContext.SubscribersInChains
                .Where(x => x.IdSubscriber == idSubscriber)
                .Include(x => x.ChainStep)
                .Any(x => x.ChainStep.IdChain == scenario.IdChain.Value);

            bool isSubscriberInChain2 = !scenario.IdChain2.HasValue ? false : dbContext.SubscribersInChains
                .Where(x => x.IdSubscriber == idSubscriber)
                .Include(x => x.ChainStep)
                .Any(x => x.ChainStep.IdChain == scenario.IdChain2.Value);

            switch (scenario.Action)
            {
                case ScenarioActions.AddToChain:
                    {
                        if (isSubscriberInChain)
                            return scenario.IdErrorMessage;

                        await AddSubscriberToChain(dbContext, idSubscriber, scenario.IdChain.Value);
                        break;
                    }
                case ScenarioActions.ChangeChain:
                    {
                        if (isSubscriberInChain2)
                        {
                            var subscriberInChain = dbContext.SubscribersInChains
                                .Include(x => x.ChainStep)
                                .Where(x => x.IdSubscriber == idSubscriber && x.ChainStep.IdChain == scenario.IdChain2.Value);
                            dbContext.SubscribersInChains.RemoveRange(subscriberInChain);
                            await dbContext.SaveChangesAsync();
                        }

                        if (isSubscriberInChain)
                            return scenario.IdErrorMessage;

                        await AddSubscriberToChain(dbContext, idSubscriber, scenario.IdChain.Value);

                        break;
                    }
                case ScenarioActions.RemoveFromChain:
                    {
                        if (!isSubscriberInChain2)
                            return scenario.IdErrorMessage;

                        var subscriberInChain = dbContext.SubscribersInChains
                                .Include(x => x.ChainStep)
                                .Where(x => x.IdSubscriber == idSubscriber && x.ChainStep.IdChain == scenario.IdChain2.Value);
                        dbContext.SubscribersInChains.RemoveRange(subscriberInChain);
                        await dbContext.SaveChangesAsync();

                        break;
                    }
                default:
                    throw new NotImplementedException();
            }

            return null;
        }
    }
}
