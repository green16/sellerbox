﻿using System;

namespace SellerBox.ViewModels.Statistics
{
    public class ChatContentsViewModel
    {
        public class MessagesPerDayViewModel
        {
            public DateTime Date { get; set; }
            public long Count { get; set; }
        }

        public long TotalReceived { get; set; }
        public MessagesPerDayViewModel[] MessagesPerDay { get; set; }
    }
}
