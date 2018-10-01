using System;

namespace SellerBox.ViewModels.Messaging
{
    public class MessagingViewModel
    {
        public Guid IdMessaging { get; set; }

        public string Name { get; set; }
        public long RecipientsCount { get; set; }

        public bool HasKeyboard { get; set; }
        public bool HasImages { get; set; }

        public string Message { get; set; }

        public Models.Database.Common.MessagingStatus Status { get; set; }

        public DateTime DtAdd { get; set; }
        public DateTime DtStart { get; set; }
    }
}
