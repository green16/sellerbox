using System;

namespace SellerBox.ViewModels.Statistics
{
    public class GroupActionsPerDayViewModel
    {
        public DateTime Date { get; set; }

        public int JoinGroupCount { get; set; }
        public int LeaveGroupCount { get; set; }
        public int BlockMessagingCount { get; set; }
        public int AcceptMessagingCount { get; set; }
        public int CancelMessagingCount { get; set; }
        public int BlockedCount { get; set; }
        public int UnblockedCount { get; set; }
    }
}
