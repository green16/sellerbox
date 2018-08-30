using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels.Chains
{
    public class IndexViewModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Название")]
        public string Name { get; set; }
        [Display(Name = "Вкл/выкл")]
        public bool IsEnabled { get; set; }
        [Display(Name = "Подписчиков в цепочке")]
        public int SubscribersInChain { get; set; }
    }
}
