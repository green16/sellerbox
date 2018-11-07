using System;

namespace SellerBox.ViewModels.Statistics
{
    public class ShortUrlsPerDayViewModel : IEquatable<ShortUrlsPerDayViewModel>
    {
        public Guid IdShortUrl { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }

        public bool Equals(ShortUrlsPerDayViewModel other)
        {
            return IdShortUrl == other?.IdShortUrl;
        }

        public override int GetHashCode() => IdShortUrl.GetHashCode();
    }

    public class ShortUrlsInfoViewModel
    {
        public DateTime Date { get; set; }
        public ShortUrlsPerDayViewModel[] ShortUrlsPerDay { get; set; }

    }
}
