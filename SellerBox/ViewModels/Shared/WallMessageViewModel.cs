using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SellerBox.ViewModels.Shared
{
    public class WallMessageViewModel : IValidatableObject
    {
        public Guid? IdMessage { get; set; }

        public bool DisableValidation { get; set; }
        public string PropertiesPrefix { get; set; }
        public string Message { get; set; }
        public List<FileModel> Files { get; set; }

        public bool HasMessage => !string.IsNullOrWhiteSpace(Message) || (Files != null && Files.Any());

        public WallMessageViewModel()
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
    }
}
