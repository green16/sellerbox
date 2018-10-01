using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellerBox.Models.Database
{
    public class History_BirthdayWall : BaseEntity
    {
        [ForeignKey(nameof(VkUser))]
        public long IdVkUser { get; set; }
        public virtual VkUsers VkUser { get; set; }

        [ForeignKey(nameof(WallPost))]
        public Guid IdPost { get; set; }
        public virtual WallPosts WallPost { get; set; }

        [ForeignKey(nameof(Group))]
        public long IdGroup { get; set; }
        public virtual Groups Group { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DtSend { get; set; }
    }
}
