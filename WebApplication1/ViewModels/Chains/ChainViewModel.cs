using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Models.Database;

namespace WebApplication1.ViewModels.Chains
{
    public class ChainViewModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Название")]
        [Required(ErrorMessage = "Введите название цепочки")]
        public string Name { get; set; }
        [Display(Name = "Группа")]
        [Required(ErrorMessage = "Выберите группу")]
        public int? IdGroup { get; set; }
        public ICollection<ChainContentIndexViewModel> Content { get; set; }
    }
}
