using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels.Subscribers
{
    public class UnmemberedIndexViewModel
    {
        public Guid Id { get; set; }
        public int IdVk { get; set; }
        public Uri Link { get; set; }
        [Display(Name = "Фото")]
        public Uri Photo { get; set; }
        [Display(Name = "ФИО")]
        public string FIO { get; set; }
        [Display(Name = "Пол")]
        public bool? Sex { get; set; }
        [Display(Name = "Страна")]
        public string Country { get; set; }
        [Display(Name = "Город")]
        public string City { get; set; }
        private DateTime? _birthday;
        [Display(Name = "День рождения")]
        [DisplayFormat(DataFormatString = "{0:dd MMMM yyyy}")]
        public DateTime? Birthday
        {
            get { return _birthday.HasValue ? (DateTime?)_birthday.Value.ToLocalTime() : null; }
            set { _birthday = value; }
        }
        private DateTime _dtAdd;
        [Display(Name = "Дата добавления")]
        [DisplayFormat(ConvertEmptyStringToNull = true, DataFormatString = "{0:dd MMMM yyyy HH:mm}")]
        public DateTime DtAdd { get { return _dtAdd.ToLocalTime(); } set { _dtAdd = value; } }
        [Display(Name = "Участник")]
        public bool? IsSubscriber { get; set; }
    }
}
