using System;

namespace SellerBox.ViewModels.Statistics
{
    public class GroupActionsPerDayViewModel
    {
        public DateTime Date { get; set; }

        public long JoinGroupCount { get; set; }
        public long LeaveGroupCount { get; set; }
        public long BlockMessagingCount { get; set; }
        public long AcceptMessagingCount { get; set; }
        public long CancelMessagingCount { get; set; }
        public long BlockedCount { get; set; }
        public long UnblockedCount { get; set; }
    }
}
