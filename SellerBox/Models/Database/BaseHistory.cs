using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellerBox.Models.Database
{
    public class BaseHistory : BaseEntity
    {
        [Column(TypeName = "datetime2")]
        public DateTime Dt { get; set; }
    }
}
