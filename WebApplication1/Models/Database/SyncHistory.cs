using System;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Common.Hubs;

namespace WebApplication1.Models.Database
{
    public class SyncHistory : BaseEntity
    {
        public int IdGroup { get; set; }
        public SyncType SyncType { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime DtStart { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime? DtEnd { get; set; }
    }
}
