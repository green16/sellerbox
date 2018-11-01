using System;

namespace SellerBox.ViewModels.Statistics
{
    public class ShortUrlsPerDayViewModel
    {
        public string Name { get; set; }
        public long Count { get; set; }
    }

    public class ShortUrlsInfoViewModel
    {
        public DateTime Date { get; set; }
        public ShortUrlsPerDayViewModel[] ShortUrlsPerDay { get; set; }

    }
}
