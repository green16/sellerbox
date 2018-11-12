using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SellerBox.Common.Helpers;
using SellerBox.Common.Hubs.Common;
using SellerBox.Common.Services;
using Microsoft.Extensions.Configuration;

namespace SellerBox.Common.Hubs
{
    public class MessagingHub : Hub
    {
        private readonly IConfiguration _configuration;
        private readonly DatabaseContext _context;
        private readonly VkPoolService _vkPoolService;

        private static readonly List<Pair<long, string>> connectedToGroups = new List<Pair<long, string>>();
        private static readonly List<Pair<long, SendingState>> sendingStates = new List<Pair<long, SendingState>>();

        public MessagingHub(IConfiguration configuration, DatabaseContext context, VkPoolService vkPoolService)
        {
            _configuration = configuration;
            _context = context;
            _vkPoolService = vkPoolService;
        }

        public SendingState[] GetState(long idGroup) => sendingStates.Where(x => x.Item1 == idGroup).Select(x => x.Item2).ToArray();

        public async Task<SendingState[]> Subscribe(long idGroup)
        {
            connectedToGroups.Add(new Pair<long, string>(idGroup, Context.ConnectionId));
            await Groups.AddToGroupAsync(Context.ConnectionId, idGroup.ToString());

            return GetState(idGroup);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var element = connectedToGroups.FirstOrDefault(x => x.Item2 == Context.ConnectionId);
            if (element != null)
            {
                connectedToGroups.RemoveAll(x => x.Item2 == Context.ConnectionId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, element.Item1.ToString());
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task UnSubscribe()
        {
            var element = connectedToGroups.FirstOrDefault(x => x.Item2 == Context.ConnectionId);
            if (element != null)
            {
                connectedToGroups.RemoveAll(x => x.Item2 == Context.ConnectionId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, element.Item1.ToString());
            }
        }

        public async Task Start(long idGroup, Guid idMessaging)
        {
            var sendingState = sendingStates.FirstOrDefault(x => x.Item1 == idGroup);
            if (sendingState == null)
            {
                sendingState = new Pair<long, SendingState>(idGroup, new SendingState(0, 0, idMessaging));
                sendingStates.Add(sendingState);
            }
            await Clients.Group(idGroup.ToString()).SendAsync("ProgressStarted", sendingState.Item2);

            await DoWork(idGroup, idMessaging);
        }

        private async Task Finish(long idGroup, Guid idMessaging)
        {
            sendingStates.RemoveAll(x => x.Item1 == idGroup && x.Item2.IdMessaging == idMessaging);
            await Clients.Group(idGroup.ToString()).SendAsync("ProgressFinished", idMessaging);
        }

        private async Task Progress(long idGroup, SendingState sendingState)
        {
            var currentSendingState = sendingStates.FirstOrDefault(x => x.Item1 == idGroup && x.Item2.IdMessaging == sendingState.IdMessaging);
            if (currentSendingState == null)
                sendingStates.Add(new Pair<long, SendingState>(idGroup, sendingState));
            else
            {
                currentSendingState.Item2.Total = sendingState.Total;
                currentSendingState.Item2.Progress = sendingState.Progress;
            }
            await Clients.Group(idGroup.ToString()).SendAsync("ProgressChanged", sendingState);
        }

        private async Task DoWork(long idGroup, Guid idMessaging)
        {
            MessageHelper messageHelper = new MessageHelper(_configuration, _context);
            messageHelper.MessageSent += async (sender, e) => await Progress(e.IdGroup, new SendingState(e.Total, e.Process, idMessaging));

            var messaging = await _context.Scheduler_Messaging.FirstOrDefaultAsync(x => x.Id == idMessaging);

            var vkApi = await _vkPoolService.GetGroupVkApi(idGroup);

            var ids = messaging.RecipientIds;

            await messageHelper.SendMessages(vkApi, idGroup, messaging.IdMessage, ids);
            var subscriberIds = await _context.Subscribers.Where(x => ids.Contains(x.IdVkUser)).Select(x => x.Id).ToArrayAsync();
            await _context.History_Messages.AddRangeAsync(subscriberIds.Select(x => new Models.Database.History_Messages()
            {
                IdMessage = messaging.IdMessage,
                IdSubscriber = x,
                IsOutgoingMessage = true,
                Dt = DateTime.UtcNow
            })).ContinueWith(result => _context.SaveChanges());

            await Finish(idGroup, idMessaging);
        }
    }
}
