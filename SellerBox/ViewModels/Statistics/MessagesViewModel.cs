using System;

namespace SellerBox.ViewModels.Statistics
{
    public class MessagesPerDayViewModel
    {
        public DateTime Date { get; set; }
        public int Sended { get; set; }
        public int Received { get; set; }
    }
}
