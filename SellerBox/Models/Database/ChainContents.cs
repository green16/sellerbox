using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellerBox.Models.Database
{
    public class ChainContents : BaseEntity
    {
        public int Index { get; set; }

        public bool IsOnlyDayTime { get; set; }

        [NotMapped]
        public TimeSpan SendAfter => TimeSpan.FromSeconds(SendAfterSeconds);
        public int SendAfterSeconds { get; set; }

        [ForeignKey(nameof(Chain))]
        public Guid IdChain { get; set; }
        public virtual Chains Chain { get; set; }

        [ForeignKey(nameof(GoToChain))]
        public Guid? IdGoToChain { get; set; }
        public virtual Chains GoToChain { get; set; }

        [ForeignKey(nameof(ExcludeFromChain))]
        public Guid? IdExcludeFromChain { get; set; }
        public virtual Chains ExcludeFromChain { get; set; }

        [ForeignKey(nameof(Message))]
        public Guid? IdMessage { get; set; }
        public virtual Messages Message { get; set; }
    }
}
