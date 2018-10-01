using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellerBox.Models.Database
{
    public class Logs : BaseEntity
    {
        [Column(TypeName = "datetime2")]
        public DateTime Logged { get; set; }
        public byte Level { get; set; }
        public string Message { get; set; }
        [MaxLength(50)]
        public string Logger { get; set; }
        public string Callsite { get; set; }
        public string Exception { get; set; }
    }
}
