using System;

namespace SellerBox.ViewModels.Statistics
{

    public class ChainMessagesPerDayViewModel
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }

    public class ChainInfoViewModel
    {
        public DateTime Date { get; set; }
        public ChainMessagesPerDayViewModel[] MessagesPerDay { get; set; }

    }
}
