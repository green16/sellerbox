using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellerBox.Models.Database
{
    public class SubscribersInSegments : BaseEntity
    {
        [ForeignKey(nameof(Subscriber))]
        public Guid IdSubscriber { get; set; }
        public virtual Subscribers Subscriber { get; set; }

        [ForeignKey(nameof(Segment))]
        public Guid IdSegment { get; set; }
        public virtual Segments Segment { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DtAdd { get; set; }
    }
}
