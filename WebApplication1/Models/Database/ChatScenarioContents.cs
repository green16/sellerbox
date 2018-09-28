using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models.Database
{
    public class ChatScenarioContents : BaseEntity
    {
        public uint Step { get; set; }

        [ForeignKey(nameof(ChatScenario))]
        public Guid IdChatScenario { get; set; }
        public virtual ChatScenarios ChatScenario { get; set; }

        public Guid IdMessage { get; set; }
    }
}
