using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellerBox.Models.Database
{
    public class History_WallPosts : BaseHistory
    {
        public bool IsRepost { get; set; }

        [ForeignKey(nameof(WallPost))]
        public Guid IdPost { get; set; }
        public virtual WallPosts WallPost { get; set; }

        [ForeignKey(nameof(Subscriber))]
        public Guid IdSubscriber { get; set; }
        public virtual Subscribers Subscriber { get; set; }
    }
}
