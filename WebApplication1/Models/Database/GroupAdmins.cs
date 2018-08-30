using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models.Database
{
    public class GroupAdmins : BaseEntity
    {
        [ForeignKey(nameof(Group))]
        public int IdGroup { get; set; }
        public virtual Groups Group { get; set; }

        [ForeignKey(nameof(User))]
        public string IdUser { get; set; }
        public virtual Users User { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DtConnect { get; set; }
    }
}
