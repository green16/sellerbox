using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SellerBox.ViewModels.Subscribers
{
    public class SubscriberCardViewModel
    {
        #region VkUser
        public long IdVk { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string Photo { get; set; }
        [Display(Name = "Пол")]
        public bool? Sex { get; set; }
        [Display(Name = "Страна")]
        public string Country { get; set; }
        [Display(Name = "Город")]
        public string City { get; set; }
        [Display(Name = "День рождения")]
        public DateTime? Birthday { get; set; }
        #endregion

        #region Subscriber
        [Display(Name = "Дата добавления в систему")]
        public DateTime DtAdd { get; set; }
        [Display(Name = "Разрешено отправлять сообщения")]
        public bool? IsChatAllowed { get; set; }
        [Display(Name = "Участник группы")]
        public bool? IsSubscribedToGroup { get; set; }
        [Display(Name = "Отписался от группы")]
        public bool IsUnsubscribed { get; set; }
        [Display(Name = "Дата отписки от группы")]
        public DateTime? DtUnsubscribe { get; set; }
        [Display(Name = "Заблокирован")]
        public bool IsBlocked { get; set; }
        #endregion

        public IEnumerable<string> Segments { get; set; }
    }
}
