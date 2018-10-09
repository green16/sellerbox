using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SellerBox.Common.Services
{
    public class NotifierService : BackgroundService
    {
        public class NotifyEvent : ICloneable
        {
            public long IdGroup { get; set; }
            public DateTime Dt { get; set; }
            public int SourceType { get; set; }
            public Guid IdElement { get; set; }
            public Guid IdSubscriber { get; set; }

            public object Clone()
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(this);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<NotifyEvent>(json);
            }
        }

        private readonly IServiceScopeFactory _serviceScopeFactory;
        private Task _executingTask;
        private static readonly AutoResetEvent waitHandler = new AutoResetEvent(false);
        private static ConcurrentQueue<NotifyEvent> NotifyEvents = new ConcurrentQueue<NotifyEvent>();

        public NotifierService(IServiceScopeFactory serviceScopeFactory) : base()
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _executingTask = Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        waitHandler.WaitOne();

                        if (!NotifyEvents.Any())
                        {
                            waitHandler.Reset();
                            continue;
                        }

                        using (var scope = _serviceScopeFactory.CreateScope())
                        {
                            await DoWork(scope.ServiceProvider);
                        }
                    }
                    catch { }
                }
            }, stoppingToken);

            return _executingTask;
        }

        public static void AddNotifyEvent(NotifyEvent notifyEvent)
        {
            if (notifyEvent == null)
                return;
            if (NotifyEvents == null)
                NotifyEvents = new ConcurrentQueue<NotifyEvent>();
            NotifyEvents.Enqueue((NotifyEvent)notifyEvent.Clone());
            waitHandler.Set();
        }

        private async Task DoWork(IServiceProvider serviceProvider)
        {
            var _context = serviceProvider.GetService<DatabaseContext>();
            var _vkPoolService = serviceProvider.GetService<VkPoolService>();

            while (NotifyEvents.TryDequeue(out NotifyEvent message))
            {
                var notification = await _context.Notifications
                    .Where(x => x.IdGroup == message.IdGroup)
                    .Where(x => x.IsEnabled)
                    .FirstOrDefaultAsync(x => x.ElementType == message.SourceType && x.IdElement == message.IdElement.ToString());
                if (notification == null)
                    continue;

                var subscriber = await _context.Subscribers.FindAsync(message.IdSubscriber);
                string messageText = $"Сработало событие: '{notification.Name}'";
                if (subscriber != null)
                    messageText += $", пользователь http://vk.com/id{subscriber.IdVkUser}";

                switch (notification.NotificationType)
                {
                    case 0:
                        {
                            var vkApi = await _vkPoolService.GetGroupVkApi(message.IdGroup);
                            await vkApi.Messages.SendAsync(new VkNet.Model.RequestParams.MessagesSendParams()
                            {
                                PeerId = long.Parse(notification.NotifyTo),
                                Message = messageText
                            });
                            continue;
                        }
                    case 1:
                        {
                            if (string.IsNullOrWhiteSpace(notification.NotifyTo))
                                continue;
                            try
                            {
                                await Helpers.EmailHelper.SendEmail(notification.NotifyTo, messageText);
                            }
                            catch { }
                            break;
                        }
                }
            }
        }
    }
}
