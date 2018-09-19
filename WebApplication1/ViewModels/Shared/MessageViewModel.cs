using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VkNet.Enums.SafetyEnums;

namespace WebApplication1.ViewModels.Shared
{
    public class MessageViewModel : IValidatableObject
    {
        public Guid? IdMessage { get; set; }

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

        private KeyboardButtonColor ConvertColor(string colorType)
        {
            switch (colorType.ToLower())
            {
                case "negative":
                    return KeyboardButtonColor.Negative;
                case "positive":
                    return KeyboardButtonColor.Positive;
                case "primary":
                    return KeyboardButtonColor.Primary;
                default:
                    return KeyboardButtonColor.Default;
            }
        }

        public VkNet.Model.Keyboard.MessageKeyboard GetVkKeyboard()
        {
            return (Keyboard == null || !Keyboard.Any()) ? null :
                new VkNet.Model.Keyboard.MessageKeyboard
                {
                    Buttons = Keyboard
                    .Select(x => x.Select(y => new VkNet.Model.Keyboard.MessageKeyboardButton
                    {
                        Action = new VkNet.Model.Keyboard.MessageKeyboardButtonAction()
                        {
                            Label = y.Text
                        },
                        Color = ConvertColor(y.ButtonColor)
                    }).ToArray()).ToArray()
                };
        }
    }
}
