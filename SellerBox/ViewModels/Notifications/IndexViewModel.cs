using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellerBox.ViewModels.Notifications
{
    public class IndexViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Вкл/выкл")]
        public bool IsEnabled { get; set; }
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Column(TypeName = "datetime2")]
        [Display(Name = "Дата создания")]
        public DateTime DtCreate { get; set; }

        public Guid IdElement { get; set; }
        [Display(Name = "Тип элемента")]
        public int ElementType { get; set; }

        [Display(Name = "Название элемента")]
        public string ElementName { get; set; }
        [Display(Name = "Уведомить")]
        public string NotifyTo { get; set; }
        public string NotifyToUrl { get; set; }
        [Display(Name = "Тип уведомления")]
        public int NotificationType { get; set; }
    }
}
