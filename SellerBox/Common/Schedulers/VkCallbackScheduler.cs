using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SellerBox.Common.Helpers;
using SellerBox.Common.Services;
using SellerBox.Models.Database;
using SellerBox.Models.Vk;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SellerBox.Common.Schedulers
{
    public class VkCallbackScheduler : BackgroundService
    {
        private delegate Task CallbackFunctionHandler(IServiceProvider serviceProvider, DatabaseContext context, CallbackMessage message);

        public const int PeriodSeconds = 5;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private static readonly Tuple<string, CallbackFunctionHandler>[] callbackFunctions = new Tuple<string, CallbackFunctionHandler>[]
        {
            new Tuple<string, CallbackFunctionHandler>("message_new", MessageNew),
            new Tuple<string, CallbackFunctionHandler>("message_allow", MessagesAllow),
            new Tuple<string, CallbackFunctionHandler>("message_deny", MessagesDeny),
            new Tuple<string, CallbackFunctionHandler>("wall_post_new", WallPostNew),
            new Tuple<string, CallbackFunctionHandler>("wall_repost", WallRepost),
            new Tuple<string, CallbackFunctionHandler>("group_leave", GroupLeave),
            new Tuple<string, CallbackFunctionHandler>("group_join", GroupJoin),
            new Tuple<string, CallbackFunctionHandler>("user_block", UserBlock),
            new Tuple<string, CallbackFunctionHandler>("user_unblock", UserUnblock)
        };

        #region Callback functions

        private static async Task MessageNew(IServiceProvider serviceProvider, DatabaseContext context, CallbackMessage message)
        {
            if (await IsPassedCallbackMessage(context, message))
                return;
            var innerMessage = VkNet.Model.Message.FromJson(new VkNet.Utils.VkResponse(message.Object));

            if (!innerMessage.UserId.HasValue || innerMessage.UserId.Value <= 0)
                return;

            var vkPoolService = serviceProvider.GetService<VkPoolService>();

            var subscriber = await VkHelper.CreateSubscriber(context, vkPoolService, message.IdGroup, innerMessage.UserId.Value);
            if (subscriber == null)
                return;

            await context.History_Messages.AddAsync(new History_Messages()
            {
                Dt = DateTime.UtcNow,
                IsOutgoingMessage = false,
                IdSubscriber = subscriber.Id,
                Text = innerMessage.Body
            });

            bool isCancelMessaging = innerMessage.Body?.ToLower() == "стоп";
            if (!isCancelMessaging && (!subscriber.IsChatAllowed.HasValue || !subscriber.IsChatAllowed.Value))
            {
                await context.History_GroupActions.AddAsync(new History_GroupActions()
                {
                    ActionType = (int)Models.Database.Common.GroupActionTypes.AcceptMessaging,
                    IdGroup = message.IdGroup,
                    IdSubscriber = subscriber.Id,
                    Dt = DateTime.UtcNow
                });

                subscriber.IsChatAllowed = true;
            }

            await context.SaveChangesAsync();

            var vkApi = await vkPoolService.GetGroupVkApi(message.IdGroup);

            if (isCancelMessaging)
            {
                await context.History_GroupActions.AddAsync(new History_GroupActions()
                {
                    ActionType = (int)Models.Database.Common.GroupActionTypes.CancelMessaging,
                    IdGroup = message.IdGroup,
                    IdSubscriber = subscriber.Id,
                    Dt = DateTime.UtcNow
                });

                if (!subscriber.IsChatAllowed.HasValue || subscriber.IsChatAllowed.Value)
                {
                    subscriber.IsChatAllowed = false;
                    if (vkApi != null)
                    {
                        await context.History_Messages.AddAsync(new History_Messages()
                        {
                            Dt = DateTime.UtcNow,
                            IsOutgoingMessage = true,
                            IdSubscriber = subscriber.Id,
                            Text = "Вы успешно отписаны от сообщений группы"
                        });

                        await vkApi.Messages.SendAsync(new VkNet.Model.RequestParams.MessagesSendParams()
                        {
                            Message = "Вы успешно отписаны от сообщений группы",
                            UserId = innerMessage.UserId
                        });
                    }
                }

                await context.SaveChangesAsync();
                return;
            }

            if (vkApi == null)
                return;

            var replyToMessageResult = await CallbackHelper.ReplyToMessage(context, message.IdGroup, subscriber.Id, innerMessage);
            if (replyToMessageResult == null)
                return;

            if (replyToMessageResult.Item1.HasValue)
            {
                var _configuration = serviceProvider.GetService<IConfiguration>();

                var messageHelper = new MessageHelper(_configuration, context);
                await messageHelper.SendMessages(vkApi, message.IdGroup, replyToMessageResult.Item1.Value, innerMessage.UserId.Value);
                await context.History_Messages.AddAsync(new History_Messages()
                {
                    Dt = DateTime.UtcNow,
                    IsOutgoingMessage = true,
                    IdSubscriber = subscriber.Id,
                    IdMessage = replyToMessageResult.Item1
                });
                await context.SaveChangesAsync();
            }
            else if (replyToMessageResult.Item2)
                await vkApi.Messages.MarkAsReadAsync(innerMessage.UserId.ToString(), groupId: message.IdGroup);
        }

        private static async Task MessagesAllow(IServiceProvider serviceProvider, DatabaseContext context, CallbackMessage message)
        {
            var innerMessage = VkNet.Model.Message.FromJson(new VkNet.Utils.VkResponse(message.Object));

            var vkPoolService = serviceProvider.GetService<VkPoolService>();

            var subscriber = await VkHelper.CreateSubscriber(context, vkPoolService, message.IdGroup, innerMessage.UserId.Value);
            if (subscriber == null)
                return;

            if (!subscriber.IsChatAllowed.HasValue || !subscriber.IsChatAllowed.Value)
                subscriber.IsChatAllowed = true;

            await context.History_GroupActions.AddAsync(new History_GroupActions()
            {
                ActionType = (int)Models.Database.Common.GroupActionTypes.AcceptMessaging,
                IdGroup = message.IdGroup,
                IdSubscriber = subscriber.Id,
                Dt = DateTime.UtcNow
            });

            await context.SaveChangesAsync();
        }

        private static async Task MessagesDeny(IServiceProvider serviceProvider, DatabaseContext context, CallbackMessage message)
        {
            var innerMessage = VkNet.Model.Message.FromJson(new VkNet.Utils.VkResponse(message.Object));

            var vkPoolService = serviceProvider.GetService<VkPoolService>();

            var subscriber = await VkHelper.CreateSubscriber(context, vkPoolService, message.IdGroup, innerMessage.UserId.Value);
            if (subscriber == null)
                return;

            if (!subscriber.IsChatAllowed.HasValue || subscriber.IsChatAllowed.Value)
                subscriber.IsChatAllowed = false;

            await context.History_GroupActions.AddAsync(new History_GroupActions()
            {
                ActionType = (int)Models.Database.Common.GroupActionTypes.BlockMessaging,
                IdGroup = message.IdGroup,
                IdSubscriber = subscriber.Id,
                Dt = DateTime.UtcNow
            });

            await context.SaveChangesAsync();
        }

        private static async Task WallPostNew(IServiceProvider serviceProvider, DatabaseContext context, CallbackMessage message)
        {
            if (await IsPassedCallbackMessage(context, message))
                return;

            var newPost = VkNet.Model.Post.FromJson(new VkNet.Utils.VkResponse(message.Object));
            if (!newPost.Id.HasValue || await context.WallPosts.AnyAsync(x => x.IdGroup == message.IdGroup && x.IdVk == newPost.Id))
                return;

            var newWallPost = new WallPosts()
            {
                DtAdd = newPost.Date ?? DateTime.UtcNow,
                IdGroup = message.IdGroup,
                IdVk = newPost.Id.Value,
                Text = newPost.Text
            };

            await context.WallPosts.AddAsync(newWallPost);
            await context.SaveChangesAsync();

            var vkPoolService = serviceProvider.GetService<VkPoolService>();

            var subscriber = await VkHelper.CreateSubscriber(context, vkPoolService, message.IdGroup, newPost.FromId.Value);
            if (subscriber == null)
                return;

            await context.History_WallPosts.AddAsync(new History_WallPosts()
            {
                Dt = DateTime.UtcNow,
                IdPost = newWallPost.Id,
                IdSubscriber = subscriber.Id,
                IsRepost = false
            });
            await context.SaveChangesAsync();
        }

        private static async Task WallRepost(IServiceProvider serviceProvider, DatabaseContext context, CallbackMessage message)
        {
            if (await IsPassedCallbackMessage(context, message))
                return;

            var repost = VkNet.Model.Wall.FromJson(new VkNet.Utils.VkResponse(message.Object));
            if (!repost.FromId.HasValue || repost.FromId.Value <= 0)
                return;

            var repostedPost = repost.CopyHistory.FirstOrDefault();

            var post = await context.WallPosts.FirstOrDefaultAsync(x => x.IdGroup == message.IdGroup && x.IdVk == repostedPost.Id);
            if (post == null)
            {
                post = new WallPosts()
                {
                    DtAdd = repostedPost.Date ?? DateTime.UtcNow,
                    IdGroup = message.IdGroup,
                    IdVk = repostedPost.Id.Value,
                };
                await context.WallPosts.AddAsync(post);
                await context.SaveChangesAsync();
            }

            var vkPoolService = serviceProvider.GetService<VkPoolService>();

            var subscriber = await VkHelper.CreateSubscriber(context, vkPoolService, message.IdGroup, repost.FromId.Value);
            if (subscriber == null)
                return;

            bool hasSubscriberRepost = await context.SubscriberReposts
                .AnyAsync(x => x.WallPost.IdVk == repostedPost.Id &&
                               x.Subscriber.IdVkUser == subscriber.IdVkUser &&
                               x.DtRepost == repost.Date);

            if (!hasSubscriberRepost)
                await context.SubscriberReposts.AddAsync(new SubscriberReposts()
                {
                    DtRepost = repost.Date ?? DateTime.UtcNow,
                    IdPost = post.Id,
                    IdSubscriber = subscriber.Id,
                    Text = repost.Text,
                });

            await context.History_WallPosts.AddAsync(new History_WallPosts()
            {
                Dt = DateTime.UtcNow,
                IdPost = post.Id,
                IdSubscriber = subscriber.Id,
                IsRepost = true
            });

            await context.SaveChangesAsync();
        }

        private static async Task GroupLeave(IServiceProvider serviceProvider, DatabaseContext context, CallbackMessage message)
        {
            var innerMessage = message.Object.ToObject<GroupLeave>();

            var vkPoolService = serviceProvider.GetService<VkPoolService>();

            var subscriber = await VkHelper.CreateSubscriber(context, vkPoolService, message.IdGroup, innerMessage.IdUser);
            if (subscriber == null)
                return;

            subscriber.IsUnsubscribed = true;
            subscriber.IsSubscribedToGroup = false;
            subscriber.DtUnsubscribe = DateTime.UtcNow;

            await context.History_GroupActions.AddAsync(new History_GroupActions()
            {
                ActionType = (int)Models.Database.Common.GroupActionTypes.LeaveGroup,
                IdGroup = message.IdGroup,
                IdSubscriber = subscriber.Id,
                Dt = DateTime.UtcNow
            });

            await context.SaveChangesAsync();
        }

        private static async Task GroupJoin(IServiceProvider serviceProvider, DatabaseContext context, CallbackMessage message)
        {
            var innerMessage = message.Object.ToObject<GroupJoin>();
            if (!innerMessage.IdUser.HasValue || innerMessage.IdUser.Value <= 0)
                return;

            var vkPoolService = serviceProvider.GetService<VkPoolService>();

            var subscriber = await VkHelper.CreateSubscriber(context, vkPoolService, message.IdGroup, innerMessage.IdUser.Value);
            if (subscriber == null)
                return;

            subscriber.IsUnsubscribed = false;
            subscriber.DtUnsubscribe = null;
            subscriber.IsSubscribedToGroup = true;

            await context.History_GroupActions.AddAsync(new History_GroupActions()
            {
                ActionType = (int)Models.Database.Common.GroupActionTypes.JoinGroup,
                IdGroup = message.IdGroup,
                IdSubscriber = subscriber.Id,
                Dt = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
        }

        private static async Task UserBlock(IServiceProvider serviceProvider, DatabaseContext context, CallbackMessage message)
        {
            var innerMessage = message.Object.ToObject<UserBlock>();

            var vkPoolService = serviceProvider.GetService<VkPoolService>();

            var subscriber = await VkHelper.CreateSubscriber(context, vkPoolService, message.IdGroup, innerMessage.IdUser);
            if (subscriber == null)
                return;

            subscriber.IsBlocked = true;

            await context.History_GroupActions.AddAsync(new History_GroupActions()
            {
                ActionType = (int)Models.Database.Common.GroupActionTypes.Blocked,
                IdGroup = message.IdGroup,
                IdSubscriber = subscriber.Id,
                Dt = DateTime.UtcNow
            });

            await context.SaveChangesAsync();
        }

        private static async Task UserUnblock(IServiceProvider serviceProvider, DatabaseContext context, CallbackMessage message)
        {
            var innerMessage = message.Object.ToObject<UserUnblock>();

            var vkPoolService = serviceProvider.GetService<VkPoolService>();

            var subscriber = await VkHelper.CreateSubscriber(context, vkPoolService, message.IdGroup, innerMessage.IdUser);
            if (subscriber == null)
                return;

            subscriber.IsBlocked = false;

            await context.History_GroupActions.AddAsync(new History_GroupActions()
            {
                ActionType = (int)Models.Database.Common.GroupActionTypes.Unblocked,
                IdGroup = message.IdGroup,
                IdSubscriber = subscriber.Id,
                Dt = DateTime.UtcNow
            });

            await context.SaveChangesAsync();
        }

        #endregion

        public VkCallbackScheduler(IServiceScopeFactory serviceScopeFactory) : base()
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
                    Console.WriteLine($"VkCallbackWorkerService started at {DateTime.Now.ToString("HH:mm:ss:ffff")}");
#endif
                    await DoWork(scope.ServiceProvider);
#if DEBUG
                    Console.WriteLine($"VkCallbackWorkerService finished at {DateTime.Now.ToString("HH:mm:ss:ffff")}");
#endif
                }
                await Task.Delay(TimeSpan.FromSeconds(PeriodSeconds), stoppingToken); //5 seconds delay
            }
            while (!stoppingToken.IsCancellationRequested);
        }

        private static async Task<bool> IsPassedCallbackMessage(DatabaseContext _context, CallbackMessage message)
        {
            string json = message.ToJSON();

            var callbackMessages = await _context.VkCallbackMessages.Where(x => x.Type == message.Type && x.IdGroup == message.IdGroup && x.Object == json && x.IsProcessed).ToArrayAsync();
            if (callbackMessages != null && callbackMessages.Any(x => x.Id != message.IdVkCallbackMessage))
            {
                await _context.VkCallbackMessages.Where(x => x.Type == message.Type && x.IdGroup == message.IdGroup && x.Object == json && !x.IsProcessed).ForEachAsync(x => x.IsProcessed = true);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        private static async Task DoWork(IServiceProvider serviceProvider)
        {
            var _configuration = serviceProvider.GetService<IConfiguration>();
            var _context = serviceProvider.GetService<DatabaseContext>();
            var _vkPoolService = serviceProvider.GetService<VkPoolService>();

            var callbackMessages = await _context.VkCallbackMessages
                .Where(x => !x.IsProcessed)
                .Select(x => new CallbackMessage()
                {
                    IdGroup = x.IdGroup,
                    IdVkCallbackMessage = x.Id,
                    Object = CallbackMessage.FromJson(x.Object),
                    Type = x.Type
                }).ToArrayAsync();

            foreach (var message in callbackMessages)
            {
                var function = callbackFunctions.FirstOrDefault(x => x.Item1 == message.Type);
                if (function == null)
                    continue;

                bool hasError = false;
                try
                {
                    await function.Item2.Invoke(serviceProvider, _context, message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"VkCallbackWorkerService exception: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
                    hasError = true;
                }

                var callbackMessage = await _context.VkCallbackMessages.FindAsync(message.IdVkCallbackMessage);
                if (callbackMessage == null)
                    continue;

                callbackMessage.IsProcessed = true;
                callbackMessage.HasError = hasError;
                await _context.SaveChangesAsync();
            }
        }


    }
}
