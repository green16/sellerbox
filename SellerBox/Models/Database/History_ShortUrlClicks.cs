using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellerBox.Models.Database
{
    public class History_ShortUrlClicks : BaseHistory
    {
        [ForeignKey(nameof(ShortUrl))]
        public Guid IdShortUrl { get; set; }
        public virtual ShortUrls ShortUrl { get; set; }

        [ForeignKey(nameof(Subscriber))]
        public Guid IdSubscriber { get; set; }
        public virtual Subscribers Subscriber { get; set; }
    }
}
