using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SellerBox.Common.Services;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SellerBox.Common.Schedulers
{
    public class NotifyScheduler : BackgroundService
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

        public const int PeriodSeconds = 60;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private static ConcurrentQueue<NotifyEvent> NotifyEvents = new ConcurrentQueue<NotifyEvent>();

        public NotifyScheduler(IServiceScopeFactory serviceScopeFactory) : base()
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
#if DEBUG
                        Console.WriteLine($"NotifierService started at {DateTime.Now.ToString("HH:mm:ss:ffff")}");
#endif
                        await DoWork(scope.ServiceProvider);
#if DEBUG
                        Console.WriteLine($"NotifierService finished at {DateTime.Now.ToString("HH:mm:ss:ffff")}");
#endif                  
                    }

                    await Task.Delay(TimeSpan.FromSeconds(PeriodSeconds), stoppingToken); //5 seconds delay
                }
                catch { }
            } while (!stoppingToken.IsCancellationRequested);
        }

        public static void AddNotifyEvent(NotifyEvent notifyEvent)
        {
            if (notifyEvent == null)
                return;
            if (NotifyEvents == null)
                NotifyEvents = new ConcurrentQueue<NotifyEvent>();
            NotifyEvents.Enqueue((NotifyEvent)notifyEvent.Clone());
        }

        private async Task DoWork(IServiceProvider serviceProvider)
        {
            var _context = serviceProvider.GetService<DatabaseContext>();
            var _vkPoolService = serviceProvider.GetService<VkPoolService>();
            var _configuration = serviceProvider.GetService<IConfiguration>();

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
                                var smtpUser = _configuration.GetSection("Email").GetValue<string>("SmtpUser");
                                var smtpPassword = _configuration.GetSection("Email").GetValue<string>("SmtpPassword");
                                var smtpFrom = _configuration.GetSection("Email").GetValue<string>("SmtpFrom");
                                await Helpers.EmailHelper.SendEmail(smtpUser, smtpPassword, smtpFrom, notification.NotifyTo, messageText);
                            }
                            catch { }
                            break;
                        }
                }
            }
        }
    }
}
