using System;
using System.ComponentModel.DataAnnotations;
using SellerBox.ViewModels.Shared;

namespace SellerBox.ViewModels.StaticScenarios
{
    [Serializable]
    public class BirthdayWallSchedulerViewModel : WallMessageViewModel
    {
        [Display(Name = "Отправить после")]
        public byte SendAt { get; set; }
    }
}
