using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SellerBox.Common.Helpers;
using SellerBox.Common.Services;
using SellerBox.Models.Database;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SellerBox.Common.Schedulers
{
    public class BirthdayScenariosScheduler : IHostedService
    {
        public const int PeriodSeconds = 60;
        private Task _executingTask;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();

        public BirthdayScenariosScheduler(IServiceScopeFactory serviceScopeFactory) : base()
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
            Console.WriteLine($"BirthdayScenariosScheduler started at {DateTime.Now.ToString("HH:mm:ss:ffff")}");
#endif
            await BirthdayScenarios(serviceProvider);
#if DEBUG
            Console.WriteLine($"BirthdayScenariosScheduler finished at {DateTime.Now.ToString("HH:mm:ss:ffff")}");
#endif
        }

        private async Task BirthdayScenarios(IServiceProvider serviceProvider)
        {
            var _configuration = serviceProvider.GetService<IConfiguration>();
            var _context = serviceProvider.GetService<DatabaseContext>();
            var _vkPoolService = serviceProvider.GetService<VkPoolService>();

            var dt = DateTime.Now;

            var birthdayScenarios = await _context.BirthdayScenarios
                .Where(x => x.IsEnabled)
                .Include(x => x.Group)
                .Include(x => x.Group.GroupAdmins)
                .Where(x => x.Group.GroupAdmins.Any() && x.SendAt <= dt.Hour)
                .ToArrayAsync();
            if (!birthdayScenarios.Any())
                return;

            foreach (var birthdayScenario in birthdayScenarios)
            {
                var subscribers = await _context.Subscribers
                    .Where(x => x.IdGroup == birthdayScenario.IdGroup)
                    .Where(x => !_context.History_Birthday.Any(y => y.Id == x.Id && y.Dt.Year == dt.Year))
                    .Include(x => x.VkUser)
                    .Where(x => x.VkUser.Birthday.HasValue && x.VkUser.Birthday.Value.Month == dt.Date.Month && x.VkUser.Birthday.Value.Day == dt.Date.AddDays(birthdayScenario.DaysBefore).Day)
                    .Where(x => x.IsChatAllowed.HasValue && x.IsChatAllowed.Value)
                    .Where(x => x.VkUser.Sex.HasValue && x.VkUser.Sex.Value == birthdayScenario.IsMale)
                    .ToArrayAsync();
                if (!subscribers.Any())
                    continue;

                var idGroup = (await _context.Messages
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == birthdayScenario.IdMessage))?.IdGroup;
                if (!idGroup.HasValue)
                    continue;

                var vkApi = await _vkPoolService.GetGroupVkApi(idGroup.Value);
                if (vkApi == null)
                    continue;

                var messageHelper = new MessageHelper(_configuration, _context);
                var tasks = new Task[]
                {
                    messageHelper.SendMessages(vkApi, idGroup.Value, birthdayScenario.IdMessage, subscribers.Select(x => x.IdVkUser).ToArray()),
                    new Task(async () =>
                    {
                        var addingTasks = new Task[]
                        {
                            _context.History_Birthday.AddRangeAsync(subscribers.Select(x => new History_Birthday()
                            {
                                Dt = dt,
                                IdSubscriber = x.Id,
                                IdGroup = birthdayScenario.IdGroup
                            })),
                            _context.History_Messages.AddRangeAsync(subscribers.Select(x => new History_Messages()
                            {
                                Dt = dt,
                                IdMessage = birthdayScenario.IdMessage,
                                IdSubscriber = x.Id
                            }))
                        };

                        await Task.WhenAll(addingTasks).ContinueWith(x => _context.SaveChanges());
                    })
                };

                await Task.WhenAll(tasks);
            }
        }
    }
}
