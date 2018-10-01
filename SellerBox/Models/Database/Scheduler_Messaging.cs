using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SellerBox.Models.Database.Common;

namespace SellerBox.Models.Database
{
    public class Scheduler_Messaging : BaseEntity
    {
        public string Name { get; set; }

        [ForeignKey(nameof(Message))]
        public Guid IdMessage { get; set; }
        public virtual Messages Message { get; set; }

        public string VkUserIds { get; set; }
        public long RecipientsCount { get; set; }

        public MessagingStatus Status { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DtAdd { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DtStart { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? DtEnd { get; set; }

        [NotMapped]
        public long[] RecipientIds { get => string.IsNullOrWhiteSpace(VkUserIds) ? new long[0] : VkUserIds.Split(',').Select(x => long.Parse(x)).ToArray(); }
    }
}
