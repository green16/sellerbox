using System;
using System.ComponentModel.DataAnnotations.Schema;
using SellerBox.Models.Database.Common;

namespace SellerBox.Models.Database
{
    public class History_GroupActions : BaseEntity
    {
        [ForeignKey(nameof(Subscriber))]
        public Guid IdSubscriber { get; set; }
        public virtual Subscribers Subscriber { get; set; }

        [ForeignKey(nameof(Group))]
        public long IdGroup { get; set; }
        public virtual Groups Group { get; set; }

        public GroupActionTypes ActionType { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime Dt { get; set; }
    }
}
