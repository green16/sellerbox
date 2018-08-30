using System;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Models.Database.Common;

namespace WebApplication1.Models.Database
{
    public class Tasks : BaseEntity
    {
        public TaskTypes TaskType { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime ExecuteAt { get; set; }

        [ForeignKey(nameof(SubscriberInChain))]
        public Guid? IdSubscriberInChain { get; set; }
        public virtual SubscribersInChains SubscriberInChain { get; set; }
    }
}
