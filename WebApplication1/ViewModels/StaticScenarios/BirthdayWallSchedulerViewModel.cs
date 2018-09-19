using System;
using System.ComponentModel.DataAnnotations;
using WebApplication1.ViewModels.Shared;

namespace WebApplication1.ViewModels.StaticScenarios
{
    [Serializable]
    public class BirthdayWallSchedulerViewModel : WallMessageViewModel
    {
        [Display(Name = "Отправить после")]
        public byte SendAt { get; set; }
    }
}
