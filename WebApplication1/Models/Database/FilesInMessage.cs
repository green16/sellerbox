using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models.Database
{
    public class FilesInMessage : BaseEntity
    {
        [ForeignKey(nameof(Message))]
        public Guid IdMessage { get; set; }
        public virtual Messages Message { get; set; }

        [ForeignKey(nameof(File))]
        public Guid IdFile { get; set; }
        public virtual Files File { get; set; }
    }
}
