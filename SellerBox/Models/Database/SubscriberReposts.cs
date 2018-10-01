using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellerBox.Models.Database
{
    public class SubscriberReposts : BaseEntity
    {
        public string Text { get; set; }

        public bool IsProcessed { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DtRepost { get; set; }

        [ForeignKey(nameof(WallPost))]
        public Guid IdPost { get; set; }
        public virtual WallPosts WallPost { get; set; }

        [ForeignKey(nameof(Subscriber))]
        public Guid IdSubscriber { get; set; }
        public virtual Subscribers Subscriber { get; set; }
    }
}
