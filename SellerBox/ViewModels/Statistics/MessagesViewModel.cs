using System;

namespace SellerBox.ViewModels.Statistics
{
    public class MessagesViewModel
    {
        public class MessagesPerDayViewModel
        {
            public DateTime Date { get; set; }
            public long Sended { get; set; }
            public long Received { get; set; }
        }

        public long TotalSended { get; set; }
        public long TotalReceived { get; set; }

        public MessagesPerDayViewModel[] MessagesPerDay { get; set; }
    }
}
