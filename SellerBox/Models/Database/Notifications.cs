using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellerBox.Models.Database
{
    public class Notifications : BaseEntity
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        public bool IsEnabled { get; set; }

        [ForeignKey(nameof(Group))]
        public long IdGroup { get; set; }
        public virtual Groups Group { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DtCreate { get; set; }

        public string IdElement { get; set; }
        public int ElementType { get; set; }
        public string NotifyTo { get; set; }
        public int NotificationType { get; set; }
    }
}
