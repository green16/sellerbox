using System;
using System.ComponentModel.DataAnnotations;

namespace SellerBox.ViewModels.Reposts
{
    public class EditRepostScenarioViewModel
    {
        public Guid? Id { get; set; }
        [Required]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Часов: ")]
        [Range(0, 168, ErrorMessage = "Введите число от 0 до 168")]
        public int CheckAfterHours { get; set; }
        [Required]
        [Display(Name = "Минут: ")]
        [Range(0, 60, ErrorMessage = "Введите число от 0 до 59")]
        public byte CheckAfterMinutes { get; set; }

        [Display(Name = "Пост")]
        public Guid? IdPost { get; set; }

        //[Display(Name = "")]
        public bool CheckLastPosts { get; set; }
        public int? LastPostsCount { get; set; }
        public bool CheckAllPosts { get; set; }

        [Required]
        [Display(Name = "Репост в цепочке")]
        public Guid? IdCheckingChain { get; set; }
        [Required]
        [Display(Name = "После сообщения")]
        public Guid? IdCheckingChainContent { get; set; }

        [Required]
        [Display(Name = "Репост есть + вступил")]
        public Guid? IdGoToChain { get; set; }
        [Display(Name = "Нет репоста + не вступил")]
        public Guid? IdGoToErrorChain1 { get; set; }
        [Display(Name = "Нет репоста + вступил")]
        public Guid? IdGoToErrorChain2 { get; set; }
        [Display(Name = "Репост есть + не вступил")]
        public Guid? IdGoToErrorChain3 { get; set; }
        
        [Display(Name = "Проверять участие в группе")]
        public bool CheckIsSubscriber { get; set; }
    }
}
