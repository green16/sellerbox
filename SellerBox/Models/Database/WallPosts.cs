using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellerBox.Models.Database
{
    public class WallPosts
    {
        [Key]
        public Guid Id { get; set; }
        public long IdVk { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime DtAdd { get; set; }

        public string Text { get; set; }

        [ForeignKey(nameof(Group))]
        public long IdGroup { get; set; }
        public virtual Groups Group { get; set; }

        [NotMapped]
        public string TextPart
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Text))
                    return string.Empty;
                if (Text.Length <= 20)
                    return Text.Substring(0, Text.Length);
                return string.Concat(Text.Substring(0, 17), "...");
            }
        }

        public virtual History_BirthdayWall BirthdayWallHistory { get; set; }
    }
}
