using System;

namespace WebApplication1.ViewModels.Chains
{
    public class ChainContentIndexViewModel
    {
        public Guid Id { get; set; }
        public int SendAfterHours { get; set; }
        public int SendAfterMinutes { get; set; }
        public bool IsOnlyDayTime { get; set; }
        public string MessageText { get; set; }
        public string GoToChainName { get; set; }
        public string ExcludeFromChainName { get; set; }
        public int SubscribersInChainContent { get; set; }
    }
}
