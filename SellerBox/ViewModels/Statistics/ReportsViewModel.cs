using System.ComponentModel.DataAnnotations;

namespace SellerBox.ViewModels.Statistics
{
    public class ReportsViewModel
    {
        [Display(Name = "Дата начала")]
        public string DtStart { get; set; }
        [Display(Name = "Дата окончания")]
        public string DtEnd { get; set; }
        [Display(Name = "Тип отчёта")]
        public byte StatisticType { get; set; }
    }
}
