using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models.Database
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
        public int IdVkUser { get; set; }
        public virtual VkUsers VkUser { get; set; }

        [ForeignKey(nameof(Group))]
        public int IdGroup { get; set; }
        public virtual Groups Group { get; set; }

        public virtual ICollection<SubscriberReposts> SubscriberReposts { get; set; }
        public virtual ICollection<SubscribersInChains> SubscribersInChains { get; set; }
        public virtual ICollection<SubscribersInSegments> SubscribersInSegments { get; set; }
        public virtual ICollection<CheckedSubscribersInRepostScenarios> CheckedSubscribersInRepostScenarios { get; set; }

        public Subscribers()
        {
            DtAdd = DateTime.UtcNow;
        }
    }
}
