using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models.Database;

namespace WebApplication1.Common.Hubs
{
    public class SubscriberSyncHub : Hub
    {
        private readonly DatabaseContext _context;
        private static readonly Dictionary<string, Pair<int, int>> syncTasks = new Dictionary<string, Pair<int, int>>(); //idGroup,total,position
        private static readonly Dictionary<string, string> connectedToGroups = new Dictionary<string, string>();
        private string DictionaryKey(int idGroup, SyncType syncType) => $"{idGroup}_{(byte)syncType}";

        public SubscriberSyncHub(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<Pair<int, int>> Subscribe(int idGroup, SyncType syncType)
        {
            string dictionaryKey = DictionaryKey(idGroup, syncType);

            connectedToGroups.Add(Context.ConnectionId, dictionaryKey);
            await Groups.AddToGroupAsync(Context.ConnectionId, dictionaryKey);

            return GetState(idGroup, syncType);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (connectedToGroups.TryGetValue(Context.ConnectionId, out string groupName) && !string.IsNullOrWhiteSpace(groupName))
            {
                connectedToGroups.Remove(Context.ConnectionId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task UnSubscribe(int idGroup, SyncType syncType)
        {
            string dictionaryKey = DictionaryKey(idGroup, syncType);

            connectedToGroups.Remove(dictionaryKey);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, dictionaryKey);
        }

        public Pair<int, int> GetState(int idGroup, SyncType syncType)
        {
            string dictionaryKey = DictionaryKey(idGroup, syncType);

            return syncTasks.ContainsKey(dictionaryKey) ? syncTasks[dictionaryKey] : null;
        }

        public async Task StartProcess(int idGroup, SyncType syncType)
        {
            switch (syncType)
            {
                case SyncType.Chats:
                    {
                        await SyncChats(idGroup);
                        break;
                    }
                case SyncType.Subscribers:
                    {
                        await SyncSubscribers(idGroup);
                        break;
                    }
            }
        }

        private async Task Start(int idGroup, SyncType syncType)
        {
            await _context.SyncHistory.AddAsync(new SyncHistory()
            {
                DtStart = DateTime.UtcNow,
                IdGroup = idGroup,
                SyncType = syncType
            });
            await _context.SaveChangesAsync();

            string dictionaryKey = DictionaryKey(idGroup, syncType);

            if (!syncTasks.ContainsKey(dictionaryKey))
                syncTasks.Add(dictionaryKey, new Pair<int, int>(0, 0));
            await Clients.Group(dictionaryKey).SendAsync("ProgressStarted");
        }

        private async Task Progress(int idGroup, SyncType syncType, int total, int progress)
        {
            string dictionaryKey = DictionaryKey(idGroup, syncType);

            if (!syncTasks.ContainsKey(dictionaryKey))
                syncTasks.Add(dictionaryKey, new Pair<int, int>(total, progress));
            else
            {
                var item = syncTasks[dictionaryKey];
                item.Item1 = total;
                item.Item2 = progress;
            }
            await Clients.Group(dictionaryKey).SendAsync("ProgressChanged", total, progress);
        }

        private async Task Finish(int idGroup, SyncType syncType)
        {
            var lastTask = await _context.SyncHistory.LastAsync(x => x.IdGroup == idGroup && x.SyncType == syncType);
            lastTask.DtEnd = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            string dictionaryKey = DictionaryKey(idGroup, syncType);

            if (syncTasks.ContainsKey(dictionaryKey))
                syncTasks.Remove(dictionaryKey);
            await Clients.Group(dictionaryKey).SendAsync("ProgressFinished");
        }

        private async Task SyncSubscribers(int idGroup)
        {
            await Start(idGroup, SyncType.Subscribers);

            string groupAccessToken = _context.Groups.FirstOrDefault(x => x.IdVk == idGroup)?.AccessToken;

            int offset = 0, count = 1000;
            do
            {
                var vkMembers = await VkConnector.Methods.Groups.GetMembers(groupAccessToken, idGroup, "bdate,city,country,domain,photo_50,photo_400_orig,sex,blacklisted", offset, count);

                await Progress(idGroup, SyncType.Subscribers, vkMembers.Count, offset);

                foreach (var vkMember in vkMembers.Items)
                {
                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            var vkUser = await _context.VkUsers.FirstOrDefaultAsync(x => x.IdVk == vkMember.Id);
                            if (vkUser == null)
                            {
                                vkUser = VkUsers.FromUser(vkMember);
                                await _context.VkUsers.AddAsync(vkUser);
                                await _context.SaveChangesAsync();
                            }
                            else
                                vkUser.Update(vkMember);

                            var subscriber = await _context.Subscribers
                                .Where(x => x.IdGroup == idGroup)
                                .FirstOrDefaultAsync(x => x.IdVkUser == vkMember.Id);

                            if (subscriber == null)
                            {
                                subscriber = new Subscribers()
                                {
                                    IdGroup = idGroup,
                                    IdVkUser = vkMember.Id,
                                    IsBlocked = vkMember.IsBlacklisted,
                                };
                                await _context.Subscribers.AddAsync(subscriber);
                            }
                            subscriber.IsSubscribedToGroup = true;

                            await _context.SaveChangesAsync();

                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                        }
                    }
                }

                offset += count;
                if (offset >= vkMembers.Count)
                    break;

                await Progress(idGroup, SyncType.Subscribers, vkMembers.Count, offset);
            } while (true);
            await Finish(idGroup, SyncType.Subscribers);
        }

        private async Task SyncChats(int idGroup)
        {
            await Start(idGroup, SyncType.Chats);

            string groupAccessToken = _context.Groups.FirstOrDefault(x => x.IdVk == idGroup)?.AccessToken;

            int offset = 0, count = 100;
            do
            {
                var conversationsInfo = await VkConnector.Methods.Messages.GetConversations(groupAccessToken, idGroup, "bdate,city,country,domain,photo_50,sex,blacklisted", offset, count);
                if (conversationsInfo == null)
                {
                    System.Threading.Thread.Sleep(1000);
                    continue;
                }

                await Progress(idGroup, SyncType.Chats, conversationsInfo.Count, offset);

                foreach (var vkMember in conversationsInfo.Users)
                {
                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            var vkUser = await _context.VkUsers.FirstOrDefaultAsync(x => x.IdVk == vkMember.Id);
                            if (vkUser == null)
                            {
                                vkUser = VkUsers.FromUser(vkMember);
                                await _context.AddAsync(vkUser);
                                await _context.SaveChangesAsync();
                            }
                            else
                                vkUser.Update(vkMember);

                            var subscriber = await _context.Subscribers
                                .Where(x => x.IdGroup == idGroup)
                                .FirstOrDefaultAsync(x => x.IdVkUser == vkMember.Id);
                            if (subscriber == null)
                            {
                                subscriber = new Subscribers()
                                {
                                    IdGroup = idGroup,
                                    IdVkUser = vkMember.Id,
                                    IsBlocked = vkMember.IsBlacklisted,

                                };
                                await _context.AddAsync(subscriber);
                            }
                            subscriber.IsChatAllowed = conversationsInfo.Items
                                .Where(x => x.Conversation != null && x.Conversation.Peer != null && x.Conversation.Peer.Id == vkMember.Id)
                                .Select(x => x.Conversation?.CanWrite?.IsAllowed)
                                .FirstOrDefault();

                            await _context.SaveChangesAsync();

                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                        }
                    }
                }

                offset += count;
                if (offset >= conversationsInfo.Count)
                    break;

                await Progress(idGroup, SyncType.Chats, conversationsInfo.Count, offset);
            } while (true);
            await Finish(idGroup, SyncType.Chats);
        }
    }
}
