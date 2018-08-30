using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WebApplication1.Models.Database.Common;
using WebApplication1.ViewModels.Shared;

namespace WebApplication1.ViewModels.Scenarios
{
    public class ScenarioViewModel : IValidatableObject
    {
        public Guid? IdScenario { get; set; }
        [Required(ErrorMessage = "Введите название сценария")]
        [Display(Name = "Название")]
        public string Name { get; set; }
        public bool IsStrictMatch { get; set; }
        [Required(ErrorMessage = "Введите входящее сообщение")]
        [Display(Name = "Входящее сообщение")]
        public string InputMessage { get; set; }

        public ScenarioActions Action { get; set; }

        [Display(Name = "Включаемая цепочка")]
        public Guid? IdChain { get; set; }
        [Display(Name = "Исключаемая цепочка")]
        public Guid? IdChain2 { get; set; }

        public Guid? IdMessage { get; set; }
        public Guid? IdErrorMessage { get; set; }

        public MessageViewModel Message { get; set; }
        public MessageViewModel ErrorMessage { get; set; }

        public ScenarioViewModel()
        {
            Message = new MessageViewModel()
            {
                PropertiesPrefix = nameof(Message),
                DisableValidation = true
            };
            ErrorMessage = new MessageViewModel()
            {
                PropertiesPrefix = nameof(ErrorMessage),
                DisableValidation = true
            };
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            switch (Action)
            {
                case ScenarioActions.Message:
                    {
                        Message.DisableValidation = false;
                        foreach (var error in Message.Validate(validationContext))
                            yield return error;
                        break;
                    }
                case ScenarioActions.AddToChain:
                    {
                        if (!IdChain.HasValue)
                            yield return new ValidationResult("Не выбрана включаемая цепочка", new[] { nameof(IdChain) });
                        break;
                    }
                case ScenarioActions.RemoveFromChain:
                    {
                        if (!IdChain2.HasValue)
                            yield return new ValidationResult("Не выбрана исключаемая цепочка", new[] { nameof(IdChain) });
                        break;
                    }
                case ScenarioActions.ChangeChain:
                    {
                        if (!IdChain.HasValue)
                            yield return new ValidationResult("Не выбрана включаемая цепочка", new[] { nameof(IdChain) });
                        if (!IdChain2.HasValue)
                            yield return new ValidationResult("Не выбрана исключаемая цепочка", new[] { nameof(IdChain2) });
                        break;
                    }
            }
        }
    }
}
