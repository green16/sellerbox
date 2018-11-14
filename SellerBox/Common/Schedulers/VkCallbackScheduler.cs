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
        public const int PeriodSeconds = 5;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public VkCallbackScheduler(IServiceScopeFactory serviceScopeFactory) : base()
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
                        Console.WriteLine($"VkCallbackWorkerService started at {DateTime.Now.ToString("HH:mm:ss:ffff")}");
#endif
                        await DoWork(scope.ServiceProvider);
#if DEBUG
                        Console.WriteLine($"VkCallbackWorkerService finished at {DateTime.Now.ToString("HH:mm:ss:ffff")}");
#endif
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"VkCallbackWorkerService exception: {ex.Message} {Environment.NewLine}{ex.StackTrace}");
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
                switch (message.Type)
                {
                    case "message_typing_state":
                        {
                            break;
                        }
                    case "message_new":
                        {
                            if (await IsPassedCallbackMessage(_context, message))
                                continue;
                            var innerMessage = VkNet.Model.Message.FromJson(new VkNet.Utils.VkResponse(message.Object));

                            if (!innerMessage.UserId.HasValue || innerMessage.UserId.Value <= 0)
                                break;

                            var subscriber = await VkHelper.CreateSubscriber(_context, _vkPoolService, message.IdGroup, innerMessage.UserId.Value);
                            if (subscriber == null)
                                break;

                            await _context.History_Messages.AddAsync(new History_Messages()
                            {
                                Dt = DateTime.UtcNow,
                                IsOutgoingMessage = false,
                                IdSubscriber = subscriber.Id,
                                Text = innerMessage.Body
                            });
                            await _context.SaveChangesAsync();

                            bool isCancelMessaging = innerMessage.Body?.ToLower() == "стоп";
                            if (!isCancelMessaging && (!subscriber.IsChatAllowed.HasValue || !subscriber.IsChatAllowed.Value))
                            {
                                await _context.History_GroupActions.AddAsync(new History_GroupActions()
                                {
                                    ActionType = (int)Models.Database.Common.GroupActionTypes.AcceptMessaging,
                                    IdGroup = message.IdGroup,
                                    IdSubscriber = subscriber.Id,
                                    Dt = DateTime.UtcNow
                                });

                                subscriber.IsChatAllowed = true;
                                await _context.SaveChangesAsync();
                            }

                            var vkApi = await _vkPoolService.GetGroupVkApi(message.IdGroup);

                            if (isCancelMessaging)
                            {
                                await _context.History_GroupActions.AddAsync(new History_GroupActions()
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
                                        await _context.History_Messages.AddAsync(new History_Messages()
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

                                await _context.SaveChangesAsync();
                                break;
                            }

                            if (vkApi == null)
                                break;

                            var replyToMessageResult = await CallbackHelper.ReplyToMessage(_context, message.IdGroup, subscriber.Id, innerMessage);
                            if (replyToMessageResult == null)
                                break;

                            if (replyToMessageResult.Item1.HasValue)
                            {
                                MessageHelper messageHelper = new MessageHelper(_configuration, _context);
                                await messageHelper.SendMessages(vkApi, message.IdGroup, replyToMessageResult.Item1.Value, innerMessage.UserId.Value);
                                await _context.History_Messages.AddAsync(new History_Messages()
                                {
                                    Dt = DateTime.UtcNow,
                                    IsOutgoingMessage = true,
                                    IdSubscriber = subscriber.Id,
                                    IdMessage = replyToMessageResult.Item1
                                }).ContinueWith(result => _context.SaveChanges());
                            }
                            else if (replyToMessageResult.Item2)
                                await vkApi.Messages.MarkAsReadAsync(innerMessage.UserId.ToString(), groupId: message.IdGroup);
                            break;
                        }
                    case "message_reply":
                        {
                            if (await IsPassedCallbackMessage(_context, message))
                                continue;

                            break;
                        }
                    case "message_edit":
                        {
                            if (await IsPassedCallbackMessage(_context, message))
                                continue;

                            break;
                        }
                    case "message_allow":
                        {
                            var innerMessage = VkNet.Model.Message.FromJson(new VkNet.Utils.VkResponse(message.Object));

                            var subscriber = await VkHelper.CreateSubscriber(_context, _vkPoolService, message.IdGroup, innerMessage.UserId.Value);
                            if (subscriber == null)
                                break;

                            if (!subscriber.IsChatAllowed.HasValue || !subscriber.IsChatAllowed.Value)
                                subscriber.IsChatAllowed = true;

                            await _context.History_GroupActions.AddAsync(new History_GroupActions()
                            {
                                ActionType = (int)Models.Database.Common.GroupActionTypes.AcceptMessaging,
                                IdGroup = message.IdGroup,
                                IdSubscriber = subscriber.Id,
                                Dt = DateTime.UtcNow
                            });

                            await _context.SaveChangesAsync();
                            break;
                        }
                    case "message_deny":
                        {
                            var innerMessage = VkNet.Model.Message.FromJson(new VkNet.Utils.VkResponse(message.Object));

                            var subscriber = await VkHelper.CreateSubscriber(_context, _vkPoolService, message.IdGroup, innerMessage.UserId.Value);
                            if (subscriber == null)
                                break;

                            if (!subscriber.IsChatAllowed.HasValue || subscriber.IsChatAllowed.Value)
                                subscriber.IsChatAllowed = false;

                            await _context.History_GroupActions.AddAsync(new History_GroupActions()
                            {
                                ActionType = (int)Models.Database.Common.GroupActionTypes.BlockMessaging,
                                IdGroup = message.IdGroup,
                                IdSubscriber = subscriber.Id,
                                Dt = DateTime.UtcNow
                            });

                            await _context.SaveChangesAsync();
                            break;
                        }
                    case "photo_new":
                        {
                            if (await IsPassedCallbackMessage(_context, message))
                                continue;

                            break;
                        }
                    case "photo_comment_new":
                        {
                            if (await IsPassedCallbackMessage(_context, message))
                                continue;

                            break;
                        }
                    case "photo_comment_edit":
                        {
                            if (await IsPassedCallbackMessage(_context, message))
                                continue;

                            break;
                        }
                    case "photo_comment_delete":
                        {
                            break;
                        }
                    case "photo_comment_restore":
                        {
                            if (await IsPassedCallbackMessage(_context, message))
                                continue;

                            break;
                        }
                    case "audio_new":
                        {
                            if (await IsPassedCallbackMessage(_context, message))
                                continue;

                            break;
                        }
                    case "video_new":
                        {
                            if (await IsPassedCallbackMessage(_context, message))
                                continue;

                            break;
                        }
                    case "video_comment_new":
                        {
                            if (await IsPassedCallbackMessage(_context, message))
                                continue;

                            break;
                        }
                    case "video_comment_edit":
                        {
                            if (await IsPassedCallbackMessage(_context, message))
                                continue;

                            break;
                        }
                    case "video_comment_delete":
                        {
                            break;
                        }
                    case "video_comment_restore":
                        {
                            if (await IsPassedCallbackMessage(_context, message))
                                continue;

                            break;
                        }
                    case "wall_post_new":
                        {
                            if (await IsPassedCallbackMessage(_context, message))
                                continue;

                            var newPost = VkNet.Model.Post.FromJson(new VkNet.Utils.VkResponse(message.Object));
                            if (!newPost.Id.HasValue || await _context.WallPosts.AnyAsync(x => x.IdGroup == message.IdGroup && x.IdVk == newPost.Id))
                                break;

                            var newWallPost = new WallPosts()
                            {
                                DtAdd = newPost.Date ?? DateTime.UtcNow,
                                IdGroup = message.IdGroup,
                                IdVk = newPost.Id.Value,
                                Text = newPost.Text
                            };

                            await _context.WallPosts.AddAsync(newWallPost);
                            await _context.SaveChangesAsync();

                            var subscriber = await VkHelper.CreateSubscriber(_context, _vkPoolService, message.IdGroup, newPost.FromId.Value);
                            if (subscriber == null)
                                break;

                            await _context.History_WallPosts.AddAsync(new History_WallPosts()
                            {
                                Dt = DateTime.UtcNow,
                                IdPost = newWallPost.Id,
                                IdSubscriber = subscriber.Id,
                                IsRepost = false
                            });
                            await _context.SaveChangesAsync();

                            break;
                        }
                    case "wall_repost":
                        {
                            if (await IsPassedCallbackMessage(_context, message))
                                continue;

                            var repost = VkNet.Model.Wall.FromJson(new VkNet.Utils.VkResponse(message.Object));
                            if (!repost.FromId.HasValue || repost.FromId.Value <= 0)
                                break;

                            var subscriber = await VkHelper.CreateSubscriber(_context, _vkPoolService, message.IdGroup, repost.FromId.Value);
                            if (subscriber == null)
                                break;

                            var repostedPost = repost.CopyHistory.FirstOrDefault();

                            var post = await _context.WallPosts.FirstOrDefaultAsync(x => x.IdGroup == message.IdGroup && x.IdVk == repostedPost.Id);
                            if (post == null)
                            {
                                post = new WallPosts()
                                {
                                    DtAdd = repostedPost.Date ?? DateTime.UtcNow,
                                    IdGroup = message.IdGroup,
                                    IdVk = repostedPost.Id.Value,
                                };
                                await _context.WallPosts.AddAsync(post);
                                await _context.SaveChangesAsync();
                            }

                            bool hasSubscriberRepost = await _context.SubscriberReposts
                                .Include(x => x.WallPost)
                                .Include(x => x.Subscriber)
                                .AnyAsync(x => x.WallPost.IdVk == repostedPost.Id && x.Subscriber.IdVkUser == repost.FromId && x.DtRepost == repost.Date);

                            if (!hasSubscriberRepost)
                                await _context.SubscriberReposts.AddAsync(new SubscriberReposts()
                                {
                                    DtRepost = repost.Date ?? DateTime.UtcNow,
                                    IdPost = post.Id,
                                    IdSubscriber = subscriber.Id,
                                    Text = repost.Text,
                                });

                            await _context.History_WallPosts.AddAsync(new History_WallPosts()
                            {
                                Dt = DateTime.UtcNow,
                                IdPost = post.Id,
                                IdSubscriber = subscriber.Id,
                                IsRepost = true
                            });

                            await _context.SaveChangesAsync();

                            break;
                        }
                    case "wall_reply_new":
                        {
                            if (await IsPassedCallbackMessage(_context, message))
                                continue;

                            break;
                        }
                    case "wall_reply_edit":
                        {
                            if (await IsPassedCallbackMessage(_context, message))
                                continue;

                            break;
                        }
                    case "wall_reply_delete":
                        {
                            break;
                        }
                    case "wall_reply_restore":
                        {
                            if (await IsPassedCallbackMessage(_context, message))
                                continue;

                            break;
                        }
                    case "board_post_new":
                        {
                            if (await IsPassedCallbackMessage(_context, message))
                                continue;

                            break;
                        }
                    case "board_post_edit":
                        {
                            if (await IsPassedCallbackMessage(_context, message))
                                continue;

                            break;
                        }
                    case "board_post_delete":
                        {
                            break;
                        }
                    case "board_post_restore":
                        {
                            if (await IsPassedCallbackMessage(_context, message))
                                continue;

                            break;
                        }
                    case "market_comment_new":
                        {
                            if (await IsPassedCallbackMessage(_context, message))
                                continue;

                            break;
                        }
                    case "market_comment_edit":
                        {
                            if (await IsPassedCallbackMessage(_context, message))
                                continue;

                            break;
                        }
                    case "market_comment_delete":
                        {
                            break;
                        }
                    case "market_comment_restore":
                        {
                            if (await IsPassedCallbackMessage(_context, message))
                                continue;

                            break;
                        }
                    case "group_leave":
                        {
                            var innerMessage = message.Object.ToObject<GroupLeave>();

                            var subscriber = await VkHelper.CreateSubscriber(_context, _vkPoolService, message.IdGroup, innerMessage.IdUser);
                            if (subscriber == null)
                                break;

                            subscriber.IsUnsubscribed = true;
                            subscriber.IsSubscribedToGroup = false;
                            subscriber.DtUnsubscribe = DateTime.UtcNow;

                            await _context.History_GroupActions.AddAsync(new History_GroupActions()
                            {
                                ActionType = (int)Models.Database.Common.GroupActionTypes.LeaveGroup,
                                IdGroup = message.IdGroup,
                                IdSubscriber = subscriber.Id,
                                Dt = DateTime.UtcNow
                            });

                            await _context.SaveChangesAsync();
                            break;
                        }
                    case "group_join":
                        {
                            var innerMessage = message.Object.ToObject<GroupJoin>();
                            if (!innerMessage.IdUser.HasValue || innerMessage.IdUser.Value <= 0)
                                break;

                            var subscriber = await VkHelper.CreateSubscriber(_context, _vkPoolService, message.IdGroup, innerMessage.IdUser.Value);
                            if (subscriber == null)
                                break;

                            subscriber.IsUnsubscribed = false;
                            subscriber.DtUnsubscribe = null;
                            subscriber.IsSubscribedToGroup = true;

                            await _context.History_GroupActions.AddAsync(new History_GroupActions()
                            {
                                ActionType = (int)Models.Database.Common.GroupActionTypes.JoinGroup,
                                IdGroup = message.IdGroup,
                                IdSubscriber = subscriber.Id,
                                Dt = DateTime.UtcNow
                            });
                            await _context.SaveChangesAsync();
                            break;
                        }
                    case "user_block":
                        {
                            var innerMessage = message.Object.ToObject<UserBlock>();

                            var subscriber = await VkHelper.CreateSubscriber(_context, _vkPoolService, message.IdGroup, innerMessage.IdUser);
                            if (subscriber == null)
                                break;

                            subscriber.IsBlocked = true;

                            await _context.History_GroupActions.AddAsync(new History_GroupActions()
                            {
                                ActionType = (int)Models.Database.Common.GroupActionTypes.Blocked,
                                IdGroup = message.IdGroup,
                                IdSubscriber = subscriber.Id,
                                Dt = DateTime.UtcNow
                            });

                            await _context.SaveChangesAsync();
                            break;
                        }
                    case "user_unblock":
                        {
                            var innerMessage = message.Object.ToObject<UserUnblock>();

                            var subscriber = await VkHelper.CreateSubscriber(_context, _vkPoolService, message.IdGroup, innerMessage.IdUser);
                            if (subscriber == null)
                                break;

                            subscriber.IsBlocked = false;

                            await _context.History_GroupActions.AddAsync(new History_GroupActions()
                            {
                                ActionType = (int)Models.Database.Common.GroupActionTypes.Unblocked,
                                IdGroup = message.IdGroup,
                                IdSubscriber = subscriber.Id,
                                Dt = DateTime.UtcNow
                            });

                            await _context.SaveChangesAsync();
                            break;
                        }
                    case "poll_vote_new":
                        {
                            break;
                        }
                    case "group_officers_edit":
                        {
                            break;
                        }
                    case "group_change_settings":
                        {
                            break;
                        }
                    case "group_change_photo":
                        {
                            break;
                        }
                    case "lead_forms_new":
                        {
                            break;
                        }
                    case "vkpay_transaction":
                        {
                            if (await IsPassedCallbackMessage(_context, message))
                                continue;

                            break;
                        }
                }

                var callbackMessage = await _context.VkCallbackMessages.FirstAsync(x => x.Id == message.IdVkCallbackMessage);
                callbackMessage.IsProcessed = true;
                await _context.SaveChangesAsync();
            }
        }


    }
}
