using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellerBox.Models.Database
{
    public class History_Scenarios : BaseHistory
    {
        [ForeignKey(nameof(Subscriber))]
        public Guid IdSubscriber { get; set; }
        public virtual Subscribers Subscriber { get; set; }

        [ForeignKey(nameof(Scenario))]
        public Guid IdScenario { get; set; }
        public virtual Scenarios Scenario { get; set; }
    }
}
