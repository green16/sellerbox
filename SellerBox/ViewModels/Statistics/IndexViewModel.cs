using System.ComponentModel.DataAnnotations;

namespace SellerBox.ViewModels.Statistics
{
    public class IndexViewModel
    {
        [Display(Name = "Дата начала")]
        public string DtStart { get; set; }
        [Display(Name = "Дата окончания")]
        public string DtEnd { get; set; }
        [Display(Name = "Тип отчёта")]
        public StatisticType StatisticType { get; set; }
    }
}
