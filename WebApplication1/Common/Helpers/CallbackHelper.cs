using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using VkConnector.Events;
using VkConnector.Models.Objects;
using WebApplication1.Models.Database;
using WebApplication1.Models.Database.Common;

namespace WebApplication1.Common.Helpers
{
    public static class CallbackHelper
    {
        public static async Task<Subscribers> CreateNewSubscriber(DatabaseContext dbContext, int idGroup, int idVkUser)
        {
            var vkUser = dbContext.VkUsers.FirstOrDefault(x => x.IdVk == idVkUser);
            bool isBlocked = false;
            if (vkUser == null)
            {
                string groupAccessToken = dbContext.Groups.FirstOrDefault(x => x.IdVk == idGroup)?.AccessToken;

                User user = (await VkConnector.Methods.Users.Get(groupAccessToken, idVkUser))?.FirstOrDefault();
                isBlocked = user.IsBlacklisted;

                vkUser = VkUsers.FromUser(user);
                await dbContext.VkUsers.AddAsync(vkUser);
                await dbContext.SaveChangesAsync();
            }

            return new Subscribers()
            {
                IdVkUser = vkUser.IdVk,
                IdGroup = idGroup,
                IsBlocked = isBlocked
            };
        }
        public static async Task NewUserSubscribed(DatabaseContext dbContext, int idGroup, int idVkUser)
        {
            var subscriber = dbContext.Subscribers.FirstOrDefault(x => x.IdGroup == idGroup && x.IdVkUser == idVkUser);
            if (subscriber == null)
            {
                subscriber = await CreateNewSubscriber(dbContext, idGroup, idVkUser);
                await dbContext.Subscribers.AddAsync(subscriber);
            }
            else
            {
                subscriber.IsUnsubscribed = false;
                subscriber.DtUnsubscribe = null;
            }
            subscriber.IsSubscribedToGroup = true;

            await dbContext.SaveChangesAsync();
        }

        public static async Task<Messages> ReplyToMessage(DatabaseContext dbContext, int idGroup, MessageNew message)
        {
            if (!string.IsNullOrWhiteSpace(message.Text))
            {
                var scenario = dbContext.Scenarios
                    .Where(x => x.IsEnabled)
                    .Where(x => x.IdGroup == idGroup)
                    .Include(x => x.Group)
                    .Include(x => x.Group.GroupAdmins)
                    .Where(x => x.Group.GroupAdmins.Any())
                    .Include(x => x.Message)
                    .Include(x => x.ErrorMessage)
                    .FirstOrDefault(x => (x.IsStrictMatch && x.InputMessage.ToLower() == message.Text.ToLower()) || (!x.IsStrictMatch && message.Text.ToLower().Contains(x.InputMessage.ToLower())));

                if (scenario != null)
                    return await ScenarioReply(dbContext, idGroup, message, scenario);
            }
            //Тут бот или картинки

            return null;
        }

        private static async Task AddSubscriberToChain(DatabaseContext dbContext, Guid idSubscriber, Guid idChain)
        {
            var firstChainStepId = dbContext.ChainContents.Where(x => x.IdChain == idChain).OrderBy(x => x.Index).Select(x => x.Id).FirstOrDefault();
            if (firstChainStepId == default(Guid))
                return;

            var subscriberInChain = new SubscribersInChains()
            {
                IdSubscriber = idSubscriber,
                IdChainStep = firstChainStepId,
                DtAdd = DateTime.UtcNow
            };

            await dbContext.SubscribersInChains.AddAsync(subscriberInChain);
            await dbContext.SaveChangesAsync();
        }

