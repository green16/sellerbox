using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models.Database
{
    public class History_Scenarios : BaseEntity
    {
        [ForeignKey(nameof(Subscriber))]
        public Guid IdSubscriber { get; set; }
        public virtual Subscribers Subscriber { get; set; }

        [ForeignKey(nameof(Scenario))]
        public Guid IdScenario { get; set; }
        public virtual Scenarios Scenario { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime Dt { get; set; }
    }
}
