using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellerBox.Models.Database
{
    public class RepostScenarios
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsEnabled { get; set; }
        public int CheckAfterSeconds { get; set; }
        [NotMapped]
        public TimeSpan CheckAfter => TimeSpan.FromSeconds(CheckAfterSeconds);

        [ForeignKey(nameof(WallPost))]
        public Guid? IdPost { get; set; }
        public virtual WallPosts WallPost { get; set; }

        public bool CheckLastPosts { get; set; }
        public int? LastPostsCount { get; set; }
        public bool CheckAllPosts { get; set; }

        public bool CheckIsSubscriber { get; set; }

        [ForeignKey(nameof(CheckingChainContent))]
        public Guid IdCheckingChainContent { get; set; }
        public virtual ChainContents CheckingChainContent { get; set; }

        [ForeignKey(nameof(GoToChain))]
        public Guid? IdGoToChain { get; set; }
        public virtual Chains GoToChain { get; set; }

        [ForeignKey(nameof(GoToChain2))]
        public Guid? IdGoToChain2 { get; set; }
        public virtual Chains GoToChain2 { get; set; }
    }
}
