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
        private readonly VkPoolService _vkPoolService;

        private static readonly List<Pair<long, string>> connectedToGroups = new List<Pair<long, string>>();
        private static readonly List<Pair<long, Pair<int, int>>> sendingStates = new List<Pair<long, Pair<int, int>>>();

        public MessagingHub(DatabaseContext context, UserHelperService userHelperService, VkPoolService vkPoolService)
        {
            _context = context;
            _userHelperService = userHelperService;
            _vkPoolService = vkPoolService;
        }

        public Pair<int, int> GetState(long idGroup) => sendingStates.FirstOrDefault(x => x.Item1 == idGroup)?.Item2;

        public async Task<Pair<int, int>> Subscribe(long idGroup)
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

        public async Task Start(long idGroup, Guid idMessage, params long[] ids)
        {
            var sendingState = sendingStates.FirstOrDefault(x => x.Item1 == idGroup);
            if (sendingState == null)
                sendingStates.Add(new Pair<long, Pair<int, int>>(idGroup, new Pair<int, int>(0, 0)));
            await Clients.Group(idGroup.ToString()).SendAsync("ProgressStarted");

            await DoWork(idGroup, idMessage, ids);
        }

        private async Task Finish(long idGroup)
        {
            sendingStates.RemoveAll(x => x.Item1 == idGroup);
            await Clients.Group(idGroup.ToString()).SendAsync("ProgressFinished");
        }

        private async Task Progress(long idGroup, int total, int progress)
        {
            var sendingState = sendingStates.FirstOrDefault(x => x.Item1 == idGroup);
            if (sendingState == null)
                sendingStates.Add(new Pair<long, Pair<int, int>>(idGroup, new Pair<int, int>(total, progress)));
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

        public async Task DoWork(long idGroup, Guid idMessage, params long[] ids)
        {
            MessageHelper messageHelper = new MessageHelper(_context);
            messageHelper.MessageSent += async (sender, e) => await Progress(e.IdGroup, e.Total, e.Current);

            var vkApi = await _vkPoolService.GetGroupVkApi(idGroup);

            var tasks = new Task[]
            {
                messageHelper.SendMessages(vkApi, idGroup, idMessage, ids),
                _context.History_Messages.AddRangeAsync(ids.Select(x => new Models.Database.History_Messages()
                {
                    IdMessage = idMessage,
                    IdSubscriber = _context.Subscribers.Where(y => y.IdVkUser == x).Select(y => y.Id).FirstOrDefault(),
                    IsOutgoingMessage = true,
                    Dt = DateTime.UtcNow
                })).ContinueWith(result => _context.SaveChanges())
            };

            await Task.WhenAll(tasks);

            await Finish(idGroup);
        }
    }
}
