using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SellerBox.Common.Schedulers
{
    public class MessagingScheduler : BackgroundService
    {
        public const int PeriodSeconds = 60;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        private static readonly ConcurrentDictionary<Guid, Task> activeTasks = new ConcurrentDictionary<Guid, Task>();

        public MessagingScheduler(IServiceScopeFactory serviceScopeFactory) : base()
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
                    Console.WriteLine($"MessagingScheduler started at {DateTime.Now.ToString("HH:mm:ss:ffff")}");
#endif
                    await Worker(scope.ServiceProvider);
#if DEBUG
                    Console.WriteLine($"MessagingScheduler finished at {DateTime.Now.ToString("HH:mm:ss:ffff")}");
#endif
                }

                await Task.Delay(TimeSpan.FromSeconds(PeriodSeconds), stoppingToken); //5 seconds delay
            }
            while (!stoppingToken.IsCancellationRequested);
        }

        private async Task Worker(IServiceProvider serviceProvider)
        {
            var _context = serviceProvider.GetService<DatabaseContext>();

            var dt = DateTime.UtcNow;

            var totalReadyToMessaging = await _context.Scheduler_Messaging
                .Where(x => x.Status == Models.Database.Common.MessagingStatus.Added)
                .Where(x => x.DtStart <= dt)
                .ToArrayAsync();
            if (totalReadyToMessaging.Length == 0)
                return;

            foreach (var readyToMessaging in totalReadyToMessaging)
            {
                readyToMessaging.Status = Models.Database.Common.MessagingStatus.InProgress;
                await _context.SaveChangesAsync();

                var newTask = Process(readyToMessaging.Id).ContinueWith(async result => activeTasks.TryRemove(await result, out Task task));
                activeTasks.TryAdd(readyToMessaging.Id, newTask);
            }
        }

        private async Task<Guid> Process(Guid idSchedulerMessaging)
        {
            AutoResetEvent waitHandler = new AutoResetEvent(false);

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetService<DatabaseContext>();
                var _configuration = scope.ServiceProvider.GetService<IConfiguration>();

                var messaging = await _context.Scheduler_Messaging.FirstOrDefaultAsync(x => x.Id == idSchedulerMessaging);
                if (messaging == null)
                    return idSchedulerMessaging;

                var idGroup = await _context.Scheduler_Messaging.Where(x => x.Id == idSchedulerMessaging).Select(x => x.Message.IdGroup).FirstOrDefaultAsync();

                //var connection = new HubConnectionBuilder().WithUrl("http://localhost:9854/messaginghub").Build();
                var connection = new HubConnectionBuilder().WithUrl(_configuration.GetValue<string>("SiteUrl") + "/messaginghub").Build();
                await connection.StartAsync();

                connection.On("ProgressFinished", new Type[] { typeof(Guid) }, (idMessaging) => Task.Run(() => waitHandler.Set()));

                await connection.InvokeAsync("Subscribe", idGroup);
                await connection.InvokeAsync("Start", idGroup, idSchedulerMessaging);

                waitHandler.WaitOne();

                await connection.StopAsync();

                messaging.Status = Models.Database.Common.MessagingStatus.Finished;
                messaging.DtEnd = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return idSchedulerMessaging;
            }
        }
    }
}
