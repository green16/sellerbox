using Microsoft.EntityFrameworkCore;
using SellerBox.Models.Database;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SellerBox.Common.Helpers
{
    public static class SubscriberHelper
    {
        public static async Task AddSubscriberToChain(DatabaseContext dbContext, long idGroup, Guid idSubscriber, Guid idChain)
        {
            var firstChainStepId = await dbContext.ChainContents.Where(x => x.IdChain == idChain).OrderBy(x => x.Index).Select(x => (Guid?)x.Id).FirstOrDefaultAsync();
            if (!firstChainStepId.HasValue)
                return;
            var dt = DateTime.UtcNow;

            await dbContext.SubscribersInChains.AddAsync(new SubscribersInChains()
            {
                IdSubscriber = idSubscriber,
                IdChainStep = firstChainStepId.Value,
                DtAdd = dt
            });

            await dbContext.History_SubscribersInChainSteps.AddAsync(new History_SubscribersInChainSteps()
            {
                IdChainStep = firstChainStepId.Value,
                IdSubscriber = idSubscriber,
                Dt = dt
            });

            Services.NotifierService.AddNotifyEvent(new Services.NotifierService.NotifyEvent()
            {
                Dt = dt,
                IdGroup = idGroup,
                IdElement = idChain,
                IdSubscriber = idSubscriber,
                SourceType = 1
            });
            Services.NotifierService.AddNotifyEvent(new Services.NotifierService.NotifyEvent()
            {
                Dt = dt,
                IdGroup = idGroup,
                IdElement = firstChainStepId.Value,
                IdSubscriber = idSubscriber,
                SourceType = 2
            });
            await dbContext.SaveChangesAsync();
        }
    }
}
