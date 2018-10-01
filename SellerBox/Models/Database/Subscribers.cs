using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellerBox.Models.Database
{
    public class Subscribers : BaseEntity
    {
        [Column(TypeName = "datetime2")]
        public DateTime DtAdd { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime? DtUnsubscribe { get; set; }

        public bool? IsChatAllowed { get; set; }

        public bool? IsSubscribedToGroup { get; set; }
        public bool IsUnsubscribed { get; set; }

        public bool IsBlocked { get; set; }

        [ForeignKey(nameof(VkUser))]
        public long IdVkUser { get; set; }
        public virtual VkUsers VkUser { get; set; }

        [ForeignKey(nameof(Group))]
        public long IdGroup { get; set; }
        public virtual Groups Group { get; set; }

        public virtual ICollection<SubscriberReposts> SubscriberReposts { get; set; }
        public virtual ICollection<SubscriberChatReplies> SubscriberChatReplies { get; set; }
        public virtual ICollection<SubscribersInChains> SubscribersInChains { get; set; }
        public virtual ICollection<SubscribersInSegments> SubscribersInSegments { get; set; }
        public virtual ICollection<CheckedSubscribersInRepostScenarios> CheckedSubscribersInRepostScenarios { get; set; }
        public virtual ICollection<History_Birthday> History_Birthday { get; set; }
        public virtual ICollection<History_GroupActions> History_GroupActions { get; set; }
        public virtual ICollection<History_Messages> History_Messages { get; set; }
        public virtual ICollection<History_Scenarios> History_Scenarios { get; set; }
        public virtual ICollection<History_SubscribersInChainSteps> History_SubscribersInChainSteps { get; set; }
        public virtual ICollection<SubscribersInChatProgress> SubscribersInChatProgress { get; set; }
        public virtual ICollection<History_WallPosts> History_WallPosts { get; set; }
        public virtual ICollection<History_SubscribersInChatScenarios> History_SubscribersInChatScenarios { get; set; }
        public virtual ICollection<History_SubscribersInChatScenariosContents> History_SubscribersInChatScenariosContents { get; set; }


    }
}