        private static async Task<Messages> ScenarioReply(DatabaseContext dbContext, int idGroup, MessageNew message, Scenarios scenario)
        {
            if (scenario.Action == ScenarioActions.Message)
                return scenario.Message;

            var subscriber = dbContext.Subscribers.FirstOrDefault(x => x.IdGroup == idGroup && x.IdVkUser == message.IdUser);

            bool isSubscriberInChain = !scenario.IdChain.HasValue ? false : dbContext.SubscribersInChains
                .Where(x => x.IdSubscriber == subscriber.Id)
                .Include(x => x.ChainStep)
                .Any(x => x.ChainStep.IdChain == scenario.IdChain.Value);

            bool isSubscriberInChain2 = !scenario.IdChain2.HasValue ? false : dbContext.SubscribersInChains
                .Where(x => x.IdSubscriber == subscriber.Id)
                .Include(x => x.ChainStep)
                .Any(x => x.ChainStep.IdChain == scenario.IdChain2.Value);

            switch (scenario.Action)
            {
                case ScenarioActions.AddToChain:
                    {
                        if (isSubscriberInChain)
                            return scenario.ErrorMessage;

                        await AddSubscriberToChain(dbContext, subscriber.Id, scenario.IdChain.Value);

                        return scenario.Message;
                    }
                case ScenarioActions.ChangeChain:
                    {
                        if (isSubscriberInChain2)
                        {
                            var subscriberInChain = dbContext.SubscribersInChains
                                .Include(x => x.ChainStep)
                                .Where(x => x.IdSubscriber == subscriber.Id && x.ChainStep.IdChain == scenario.IdChain2.Value);
                            dbContext.SubscribersInChains.RemoveRange(subscriberInChain);
                            await dbContext.SaveChangesAsync();
                        }

                        if (isSubscriberInChain)
                            return scenario.ErrorMessage;

                        await AddSubscriberToChain(dbContext, subscriber.Id, scenario.IdChain.Value);

                        return scenario.Message;
                    }
                case ScenarioActions.RemoveFromChain:
                    {
                        if (!isSubscriberInChain2)
                            return scenario.ErrorMessage;

                        var subscriberInChain = dbContext.SubscribersInChains
                                .Include(x => x.ChainStep)
                                .Where(x => x.IdSubscriber == subscriber.Id && x.ChainStep.IdChain == scenario.IdChain2.Value);
                        dbContext.SubscribersInChains.RemoveRange(subscriberInChain);
                        await dbContext.SaveChangesAsync();

                        return scenario.Message;
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        public static async Task AddWallPost(DatabaseContext dbContext, int idGroup, WallPostNew newPost)
        {
            if (dbContext.WallPosts.Any(x => x.IdGroup == idGroup && x.IdVk == newPost.Id))
                return;

            var newWallPost = new WallPosts()
            {
                DtAdd = newPost.Dt,
                IdGroup = idGroup,
                IdVk = newPost.Id,
                Text = newPost.Text
            };

            await dbContext.WallPosts.AddAsync(newWallPost);
            await dbContext.SaveChangesAsync();
        }

        public static async Task AddRepost(DatabaseContext dbContext, int idGroup, WallRepost repost)
        {
            var subscriber = dbContext.Subscribers.FirstOrDefault(x => x.IdGroup == idGroup && x.IdVkUser == repost.IdAuthor);
            if (subscriber == null)
            {
                subscriber = await CreateNewSubscriber(dbContext, idGroup, repost.IdAuthor);
                await dbContext.Subscribers.AddAsync(subscriber);
                await dbContext.SaveChangesAsync();
            }
            var repostedPost = repost.Copy_history.FirstOrDefault();

            bool hasSubscriberRepost = dbContext.SubscriberReposts
                .Include(x => x.WallPost)
                .Include(x => x.Subscriber)
                .Any(x => x.WallPost.IdVk == repostedPost.Id && x.Subscriber.IdVkUser == repost.IdAuthor && x.DtRepost == repost.Dt);

            if (hasSubscriberRepost)
                return;

            var post = dbContext.WallPosts.FirstOrDefault(x => x.IdGroup == idGroup && x.IdVk == repostedPost.Id);
            if (post == null)
            {
                post = new WallPosts()
                {
                    DtAdd = repostedPost.Dt,
                    IdGroup = idGroup,
                    IdVk = repostedPost.Id,
                };
                await dbContext.WallPosts.AddAsync(post);
                await dbContext.SaveChangesAsync();
            }

            var subscriberRepost = new SubscriberReposts()
            {
                DtRepost = repost.Dt,
                IdPost = post.Id,
                IdSubscriber = subscriber.Id,
                Text = repost.Text,
            };

            await dbContext.SubscriberReposts.AddAsync(subscriberRepost);
            await dbContext.SaveChangesAsync();
        }
    }
}
