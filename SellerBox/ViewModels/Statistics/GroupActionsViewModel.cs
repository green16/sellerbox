using System;

namespace SellerBox.ViewModels.Statistics
{
    public class GroupActionsViewModel
    {
        public class ActionsPerDayViewModel
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

        public long TotalJoinGroupCount { get; set; }
        public long TotalLeaveGroupCount { get; set; }
        public long TotalBlockMessagingCount { get; set; }
        public long TotalAcceptMessagingCount { get; set; }
        public long TotalCancelMessagingCount { get; set; }
        public long TotalBlockedCount { get; set; }
        public long TotalUnblockedCount { get; set; }

        public ActionsPerDayViewModel[] ActionsPerDay { get; set; }
    }
}
