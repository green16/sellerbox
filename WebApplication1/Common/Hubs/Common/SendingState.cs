using System;

namespace WebApplication1.Common.Hubs.Common
{
    public class SendingState
    {
        public int Total { get; set; } = 0;
        public int Progress { get; set; } = 0;
        public Guid IdMessaging { get; set; }

        public SendingState(Guid idMessaging)
        {
            IdMessaging = idMessaging;
        }

        public SendingState(int total, int progress, Guid idMessaging)
        {
            Total = total;
            Progress = progress;
            IdMessaging = idMessaging;
        }
    }
}
