using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellerBox.Models.Database
{
    public class VkCallbackMessages : BaseEntity
    {
        public bool IsProcessed { get; set; }
        public bool HasError { get; set; }

        public string Type { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime Dt { get; set; }

        [ForeignKey(nameof(Group))]
        public long IdGroup { get; set; }
        public virtual Groups Group { get; set; }

        public string Object { get; set; }
    }
}
