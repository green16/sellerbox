using System;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Models.Database.Common;

namespace WebApplication1.Models.Database
{
    public class Scenarios : BaseEntity
    {
        public string Name { get; set; }
        public bool IsEnabled { get; set; }
        public string InputMessage { get; set; }
        public bool IsStrictMatch { get; set; }
        public ScenarioActions Action { get; set; }

        [ForeignKey(nameof(Group))]
        public long IdGroup { get; set; }
        public virtual Groups Group { get; set; }

        [ForeignKey(nameof(Message))]
        public Guid? IdMessage { get; set; }
        public virtual Messages Message { get; set; }

        [ForeignKey(nameof(ErrorMessage))]
        public Guid? IdErrorMessage { get; set; }
        public virtual Messages ErrorMessage { get; set; }

        [ForeignKey(nameof(Chain))]
        public Guid? IdChain { get; set; }
        public virtual Chains Chain { get; set; }

        [ForeignKey(nameof(Chain2))]
        public Guid? IdChain2 { get; set; }
        public virtual Chains Chain2 { get; set; }

        public static Scenarios FromScenarioModel(ScenarioModel scenarioModel)
        {
            Scenarios result = new Scenarios()
            {
                InputMessage = scenarioModel.InputMessage,
                IsStrictMatch = scenarioModel.IsStrictMatch,
                Name = scenarioModel.Name,
                Action = scenarioModel.Action
            };

            if (!string.IsNullOrWhiteSpace(scenarioModel.IdChain))
                result.IdChain = Guid.Parse(scenarioModel.IdChain);

            return result;
        }
    }
}
