using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellerBox.Models.Database
{
    public class History_SubscribersInChainSteps : BaseEntity
    {
        [ForeignKey(nameof(Subscriber))]
        public Guid IdSubscriber { get; set; }
        public virtual Subscribers Subscriber { get; set; }

        [ForeignKey(nameof(ChainStep))]
        public Guid IdChainStep { get; set; }
        public virtual ChainContents ChainStep { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DtAdd { get; set; }
    }
}
