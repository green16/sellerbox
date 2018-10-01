using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VkNet.Enums.Filters;
using SellerBox.Common.Services;
using SellerBox.Models.Database;

namespace SellerBox.Common.Hubs
{
    public class SubscriberSyncHub : Hub
    {
        private readonly DatabaseContext _context;
        private readonly VkPoolService _vkPoolService;

        private static readonly Dictionary<string, Pair<ulong, ulong>> syncTasks = new Dictionary<string, Pair<ulong, ulong>>(); //idGroup,total,position
        private static readonly Dictionary<string, string> connectedToGroups = new Dictionary<string, string>();
        private string DictionaryKey(long idGroup, SyncType syncType) => $"{idGroup}_{(byte)syncType}";

        public SubscriberSyncHub(DatabaseContext context, VkPoolService vkPoolService)
        {
            _context = context;
            _vkPoolService = vkPoolService;
        }

        public async Task<Pair<ulong, ulong>> Subscribe(long idGroup, SyncType syncType)
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

        public async Task UnSubscribe(long idGroup, SyncType syncType)
        {
            string dictionaryKey = DictionaryKey(idGroup, syncType);

            connectedToGroups.Remove(dictionaryKey);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, dictionaryKey);
        }

        public Pair<ulong, ulong> GetState(long idGroup, SyncType syncType)
        {
            string dictionaryKey = DictionaryKey(idGroup, syncType);

            return syncTasks.ContainsKey(dictionaryKey) ? syncTasks[dictionaryKey] : null;
        }

        public async Task StartProcess(long idGroup, SyncType syncType)
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

        private async Task Start(long idGroup, SyncType syncType)
        {
            await _context.SyncHistory.AddAsync(new History_Synchronization()
            {
                DtStart = DateTime.UtcNow,
                IdGroup = idGroup,
                SyncType = syncType
            });
            await _context.SaveChangesAsync();

            string dictionaryKey = DictionaryKey(idGroup, syncType);

            if (!syncTasks.ContainsKey(dictionaryKey))
                syncTasks.Add(dictionaryKey, new Pair<ulong, ulong>(0, 0));
            await Clients.Group(dictionaryKey).SendAsync("ProgressStarted");
        }

        private async Task Progress(long idGroup, SyncType syncType, ulong total, ulong progress)
        {
            string dictionaryKey = DictionaryKey(idGroup, syncType);

            if (!syncTasks.ContainsKey(dictionaryKey))
                syncTasks.Add(dictionaryKey, new Pair<ulong, ulong>(total, progress));
            else
            {
                var item = syncTasks[dictionaryKey];
                item.Item1 = total;
                item.Item2 = progress;
            }
            await Clients.Group(dictionaryKey).SendAsync("ProgressChanged", total, progress);
        }

        private async Task Finish(long idGroup, SyncType syncType)
        {
            var lastTask = await _context.SyncHistory.Where(x => x.IdGroup == idGroup && x.SyncType == syncType).LastAsync();
            lastTask.DtEnd = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            string dictionaryKey = DictionaryKey(idGroup, syncType);

            if (syncTasks.ContainsKey(dictionaryKey))
                syncTasks.Remove(dictionaryKey);
            await Clients.Group(dictionaryKey).SendAsync("ProgressFinished");
        }

        private async Task SyncSubscribers(long idGroup)
        {
            await Start(idGroup, SyncType.Subscribers);

            var vkApi = await _vkPoolService.GetGroupVkApi(idGroup);

            long offset = 0, count = 1000;
            do
            {
                var vkMembers = await vkApi.Groups.GetMembersAsync(new VkNet.Model.RequestParams.GroupsGetMembersParams()
                {
                    GroupId = idGroup.ToString(),
                    Count = count,
                    Offset = offset,
                    Fields = UsersFields.BirthDate | UsersFields.City | UsersFields.Country | UsersFields.Domain | UsersFields.Photo50 | UsersFields.Photo400Orig | UsersFields.Sex | UsersFields.Sex,
                });

                await Progress(idGroup, SyncType.Subscribers, vkMembers.TotalCount, (ulong)offset);

                foreach (var vkMember in vkMembers)
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
                                    IsBlocked = vkMember.Blacklisted,
                                    DtAdd = DateTime.UtcNow
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

                await Progress(idGroup, SyncType.Subscribers, vkMembers.TotalCount, (ulong)offset);
            } while (true);
            await Finish(idGroup, SyncType.Subscribers);
        }

        private async Task SyncChats(long idGroup)
        {
            await Start(idGroup, SyncType.Chats);

            var vkApi = await _vkPoolService.GetGroupVkApi(idGroup);

            ulong offset = 0, count = 100;
            do
            {
                var conversationsInfo = await vkApi.Messages.GetConversationsAsync(new VkNet.Model.RequestParams.GetConversationsParams()
                {
                    Count = count,
                    Offset = offset,
                    Extended = true,
                    Fields = new string[] { "bdate", "city", "country", "domain", "photo_50", "sex", "blacklisted" }
                });// VkConnector.Methods.Messages.GetConversations(groupAccessToken, idGroup, "bdate,city,country,domain,photo_50,sex,blacklisted", offset, count);
                if (conversationsInfo == null)
                {
                    System.Threading.Thread.Sleep(1000);
                    continue;
                }

                await Progress(idGroup, SyncType.Chats, (ulong)conversationsInfo.Count, offset);

                foreach (var vkMember in conversationsInfo.Profiles)
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
                            }
                            else
                                vkUser.Update(vkMember);
                            await _context.SaveChangesAsync();

                            var subscriber = await _context.Subscribers
                                .Where(x => x.IdGroup == idGroup)
                                .FirstOrDefaultAsync(x => x.IdVkUser == vkMember.Id);
                            if (subscriber == null)
                            {
                                subscriber = new Subscribers()
                                {
                                    IdGroup = idGroup,
                                    IdVkUser = vkMember.Id,
                                    IsBlocked = vkMember.Blacklisted,
                                    DtAdd = DateTime.UtcNow

                                };
                                await _context.AddAsync(subscriber);
                            }
                            subscriber.IsChatAllowed = conversationsInfo.Items
                                .Where(x => x.Conversation != null && x.Conversation.Peer != null && x.Conversation.Peer.Id == vkMember.Id)
                                .Select(x => x.Conversation?.CanWrite?.Allowed)
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
                if (offset >= (ulong)conversationsInfo.Count)
                    break;

                await Progress(idGroup, SyncType.Chats, (ulong)conversationsInfo.Count, offset);
            } while (true);
            await Finish(idGroup, SyncType.Chats);
        }
    }
}
