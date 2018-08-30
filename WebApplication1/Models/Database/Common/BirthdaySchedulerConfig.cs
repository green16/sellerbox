using System;

namespace WebApplication1.Models.Database.Common
{
    public class BirthdaySchedulerConfig
    {
        public DateTime SendAt { get; set; }
        public int DaysBefore { get; set; }
        public Guid IdMessage { get; set; }
    }
}
