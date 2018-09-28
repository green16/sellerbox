using WebApplication1.ViewModels.Shared;

namespace WebApplication1.ViewModels.Messaging
{
    public class EditMessagingViewModel : MessageViewModel
    {
        public string Name { get; set; }
        public long IdGroup { get; set; }
        public string IsSelfSendString { get; set; }
        public string DtStart { get; set; } = System.DateTime.Now.ToString("dd.MM.yyyy HH:mm");

        public bool IsSelfSend
        {
            get
            {
                return !string.IsNullOrWhiteSpace(IsSelfSendString) && IsSelfSendString == "on";
            }
            set
            {
                IsSelfSendString = value ? "on" : null;
            }
        }
    }
}
