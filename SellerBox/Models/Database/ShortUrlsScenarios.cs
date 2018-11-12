using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellerBox.Models.Database
{
    public class ShortUrlsScenarios : BaseEntity
    {
        public string Name { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime DtCreate { get; set; }
        public bool IsEnabled { get; set; }
        public int CheckAfterSeconds { get; set; }
        [NotMapped]
        public TimeSpan CheckAfter => TimeSpan.FromSeconds(CheckAfterSeconds);

        [ForeignKey(nameof(ShortUrl))]
        public Guid? IdShortUrl { get; set; }
        public virtual ShortUrls ShortUrl { get; set; }

        public bool CheckIsSubscriber { get; set; }

        [ForeignKey(nameof(CheckingChainContent))]
        public Guid IdCheckingChainContent { get; set; }
        public virtual ChainContents CheckingChainContent { get; set; }

        [ForeignKey(nameof(GoToChain))]
        public Guid? IdGoToChain { get; set; }
        public virtual Chains GoToChain { get; set; }

        [ForeignKey(nameof(GoToErrorChain1))]
        public Guid? IdGoToErrorChain1 { get; set; }
        public virtual Chains GoToErrorChain1 { get; set; }

        [ForeignKey(nameof(GoToErrorChain2))]
        public Guid? IdGoToErrorChain2 { get; set; }
        public virtual Chains GoToErrorChain2 { get; set; }

        [ForeignKey(nameof(GoToErrorChain3))]
        public Guid? IdGoToErrorChain3 { get; set; }
        public virtual Chains GoToErrorChain3 { get; set; }
    }
}
