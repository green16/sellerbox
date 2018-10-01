using System;
using System.ComponentModel.DataAnnotations;
using SellerBox.ViewModels.Shared;

namespace SellerBox.ViewModels.StaticScenarios
{
    [Serializable]
    public class BirthdaySchedulerViewModel : MessageViewModel
    {
        [Display(Name = "Отправить после")]
        public byte SendAt { get; set; }
        [Display(Name = "За сколько дней отправлять: ")]
        public int DaysBefore { get; set; }
    }
}
