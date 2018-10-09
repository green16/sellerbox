using System;
using System.ComponentModel.DataAnnotations;

namespace SellerBox.ViewModels.Notifications
{
    public class EditViewModel
    {
        public Guid? Id { get; set; }
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Тип элемента")]
        public int? ElementType { get; set; }
        [Display(Name = "Элемент")]
        public string IdElement { get; set; }
        [Display(Name = "Уведомить")]
        public string NotifyTo { get; set; }
        [Display(Name = "Тип уведомления")]
        public int? NotificationType { get; set; }
    }
}
