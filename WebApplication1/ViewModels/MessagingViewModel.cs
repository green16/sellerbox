using WebApplication1.ViewModels.Shared;

namespace WebApplication1.ViewModels
{
    public class MessagingViewModel : MessageViewModel
    {
        public long IdGroup { get; set; }
        public string IsSelfSendString { get; set; }
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
