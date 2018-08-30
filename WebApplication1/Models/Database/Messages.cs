using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models.Database
{
    public class Messages : BaseEntity
    {
        [ForeignKey(nameof(Group))]
        public int IdGroup { get; set; }
        public virtual Groups Group { get; set; }

        public string Text { get; set; }
        public virtual ICollection<FilesInMessage> Files { get; set; }
        public bool IsImageFirst { get; set; }
        public string Keyboard { get; set; }
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
    }
}
