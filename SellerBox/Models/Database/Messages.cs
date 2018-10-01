using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellerBox.Models.Database
{
    public class Messages : BaseEntity
    {
        [ForeignKey(nameof(Group))]
        public long IdGroup { get; set; }
        public virtual Groups Group { get; set; }

        public string Text { get; set; }
        public virtual ICollection<FilesInMessage> Files { get; set; }
        public bool IsImageFirst { get; set; }
        public string Keyboard { get; set; }

        [NotMapped]
        public string TextPart => GetTextPart(Text);

        public static string GetTextPart(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;
            if (str.Length <= 20)
                return str;
            return string.Concat(str.Substring(0, 17), "...");
        }
    }
}
