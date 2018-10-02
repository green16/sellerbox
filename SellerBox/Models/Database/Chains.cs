﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellerBox.Models.Database
{
    public class Chains : BaseEntity
    {
        [Display(Name="Название")]
        public string Name { get; set; }
        public bool IsEnabled { get; set; }

        [ForeignKey(nameof(Group))]
        public long IdGroup { get; set; }
        public virtual Groups Group { get; set; }
    }
}