﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellerBox.Models.Database
{
    public class ChatScenarios : BaseEntity
    {
        public string Name { get; set; }
        public bool IsEnabled { get; set; }
        public string InputMessage { get; set; }
        public bool HasFormula { get; set; }
        public string Formula { get; set; }

        [ForeignKey(nameof(Group))]
        public long IdGroup { get; set; }
        public virtual Groups Group { get; set; }

        public virtual ICollection<ChatScenarioContents> ChatScenarioContents { get; set; }
    }
}