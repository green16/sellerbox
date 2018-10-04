using System;

namespace SellerBox.ViewModels.Statistics
{
    public class MessagesPerDayViewModel
    {
        public DateTime Date { get; set; }
        public long Sended { get; set; }
        public long Received { get; set; }
    }
}
