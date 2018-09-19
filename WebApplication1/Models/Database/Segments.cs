using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models.Database
{
    public class Segments : BaseEntity
    {
        public string Name { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime DtCreate { get; set; }

        [ForeignKey(nameof(Group))]
        public long IdGroup { get; set; }
        public virtual Groups Group { get; set; }
    }
}
