using System;
using System.ComponentModel.DataAnnotations;

namespace SellerBox.ViewModels.ShortUrls
{
    public class EditViewModel
    {
        public Guid? Id { get; set; }
        [Required]
        [Display(Name = "Название")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Направление")]
        public string RedirectTo { get; set; }
        [Display(Name = "Считать единственный переход")]
        public bool IsSingleClick { get; set; }
        [Display(Name = "Отслеживать подписчиков")]
        public bool IsSubscriberRequired { get; set; }
        [Display(Name = "Добавлять в цепочку")]
        public bool AddToChain { get; set; }
        public Guid? IdChain { get; set; }
    }
}
