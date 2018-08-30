using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace WebApplication1.ViewModels.Shared
{
    public class MessageViewModel : IValidatableObject
    {
        public bool DisableValidation { get; set; }
        public string PropertiesPrefix { get; set; }
        public string Message { get; set; }
        public List<FileModel> Files { get; set; }
        public bool HasKeyboard { get; set; }
        public List<List<MessageButton>> Keyboard { get; set; }
        public string IsImageFirstString { get; set; }
        public bool IsImageFirst
        {
            get
            {
                return !string.IsNullOrWhiteSpace(IsImageFirstString) && IsImageFirstString == "on";
            }
            set
            {
                IsImageFirstString = value ? "on" : null;
            }
        }

        public bool HasMessage => !string.IsNullOrWhiteSpace(Message) || (Files != null && Files.Any());

        public MessageViewModel()
        {
            PropertiesPrefix = string.Empty;
            Files = new List<FileModel>();
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DisableValidation)
                yield break;

            if (string.IsNullOrWhiteSpace(Message) && !Files.Any())
                yield return new ValidationResult("Сообщение должно содержать либо текст либо вложение", new[] { nameof(Message) });
        }

        public VkConnector.Models.Common.Keyboard GetVkKeyboard()
        {
            return (Keyboard == null || !Keyboard.Any()) ? null :
                new VkConnector.Models.Common.Keyboard
                {
                    Buttons = Keyboard
                    .Select(x => x.Select(y => new VkConnector.Models.Common.Keyboard.Button
                    {
                        Action = new VkConnector.Models.Common.Keyboard.ButtonAction()
                        {
                            Text = y.Text
                        },
                        Color = y.ButtonColor
                    }).ToArray()).ToArray()
                };
        }
    }
}
