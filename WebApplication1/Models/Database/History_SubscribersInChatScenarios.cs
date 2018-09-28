using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models.Database
{
    public class History_SubscribersInChatScenarios : BaseEntity
    {
        [Column(TypeName = "datetime2")]
        public DateTime Dt { get; set; }

        [ForeignKey(nameof(ChatScenario))]
        public Guid IdChatScenario { get; set; }
        public virtual ChatScenarios ChatScenario { get; set; }

        [ForeignKey(nameof(Subscriber))]
        public Guid IdSubscriber { get; set; }
        public virtual Subscribers Subscriber { get; set; }
    }
}
