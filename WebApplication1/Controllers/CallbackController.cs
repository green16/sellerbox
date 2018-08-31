using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using VkConnector.Events;
using VkConnector.Models.Common;
using WebApplication1.Common;
using WebApplication1.Common.Helpers;

namespace WebApplication1.Controllers
{
    public class CallbackController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly IConfiguration _configuration;

        public CallbackController(DatabaseContext context, IConfiguration Configuration)
        {
            _context = context;
            _configuration = Configuration;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Vk([FromBody]CallbackMessage message)
        {
            if (await _context.GroupAdmins.AllAsync(x => x.IdGroup != message.IdGroup))
                return Content("Ok");

            if (message == null)
                throw new Exception("deserialize error");

            string json = message.Object?.ToString(Newtonsoft.Json.Formatting.None);

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

                        MessageNew innerMessage = message.Object.ToObject<MessageNew>();

                        if (innerMessage.IdUser <= 0)
                            break;

                        string accessToken = await _context.Groups.Where(x => x.IdVk == message.IdGroup).Select(x => x.AccessToken).FirstOrDefaultAsync();

                        var subscriber = _context.Subscribers.FirstOrDefault(x => x.IdGroup == message.IdGroup && x.IdVkUser == innerMessage.IdUser);
                        if (subscriber == null)
                        {
                            subscriber = await CallbackHelper.CreateNewSubscriber(_context, message.IdGroup, innerMessage.IdUser);
                            await _context.Subscribers.AddAsync(subscriber);
                            await _context.SaveChangesAsync();
                        }
                        if (innerMessage.Text.ToLower() == "стоп")
                        {
                            if (subscriber.IsChatAllowed.HasValue && !subscriber.IsChatAllowed.Value)
                                break;
                            subscriber.IsChatAllowed = false;
                            await _context.SaveChangesAsync();
                            await VkConnector.Methods.Messages.Send(accessToken, "Вы успешно отписаны от сообщений группы", null, null, new int[] { innerMessage.IdUser });
                            break;
                        }
                        else if (!subscriber.IsChatAllowed.HasValue || subscriber.IsChatAllowed == false)
                        {
                            subscriber.IsChatAllowed = true;
                            await _context.SaveChangesAsync();
                        }

                        var idAnswerMessage = await CallbackHelper.ReplyToMessage(_context, message.IdGroup, innerMessage);
                        bool markAsRead = idAnswerMessage.HasValue;

                        if (idAnswerMessage.HasValue)
                        {
                            MessageHelper messageHelper = new MessageHelper(_context);
                            await messageHelper.SendMessages(message.IdGroup, idAnswerMessage.Value, innerMessage.IdUser);
                        }
                        else if (markAsRead)
                            await VkConnector.Methods.Messages.MarkAsRead(accessToken, message.IdGroup, innerMessage.IdUser);
                        break;
                    }
                case "message_reply":
                    {
                        MessageReply innerMessage = message.Object.ToObject<MessageReply>();
                        break;
                    }
                case "message_edit":
                    {
                        MessageEdit innerMessage = message.Object.ToObject<MessageEdit>();
                        break;
                    }
                case "message_allow":
                    {
                        MessageAllow innerMessage = message.Object.ToObject<MessageAllow>();

                        var subscriber = _context.Subscribers.FirstOrDefault(x => x.IdGroup == message.IdGroup && x.IdVkUser == innerMessage.IdUser);
                        if (subscriber != null && (!subscriber.IsChatAllowed.HasValue || !subscriber.IsChatAllowed.Value))
                        {
                            subscriber.IsChatAllowed = true;
                            await _context.SaveChangesAsync();
                        }
                        break;
                    }
                case "message_deny":
                    {
                        MessageDeny innerMessage = message.Object.ToObject<MessageDeny>();

                        var subscriber = _context.Subscribers.FirstOrDefault(x => x.IdGroup == message.IdGroup && x.IdVkUser == innerMessage.IdUser);
                        if (subscriber != null && (!subscriber.IsChatAllowed.HasValue || (subscriber.IsChatAllowed.HasValue && subscriber.IsChatAllowed.Value)))
                        {
                            subscriber.IsChatAllowed = false;
                            await _context.SaveChangesAsync();
                        }
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
                        WallPostNew newPost = message.Object.ToObject<WallPostNew>();
                        await CallbackHelper.AddWallPost(_context, message.IdGroup, newPost);
                        break;
                    }
                case "wall_repost":
                    {
                        WallRepost repost = message.Object.ToObject<WallRepost>();
                        if (repost.IdAuthor <= 0)
                            break;
                        await CallbackHelper.AddRepost(_context, message.IdGroup, repost);
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
                case "market_comment_restore":
                    {
                        break;
                    }
                case "market_comment_delete":
                    {
                        break;
                    }
                case "group_leave":
                    {
                        GroupLeave innerMessage = message.Object.ToObject<GroupLeave>();

                        var subscriber = _context.Subscribers.FirstOrDefault(x => x.IdGroup == message.IdGroup && x.IdVkUser == innerMessage.IdUser);
                        if (subscriber != null)
                        {
                            subscriber.IsUnsubscribed = true;
                            subscriber.IsSubscribedToGroup = false;
                            subscriber.DtUnsubscribe = DateTime.UtcNow;
                            await _context.SaveChangesAsync();
                        }
                        break;
                    }
                case "group_join":
                    {
                        GroupJoin innerMessage = message.Object.ToObject<GroupJoin>();
                        await CallbackHelper.NewUserSubscribed(_context, message.IdGroup, innerMessage.IdUser);
                        break;
                    }
                case "user_block":
                    {
                        UserBlock userBlock = message.Object.ToObject<UserBlock>();

                        var subscriber = _context.Subscribers.FirstOrDefault(x => x.IdVkUser == userBlock.IdUser && x.IdGroup == message.IdGroup);
                        if (subscriber != null)
                        {
                            subscriber.IsBlocked = true;
                            await _context.SaveChangesAsync();
                        }
                        break;
                    }
                case "user_unblock":
                    {
                        UserUnblock userUnBlock = message.Object.ToObject<UserUnblock>();

                        var subscriber = _context.Subscribers.FirstOrDefault(x => x.IdVkUser == userUnBlock.IdUser && x.IdGroup == message.IdGroup);
                        if (subscriber != null)
                        {
                            subscriber.IsBlocked = false;
                            await _context.SaveChangesAsync();
                        }
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
                default:
                    throw new NotImplementedException($"type = {message.Type} not implemented");
            }

            await _context.VkCallbackMessages.AddAsync(new Models.Database.VkCallbackMessages()
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