using System;

namespace SellerBox.ViewModels.Statistics
{
    public class ScenariosViewModel
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
