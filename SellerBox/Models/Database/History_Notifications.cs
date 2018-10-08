using System.ComponentModel.DataAnnotations.Schema;

namespace SellerBox.Models.Database
{
    public class History_Notifications : BaseHistory
    {
        [ForeignKey(nameof(Notification))]
        public long IdNotification { get; set; }
        public virtual Notifications Notification { get; set; }
    }
}
