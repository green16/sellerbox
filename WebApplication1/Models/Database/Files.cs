using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models.Database
{
    public class Files : BaseEntity
    {
        public string Name { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime DtAdd { get; set; }
        public long Size { get; set; }
        public byte[] Content { get; set; }
        public string VkUrl { get; set; }
    }
}
