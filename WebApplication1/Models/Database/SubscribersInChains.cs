using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models.Database
{
    public class SubscribersInChains : BaseEntity
    {
        [ForeignKey(nameof(Subscriber))]
        public Guid IdSubscriber { get; set; }
        public virtual Subscribers Subscriber { get; set; }

        [ForeignKey(nameof(ChainStep))]
        public Guid IdChainStep { get; set; }
        public virtual ChainContents ChainStep { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DtAdd { get; set; }

        public bool IsSended { get; set; }
    }
}
