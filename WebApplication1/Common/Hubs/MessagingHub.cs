using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Common.Helpers;
using WebApplication1.Common.Services;

namespace WebApplication1.Common.Hubs
{
    public class MessagingHub : Hub
    {
        private readonly DatabaseContext _context;
        private readonly UserHelperService _userHelperService;

        private static readonly List<Pair<int, string>> connectedToGroups = new List<Pair<int, string>>();
        private static readonly List<Pair<int, Pair<int, int>>> sendingStates = new List<Pair<int, Pair<int, int>>>();

        public MessagingHub(DatabaseContext context, UserHelperService userHelperService)
        {
            _context = context;
            _userHelperService = userHelperService;
        }

        public Pair<int, int> GetState(int idGroup) => sendingStates.FirstOrDefault(x => x.Item1 == idGroup)?.Item2;

        public async Task<Pair<int, int>> Subscribe(int idGroup)
        {
            connectedToGroups.Add(new Pair<int, string>(idGroup, Context.ConnectionId));
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

        public async Task Start(int idGroup, Guid idMessage, params int[] ids)
        {
            var sendingState = sendingStates.FirstOrDefault(x => x.Item1 == idGroup);
            if (sendingState == null)
                sendingStates.Add(new Pair<int, Pair<int, int>>(idGroup, new Pair<int, int>(0, 0)));
            await Clients.Group(idGroup.ToString()).SendAsync("ProgressStarted");

            await DoWork(idGroup, idMessage, ids);
        }

        private async Task Finish(int idGroup)
        {
            sendingStates.RemoveAll(x => x.Item1 == idGroup);
            await Clients.Group(idGroup.ToString()).SendAsync("ProgressFinished");
        }

        private async Task Progress(int idGroup, int total, int progress)
        {
            var sendingState = sendingStates.FirstOrDefault(x => x.Item1 == idGroup);
            if (sendingState == null)
                sendingStates.Add(new Pair<int, Pair<int, int>>(idGroup, new Pair<int, int>(total, progress)));
            else
            {
                if (sendingState.Item2 == null)
                    sendingState.Item2 = new Pair<int, int>(total, progress);
                else
                {
                    sendingState.Item2.Item1 = total;
                    sendingState.Item2.Item2 = progress;
                }
            }
            await Clients.Group(idGroup.ToString()).SendAsync("ProgressChanged", total, progress);
        }

        public async Task DoWork(int idGroup, Guid idMessage, params int[] ids)
        {
            MessageHelper messageHelper = new MessageHelper(_context);
            messageHelper.MessageSent += async (sender, e) => await Progress(e.IdGroup, e.Total, e.Current);
            await messageHelper.SendMessages(idGroup, idMessage, ids);

            await Finish(idGroup);
        }
    }
}
