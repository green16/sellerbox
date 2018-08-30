using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models.Database
{
    public class VkCallbackMessages : BaseEntity
    {
        public string Type { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime Dt { get; set; }
        public int IdGroup { get; set; }
        public string Object { get; set; }
    }
}
