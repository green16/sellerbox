﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models.Database
{
    public class History_Messages : BaseEntity
    {
        [ForeignKey(nameof(Message))]
        public Guid? IdMessage { get; set; }
        public virtual Messages Message { get; set; }

        [ForeignKey(nameof(Subscriber))]
        public Guid IdSubscriber { get; set; }
        public virtual Subscribers Subscriber { get; set; }

        public bool IsOutgoingMessage { get; set; }

        public string Text { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime Dt { get; set; }
    }
}
