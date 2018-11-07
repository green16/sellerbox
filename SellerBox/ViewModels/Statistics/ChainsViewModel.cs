using System;

namespace SellerBox.ViewModels.Statistics
{

    public class ChainMessagesPerDayViewModel : IEquatable<ChainMessagesPerDayViewModel>
    {
        public Guid IdChain { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }

        public bool Equals(ChainMessagesPerDayViewModel other)
        {
            return IdChain == other?.IdChain;
        }

        public override int GetHashCode() => IdChain.GetHashCode();
    }

    public class ChainInfoViewModel
    {
        public DateTime Date { get; set; }
        public ChainMessagesPerDayViewModel[] MessagesPerDay { get; set; }

    }
}
