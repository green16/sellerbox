using System;
using System.ComponentModel.DataAnnotations;
using WebApplication1.ViewModels.Shared;

namespace WebApplication1.ViewModels.StaticScenarios
{
    [Serializable]
    public class BirthdaySchedulerViewModel : MessageViewModel
    {
        public Guid? IdMessage { get; set; }
        [Display(Name = "Отправить после")]
        public byte SendAt { get; set; }
        [Display(Name = "За сколько дней отправлять: ")]
        public int DaysBefore { get; set; }
    }
}
