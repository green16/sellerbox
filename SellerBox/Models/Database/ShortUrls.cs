using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellerBox.Models.Database
{
    public class ShortUrls : BaseEntity
    {
        [Column(TypeName = "datetime2")]
        public DateTime DtAdd { get; set; }

        [ForeignKey(nameof(Group))]
        public long IdGroup { get; set; }
        public virtual Groups Group { get; set; }

        [ForeignKey(nameof(Chain))]
        public Guid? IdChain { get; set; }
        public virtual Chains Chain { get; set; }

        public string Name { get; set; }
        public string RedirectTo { get; set; }
        public bool IsSingleClick { get; set; }
        public bool IsSubscriberRequired { get; set; }
    }
}
