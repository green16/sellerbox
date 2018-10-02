using System;

namespace SellerBox.ViewModels.Statistics
{
    public class ChainsViewModel
    {
        public class MessagesPerDayViewModel
        {
            public string Name { get; set; }
            public long Count { get; set; }
        }

        public class ChainInfoViewModel
        {
            public DateTime Date { get; set; }
            public MessagesPerDayViewModel[] MessagesPerDay { get; set; }

        }

        public long TotalReceived { get; set; }
        public ChainInfoViewModel[] ChainInfo { get; set; }
    }
}
