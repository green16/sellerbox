using System;
using System.ComponentModel.DataAnnotations;

namespace SellerBox.ViewModels.Messaging
{
    public class MessagingHistoryViewModel
    {
        [Display(Name = "Название")]
        public string Name { get; set; }
        [Display(Name = "Кол-во получателей")]
        public long RecipientsCount { get; set; }
        [Display(Name = "Клавиатура")]
        public bool HasKeyboard { get; set; }
        [Display(Name = "Изображения")]
        public bool HasImages { get; set; }
        [Display(Name = "Сообщение")]
        public string Message { get; set; }
        public string CropMessage { get; set; }
        [Display(Name = "Добавлено")]
        public DateTime DtAdd { get; set; }
        [Display(Name = "Запуск")]
        public DateTime DtStart { get; set; }
        [Display(Name = "Окончание")]
        public DateTime DtEnd { get; set; }
    }
}
