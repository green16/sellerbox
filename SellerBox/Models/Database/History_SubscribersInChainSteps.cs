using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellerBox.Models.Database
{
    public class History_SubscribersInChainSteps : BaseHistory
    {
        [ForeignKey(nameof(Subscriber))]
        public Guid IdSubscriber { get; set; }
        public virtual Subscribers Subscriber { get; set; }

        [ForeignKey(nameof(ChainStep))]
        public Guid IdChainStep { get; set; }
        public virtual ChainContents ChainStep { get; set; }
    }
}
