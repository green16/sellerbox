using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models.Database
{
    public class BirthdayHistory : BaseEntity
    {
        [ForeignKey(nameof(VkUser))]
        public int IdVkUser { get; set; }
        public virtual VkUsers VkUser { get; set; }

        [ForeignKey(nameof(Group))]
        public int IdGroup { get; set; }
        public virtual Groups Group { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DtSend { get; set; }
    }
}
