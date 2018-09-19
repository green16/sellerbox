using System;
using System.ComponentModel.DataAnnotations;
using WebApplication1.ViewModels.Shared;

namespace WebApplication1.ViewModels.Chains
{
    public class EditMessageViewModel : MessageViewModel
    {
        public Guid? IdChainContent { get; set; }

        public Guid IdChain { get; set; }

        [Required]
        [Display(Name = "Часов: ")]
        [Range(0, 168, ErrorMessage = "Введите число от 0 до 168")]
        public int Hours { get; set; }
        [Required]
        [Display(Name = "Минут: ")]
        [Range(0, 60, ErrorMessage = "Введите число от 0 до 59")]
        public byte Minutes { get; set; }

        [Display(Name = "Время отправки: ")]
        public bool IsOnlyDayTime { get; set; }
        [Display(Name = "Переключить в цепочку: ")]
        public Guid? IdGoToChain { get; set; }
        [Display(Name = "Исключить из цепочки: ")]
        public Guid? IdExcludeFromChain { get; set; }
    }
}
