using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models.Database
{
    public class MessagesGroups : BaseEntity
    {
        public string Name { get; set; }

        [ForeignKey(nameof(Group))]
        public int IdGroup { get; set; }
        public virtual Groups Group { get; set; }

        public virtual ICollection<Messages> Messages { get; set; }
    }
}
