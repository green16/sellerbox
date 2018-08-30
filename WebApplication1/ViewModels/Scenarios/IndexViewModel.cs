using System;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Models.Database.Common;

namespace WebApplication1.ViewModels.Scenarios
{
    public class IndexViewModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Название")]
        public string Name { get; set; }
        [Display(Name = "Точное соответствие")]
        public bool IsStrictMatch { get; set; }
        [Display(Name = "Входящее сообщение")]
        public string InputMessage { get; set; }
        [Display(Name = "Действие")]
        public ScenarioActions Action { get; set; }
        public string ChainName { get; set; }
    }
}
