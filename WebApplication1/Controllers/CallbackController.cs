using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Common;
using WebApplication1.Common.Helpers;
using WebApplication1.Common.Services;
using WebApplication1.Models.Database;

namespace WebApplication1.Controllers
{
    public class CallbackController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly VkPoolService _vkPoolService;

        public CallbackController(DatabaseContext context, VkPoolService vkPoolService)
        {
            _context = context;
            _vkPoolService = vkPoolService;
        }

        private async Task<Subscribers> CreateSubscriber(long idGroup, long idVkUser)
        {
            var subscriber = await _context.Subscribers.FirstOrDefaultAsync(x => x.IdGroup == idGroup && x.IdVkUser == idVkUser);
            if (subscriber != null)
                return subscriber;

            var vkUser = await _context.VkUsers.FirstOrDefaultAsync(x => x.IdVk == idVkUser);
            if (vkUser == null)
            {
                var vkApi = await _vkPoolService.GetGroupVkApi(idGroup);

                var user = (await vkApi.Users.GetAsync(new long[] { idVkUser }))?.FirstOrDefault();
                if (user == null)
                    return null;

                vkUser = VkUsers.FromUser(user);
                await _context.VkUsers.AddAsync(vkUser);
                await _context.SaveChangesAsync();
            }

            subscriber = new Subscribers()
            {
                IdVkUser = vkUser.IdVk,
                IdGroup = idGroup,
                DtAdd = DateTime.UtcNow
            };
            await _context.Subscribers.AddAsync(subscriber);
            await _context.SaveChangesAsync();

            return subscriber;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Vk([FromBody]Models.Vk.CallbackMessage message)
        {
            if (await _context.GroupAdmins.AllAsync(x => x.IdGroup != message.IdGroup))
                return Content("Ok");

            if (message == null)
                throw new Exception("deserialize error");

            string json = message.Object?.ToString(Formatting.None);

            switch (message.Type)
            {
                case "confirmation":
                    {
                        string callbackConfirmationCode = await _context.Groups
                            .Where(x => x.IdVk == message.IdGroup)
                            .Select(x => x.CallbackConfirmationCode)
                            .FirstOrDefaultAsync();

                        return Ok(callbackConfirmationCode);
                    }
                case "message_typing_state":
                    {
                        break;
                    }
                case "message_new":
                    {
                        if (_context.VkCallbackMessages.Any(x => x.Type == message.Type && x.IdGroup == message.IdGroup && x.Object == json))
                            return Content("ok");

                        var innerMessage = VkNet.Model.Message.FromJson(new VkNet.Utils.VkResponse(message.Object));

                        if (!innerMessage.UserId.HasValue || innerMessage.UserId.Value <= 0)
                            break;

                        var subscriber = await CreateSubscriber(message.IdGroup, innerMessage.UserId.Value);
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

                        var vkApi = await _vkPoolService.GetGroupVkApi(message.IdGroup);

                        if (innerMessage.Body?.ToLower() == "стоп")
                        {
                            await _context.History_GroupActions.AddAsync(new History_GroupActions()
                            {
                                ActionType = Models.Database.Common.GroupActionTypes.CancelMessaging,
                                IdGroup = message.IdGroup,
                                IdSubscriber = subscriber.Id,
                                Dt = DateTime.UtcNow
                            });
                            if (!subscriber.IsChatAllowed.HasValue || subscriber.IsChatAllowed.Value)
                            {
                                subscriber.IsChatAllowed = false;

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

                            await _context.SaveChangesAsync();
                            break;
                        }
                        else if (!subscriber.IsChatAllowed.HasValue || subscriber.IsChatAllowed == false)
                        {
                            await _context.History_GroupActions.AddAsync(new History_GroupActions()
                            {
                                ActionType = Models.Database.Common.GroupActionTypes.AcceptMessaging,
                                IdGroup = message.IdGroup,
                                IdSubscriber = subscriber.Id,
                                Dt = DateTime.UtcNow
                            });

                            subscriber.IsChatAllowed = true;
                            await _context.SaveChangesAsync();
                        }

                        var idAnswerMessage = await CallbackHelper.ReplyToMessage(_context, message.IdGroup, subscriber.Id, innerMessage);
                        bool markAsRead = !idAnswerMessage.HasValue;

                        if (idAnswerMessage.HasValue)
                        {
                            MessageHelper messageHelper = new MessageHelper(_context);
                            await messageHelper.SendMessages(vkApi, message.IdGroup, idAnswerMessage.Value, innerMessage.UserId.Value);
                            await _context.History_Messages.AddAsync(new History_Messages()
                            {
                                Dt = DateTime.UtcNow,
                                IsOutgoingMessage = true,
                                IdSubscriber = subscriber.Id,
                                IdMessage = idAnswerMessage
                            }).ContinueWith(result => _context.SaveChanges());
                        }
                        else if (markAsRead)
                            await vkApi.Messages.MarkAsReadAsync(innerMessage.UserId.ToString(), groupId: message.IdGroup);
                        break;
                    }
                case "message_reply":
                    {
                        break;
                    }
                case "message_edit":
                    {
                        break;
                    }
                case "message_allow":
                    {
                        var innerMessage = VkNet.Model.Message.FromJson(new VkNet.Utils.VkResponse(message.Object));

                        var subscriber = await CreateSubscriber(message.IdGroup, innerMessage.FromId.Value);
                        if (subscriber == null)
                            break;

                        if (!subscriber.IsChatAllowed.HasValue || !subscriber.IsChatAllowed.Value)
                            subscriber.IsChatAllowed = true;

                        await _context.History_GroupActions.AddAsync(new History_GroupActions()
                        {
                            ActionType = Models.Database.Common.GroupActionTypes.AcceptMessaging,
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

                        var subscriber = await CreateSubscriber(message.IdGroup, innerMessage.FromId.Value);
                        if (subscriber == null)
                            break;

                        if (!subscriber.IsChatAllowed.HasValue || (subscriber.IsChatAllowed.HasValue && subscriber.IsChatAllowed.Value))
                            subscriber.IsChatAllowed = false;

                        await _context.History_GroupActions.AddAsync(new History_GroupActions()
                        {
                            ActionType = Models.Database.Common.GroupActionTypes.BlockMessaging,
                            IdGroup = message.IdGroup,
                            IdSubscriber = subscriber.Id,
                            Dt = DateTime.UtcNow
                        });

                        await _context.SaveChangesAsync();
                        break;
                    }
                case "photo_new":
                    {
                        break;
                    }
                case "photo_comment_new":
                    {
                        break;
                    }
                case "photo_comment_edit":
                    {
                        break;
                    }
                case "photo_comment_delete":
                    {
                        break;
                    }
                case "photo_comment_restore":
                    {
                        break;
                    }
                case "audio_new":
                    {
                        break;
                    }
                case "video_new":
                    {
                        break;
                    }
                case "video_comment_new":
                    {
                        break;
                    }
                case "video_comment_edit":
                    {
                        break;
                    }
                case "video_comment_delete":
                    {
                        break;
                    }
                case "video_comment_restore":
                    {
                        break;
                    }
                case "wall_post_new":
                    {
                        var newPost = VkNet.Model.Wall.FromJson(new VkNet.Utils.VkResponse(message.Object));
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

                        var subscriber = await CreateSubscriber(message.IdGroup, newPost.FromId.Value);
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
                        var repost = VkNet.Model.Wall.FromJson(new VkNet.Utils.VkResponse(message.Object));
                        if (!repost.FromId.HasValue || repost.FromId.Value <= 0)
                            break;

                        var subscriber = await CreateSubscriber(message.IdGroup, repost.FromId.Value);
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
                        break;
                    }
                case "wall_reply_edit":
                    {
                        break;
                    }
                case "wall_reply_delete":
                    {
                        break;
                    }
                case "wall_reply_restore":
                    {
                        break;
                    }
                case "board_post_new":
                    {
                        break;
                    }
                case "board_post_edit":
                    {
                        break;
                    }
                case "board_post_delete":
                    {
                        break;
                    }
                case "board_post_restore":
                    {
                        break;
                    }
                case "market_comment_new":
                    {
                        break;
                    }
                case "market_comment_edit":
                    {
                        break;
                    }
                case "market_comment_delete":
                    {
                        break;
                    }
                case "market_comment_restore":
                    {
                        break;
                    }
                case "group_leave":
                    {
                        var innerMessage = message.Object.ToObject<Models.Vk.GroupLeave>();

                        var subscriber = await CreateSubscriber(message.IdGroup, innerMessage.IdUser);
                        if (subscriber == null)
                            break;

                        subscriber.IsUnsubscribed = true;
                        subscriber.IsSubscribedToGroup = false;
                        subscriber.DtUnsubscribe = DateTime.UtcNow;

                        await _context.History_GroupActions.AddAsync(new History_GroupActions()
                        {
                            ActionType = Models.Database.Common.GroupActionTypes.LeaveGroup,
                            IdGroup = message.IdGroup,
                            IdSubscriber = subscriber.Id,
                            Dt = DateTime.UtcNow
                        });

                        await _context.SaveChangesAsync();
                        break;
                    }
                case "group_join":
                    {
                        var innerMessage = message.Object.ToObject<Models.Vk.GroupJoin>();
                        if (!innerMessage.IdUser.HasValue || innerMessage.IdUser.Value <= 0)
                            break;

                        var subscriber = await CreateSubscriber(message.IdGroup, innerMessage.IdUser.Value);
                        if (subscriber == null)
                            break;

                        subscriber.IsUnsubscribed = false;
                        subscriber.DtUnsubscribe = null;
                        subscriber.IsSubscribedToGroup = true;

                        await _context.History_GroupActions.AddAsync(new History_GroupActions()
                        {
                            ActionType = Models.Database.Common.GroupActionTypes.JoinGroup,
                            IdGroup = message.IdGroup,
                            IdSubscriber = subscriber.Id,
                            Dt = DateTime.UtcNow
                        });
                        await _context.SaveChangesAsync();
                        break;
                    }
                case "user_block":
                    {
                        var innerMessage = message.Object.ToObject<Models.Vk.UserBlock>();

                        var subscriber = await CreateSubscriber(message.IdGroup, innerMessage.IdUser);
                        if (subscriber == null)
                            break;

                        subscriber.IsBlocked = true;

                        await _context.History_GroupActions.AddAsync(new History_GroupActions()
                        {
                            ActionType = Models.Database.Common.GroupActionTypes.Blocked,
                            IdGroup = message.IdGroup,
                            IdSubscriber = subscriber.Id,
                            Dt = DateTime.UtcNow
                        });

                        await _context.SaveChangesAsync();
                        break;
                    }
                case "user_unblock":
                    {
                        var innerMessage = message.Object.ToObject<Models.Vk.UserUnblock>();

                        var subscriber = await CreateSubscriber(message.IdGroup, innerMessage.IdUser);
                        if (subscriber == null)
                            break;

                        subscriber.IsBlocked = false;

                        await _context.History_GroupActions.AddAsync(new History_GroupActions()
                        {
                            ActionType = Models.Database.Common.GroupActionTypes.Unblocked,
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
                        break;
                    }
                default:
                    throw new NotImplementedException($"type = {message.Type} not implemented");
            }

            await _context.VkCallbackMessages.AddAsync(new VkCallbackMessages()
            {
                Dt = DateTime.UtcNow,
                Type = message.Type,
                IdGroup = message.IdGroup,
                Object = json
            });
            await _context.SaveChangesAsync();

            return Content("ok");
        }
    }
}