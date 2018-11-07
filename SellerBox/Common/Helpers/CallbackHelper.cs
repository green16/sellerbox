using Microsoft.EntityFrameworkCore;
using SellerBox.Models.Database;
using SellerBox.Models.Database.Common;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SellerBox.Common.Helpers
{
    public static class CallbackHelper
    {
        public static async Task<Tuple<Guid?, bool>> ReplyToMessage(DatabaseContext dbContext, long idGroup, Guid idSubscriber, VkNet.Model.Message message)
        {
            if (string.IsNullOrWhiteSpace(message.Body))
                return null;

            var isChatAnswer = await dbContext.SubscribersInChatProgress
                .AnyAsync(x => x.IdSubscriber == idSubscriber);

            if (isChatAnswer)
                return new Tuple<Guid?, bool>(await ChatScenarioContentReply(dbContext, idGroup, idSubscriber, message), true);

            var scenario = await dbContext.Scenarios
                .Where(x => x.IsEnabled)
                .Where(x => x.IdGroup == idGroup)
                .Where(x => x.Group.GroupAdmins.Any())
                .FirstOrDefaultAsync(x => (x.IsStrictMatch && x.InputMessage.ToLower() == message.Body.ToLower()) || (!x.IsStrictMatch && message.Body.ToLower().Contains(x.InputMessage.ToLower())));

            if (scenario != null)
                return new Tuple<Guid?, bool>(await ScenarioReply(dbContext, idGroup, idSubscriber, scenario), true);

            var chatScenario = await dbContext.ChatScenarios
                .Where(x => x.IsEnabled)
                .Where(x => x.IdGroup == idGroup)
                .Where(x => x.Group.GroupAdmins.Any())
                .FirstOrDefaultAsync(x => x.InputMessage.ToLower() == message.Body.ToLower());

            if (chatScenario != null)
                return new Tuple<Guid?, bool>(await ChatScenarioReply(dbContext, idGroup, idSubscriber, chatScenario), true);
            //Тут бот или картинки

            return new Tuple<Guid?, bool>(null, false);
        }
        
        private static async Task<Guid?> ScenarioReply(DatabaseContext dbContext, long idGroup, Guid idSubscriber, Scenarios scenario)
        {
            await dbContext.History_Scenarios.AddAsync(new History_Scenarios()
            {
                IdScenario = scenario.Id,
                IdSubscriber = idSubscriber,
                Dt = DateTime.UtcNow
            });
            await dbContext.SaveChangesAsync();

            Services.NotifierService.AddNotifyEvent(new Services.NotifierService.NotifyEvent()
            {
                Dt = DateTime.UtcNow,
                IdGroup = idGroup,
                IdElement = scenario.Id,
                IdSubscriber = idSubscriber,
                SourceType = 0
            });

            if (scenario.Action == ScenarioActions.Message)
                return scenario.IdMessage;

            bool isSubscriberInChain = !scenario.IdChain.HasValue ? false : dbContext.SubscribersInChains
                .Where(x => x.IdSubscriber == idSubscriber)
                .Include(x => x.ChainStep)
                .Any(x => x.ChainStep.IdChain == scenario.IdChain.Value);

            bool isSubscriberInChain2 = !scenario.IdChain2.HasValue ? false : dbContext.SubscribersInChains
                .Where(x => x.IdSubscriber == idSubscriber)
                .Include(x => x.ChainStep)
                .Any(x => x.ChainStep.IdChain == scenario.IdChain2.Value);

            switch (scenario.Action)
            {
                case ScenarioActions.AddToChain:
                    {
                        if (isSubscriberInChain)
                            return scenario.IdErrorMessage;

                        await SubscriberHelper.AddSubscriberToChain(dbContext, idGroup, idSubscriber, scenario.IdChain.Value);
                        break;
                    }
                case ScenarioActions.ChangeChain:
                    {
                        if (isSubscriberInChain2)
                        {
                            var subscriberInChain = dbContext.SubscribersInChains
                                .Include(x => x.ChainStep)
                                .Where(x => x.IdSubscriber == idSubscriber && x.ChainStep.IdChain == scenario.IdChain2.Value);
                            dbContext.SubscribersInChains.RemoveRange(subscriberInChain);
                            await dbContext.SaveChangesAsync();
                        }

                        if (isSubscriberInChain)
                            return scenario.IdErrorMessage;

                        await SubscriberHelper.AddSubscriberToChain(dbContext, idGroup, idSubscriber, scenario.IdChain.Value);
                        break;
                    }
                case ScenarioActions.RemoveFromChain:
                    {
                        if (!isSubscriberInChain2)
                            return scenario.IdErrorMessage;

                        var subscriberInChain = dbContext.SubscribersInChains
                                .Include(x => x.ChainStep)
                                .Where(x => x.IdSubscriber == idSubscriber && x.ChainStep.IdChain == scenario.IdChain2.Value);
                        dbContext.SubscribersInChains.RemoveRange(subscriberInChain);
                        await dbContext.SaveChangesAsync();

                        break;
                    }
                default:
                    throw new NotImplementedException();
            }

            return null;
        }

        private static async Task<Guid?> ChatScenarioContentReply(DatabaseContext dbContext, long idGroup, Guid idSubscriber, VkNet.Model.Message message)
        {
            Guid? result = null;

            var subscribersInChatProgress = await dbContext.SubscribersInChatProgress
                .Where(x => x.IdSubscriber == idSubscriber)
                .Include(x => x.ChatScenarioContent)
                .OrderBy(x => x.DtAdd)
                .LastOrDefaultAsync();

            await dbContext.History_SubscribersInChatScenariosContents.AddAsync(new History_SubscribersInChatScenariosContents()
            {
                IdChatScenarioContent = subscribersInChatProgress.IdChatScenarioContent,
                IdSubscriber = idSubscriber,
                Dt = DateTime.UtcNow
            });

            var idChatScenario = subscribersInChatProgress.ChatScenarioContent.IdChatScenario;

            var chatScenarioContents = await dbContext.ChatScenarioContents
                .Where(x => x.IdChatScenario == idChatScenario)
                .OrderBy(x => x.Step)
                .ToArrayAsync();

            await dbContext.SubscriberChatReplies.AddAsync(new SubscriberChatReplies()
            {
                IdChatScenarioContent = subscribersInChatProgress.IdChatScenarioContent,
                IdSubscriber = idSubscriber,
                Text = message.Body,
                Dt = DateTime.UtcNow,
                UniqueId = subscribersInChatProgress.UniqueId
            });
            await dbContext.SaveChangesAsync();

            var nextChatScenarioContent = chatScenarioContents.SkipWhile(x => x.Id != subscribersInChatProgress.IdChatScenarioContent).Skip(1).FirstOrDefault();
            if (nextChatScenarioContent != null)
            {
                subscribersInChatProgress.IdChatScenarioContent = nextChatScenarioContent.Id;
                result = nextChatScenarioContent.IdMessage;
            }
            else
            {
                dbContext.SubscribersInChatProgress.Remove(subscribersInChatProgress);
                await dbContext.SaveChangesAsync();

                var chatScenario = await dbContext.ChatScenarios.Where(x => x.Id == idChatScenario).FirstOrDefaultAsync();
                if (chatScenario.HasFormula)
                    result = await EvaluateChatScenarioFormulaResult(dbContext, idGroup, idSubscriber, subscribersInChatProgress.UniqueId, chatScenario);
            }

            return result;
        }

        private static async Task<Guid?> ChatScenarioReply(DatabaseContext dbContext, long idGroup, Guid idSubscriber, ChatScenarios chatScenario)
        {
            var isInProgress = await dbContext.SubscribersInChatProgress.AnyAsync(x => x.IdSubscriber == idSubscriber && x.ChatScenarioContent.IdChatScenario == chatScenario.Id);
            if (isInProgress)
                return null;

            var firstChatContent = await dbContext.ChatScenarioContents
                .Where(x => x.IdChatScenario == chatScenario.Id)
                .OrderBy(x => x.Step)
                .FirstOrDefaultAsync();

            if (firstChatContent == null)
                return null;

            await dbContext.History_SubscribersInChatScenariosContents.AddAsync(new History_SubscribersInChatScenariosContents()
            {
                Dt = DateTime.UtcNow,
                IdSubscriber = idSubscriber,
                IdChatScenarioContent = firstChatContent.Id
            });
            await dbContext.SubscribersInChatProgress.AddAsync(new SubscribersInChatProgress()
            {
                DtAdd = DateTime.UtcNow,
                IdSubscriber = idSubscriber,
                IdChatScenarioContent = firstChatContent.Id,
                UniqueId = Guid.NewGuid()
            });
            await dbContext.SaveChangesAsync();

            return firstChatContent.IdMessage;
        }

        private static async Task<Guid?> EvaluateChatScenarioFormulaResult(DatabaseContext dbContext, long idGroup, Guid idSubscriber, Guid uniqueId, ChatScenarios chatScenario)
        {
            var answers = await dbContext.SubscriberChatReplies
                .Where(x => x.IdSubscriber == idSubscriber)
                .Where(x => x.ChatScenarioContent.IdChatScenario == chatScenario.Id)
                .Where(x => x.UniqueId == uniqueId)
                .OrderBy(x => x.ChatScenarioContent.Step)
                .ToArrayAsync();

            string text = null;
            int var0 = 0, var1 = 0, var2 = 0;
            double var4 = 0;

            bool isInvalid = !int.TryParse(answers[0].Text, out var0) ||
                !int.TryParse(answers[1].Text, out var1) ||
                !int.TryParse(answers[2].Text, out var2) ||
                !double.TryParse(answers[4].Text.Split(' ').FirstOrDefault().Replace('.', ','), out var4);
            if (isInvalid)
                text = "Направильно введены данные";
            else
                text = $"Ваша дневная норма калорий (DCI) = {(var0 * 10 + var1 * 6.25 - var2 * 5 + (answers[3].Text.ToLower() == "мужчина" ? 5 : -161)) * var4}\n" +
                    $"Индекс массы тела (ИМТ) = {var0 / (((double)var1 / 100) * ((double)var1 / 100)):N2}";

            var answerMessage = new Messages()
            {
                IdGroup = idGroup,
                Text = text,
                Keyboard = "{\"one_time\":true,\"buttons\":[[{\"action\":{\"type\":\"text\",\"label\":\"Начать\"},\"color\":\"primary\"}]]}"
            };

            await dbContext.Messages.AddAsync(answerMessage);
            await dbContext.SaveChangesAsync();
            if (!isInvalid)
            {
                await dbContext.FilesInMessage.AddAsync(new FilesInMessage()
                {
                    IdFile = Guid.Parse("6b898bee-d40b-4e50-56b8-08d61fded974"),
                    IdMessage = answerMessage.Id
                });

                await dbContext.SaveChangesAsync();
            }
            return answerMessage.Id;
        }
    }
}
