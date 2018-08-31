using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models.Database
{
    public class BirthdayWallScenarios : BaseEntity
    {
        public byte SendAt { get; set; }
        public bool IsEnabled { get; set; }

        [ForeignKey(nameof(Group))]
        public int IdGroup { get; set; }
        public virtual Groups Group { get; set; }

        [ForeignKey(nameof(MessagesGroup))]
        public Guid IdMessagesGroup { get; set; }
        public virtual MessagesGroups MessagesGroup { get; set; }
    }
}
