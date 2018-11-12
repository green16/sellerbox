using System;
using System.ComponentModel.DataAnnotations;

namespace SellerBox.ViewModels.ShortUrlsScenarios
{
    public class IndexViewModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Активность")]
        public bool IsEnabled { get; set; }
        [Display(Name = "Название")]
        public string Name { get; set; }
        [Display(Name = "Проверяемая цепочка")]
        public string CheckingChainName { get; set; }
        [Display(Name = "№ сообщения")]
        public int MessageIndex { get; set; }
        [Display(Name = "Проверка подписки")]
        public bool CheckIsSubscriber { get; set; }
        public string MessageText { get; set; }
        [Display(Name = "Проверяемая ссылка")]
        public string ShortUrlName { get; set; }
    }
}
