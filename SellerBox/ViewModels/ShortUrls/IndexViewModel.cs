using System;
using System.ComponentModel.DataAnnotations;

namespace SellerBox.ViewModels.ShortUrls
{
    public class IndexViewModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Название")]
        public string Name { get; set; }
        [Display(Name = "Направление")]
        public string RedirectTo { get; set; }
    }
}
