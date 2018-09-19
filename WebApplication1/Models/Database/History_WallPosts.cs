using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models.Database
{
    public class History_WallPosts : BaseEntity
    {
        public bool IsRepost { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime Dt { get; set; }

        [ForeignKey(nameof(WallPost))]
        public Guid IdPost { get; set; }
        public virtual WallPosts WallPost { get; set; }

        [ForeignKey(nameof(Subscriber))]
        public Guid IdSubscriber { get; set; }
        public virtual Subscribers Subscriber { get; set; }
    }
}
