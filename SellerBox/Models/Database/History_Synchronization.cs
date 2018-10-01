using System;
using System.ComponentModel.DataAnnotations.Schema;
using SellerBox.Common.Hubs;

namespace SellerBox.Models.Database
{
    public class History_Synchronization : BaseEntity
    {
        [ForeignKey(nameof(Group))]
        public long IdGroup { get; set; }
        public virtual Groups Group { get; set; }

        public SyncType SyncType { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime DtStart { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime? DtEnd { get; set; }
    }
}
