using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellerBox.Models.Database
{
    public class ShortUrlsPassedClicks : BaseEntity
    {
        [Column(TypeName = "datetime2")]
        public DateTime Dt { get; set; }

        [ForeignKey(nameof(Subscriber))]
        public Guid IdSubscriber { get; set; }
        public virtual Subscribers Subscriber { get; set; }

        [ForeignKey(nameof(ShortUrlsScenario))]
        public Guid IdShortUrlsScenario { get; set; }
        public virtual ShortUrlsScenarios ShortUrlsScenario { get; set; }
    }
}
