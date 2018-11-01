using System.ComponentModel.DataAnnotations.Schema;

namespace SellerBox.Models.Database
{
    public class ShortUrls : BaseEntity
    {
        [ForeignKey(nameof(Group))]
        public long IdGroup { get; set; }
        public virtual Groups Group { get; set; }

        public string Name { get; set; }
        public string RedirectTo { get; set; }
        public bool IsSingleClick { get; set; }
    }
}
