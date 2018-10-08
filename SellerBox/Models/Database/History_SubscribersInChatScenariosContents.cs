using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellerBox.Models.Database
{
    public class History_SubscribersInChatScenariosContents : BaseHistory
    {
        [ForeignKey(nameof(ChatScenarioContent))]
        public Guid IdChatScenarioContent { get; set; }
        public virtual ChatScenarioContents ChatScenarioContent { get; set; }

        [ForeignKey(nameof(Subscriber))]
        public Guid IdSubscriber { get; set; }
        public virtual Subscribers Subscriber { get; set; }
    }
}
