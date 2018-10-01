using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellerBox.Models.Database
{
    public class CheckedSubscribersInRepostScenarios : BaseEntity
    {
        [ForeignKey(nameof(Subscriber))]
        public Guid IdSubscriber { get; set; }
        public virtual Subscribers Subscriber { get; set; }

        [ForeignKey(nameof(RepostScenario))]
        public Guid? IdRepostScenario { get; set; }
        public virtual RepostScenarios RepostScenario { get; set; }
        
        public DateTime DtCheck { get; set; }
    }
}
