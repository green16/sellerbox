using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models.Database
{
    public class BirthdayScenarios : BaseEntity
    {
        public byte SendAt { get; set; }
        public int DaysBefore { get; set; }
        public bool IsEnabled { get; set; }

        [ForeignKey(nameof(Group))]
        public long IdGroup { get; set; }
        public virtual Groups Group { get; set; }
        [ForeignKey(nameof(Message))]
        public Guid IdMessage { get; set; }
        public virtual Messages Message { get; set; }
    }
}
