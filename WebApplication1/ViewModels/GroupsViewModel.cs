using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels
{
    public class GroupsViewModel
    {
        public long IdVk { get; set; }
        [Display(Name = "Название")]
        public string Name { get; set; }
        public string Link { get; set; }
        [Display(Name = "Статус")]
        public bool IsConnected { get; set; }
        [Display(Name = "Изображение")]
        public Uri ImageUrl { get; set; }
    }
}
