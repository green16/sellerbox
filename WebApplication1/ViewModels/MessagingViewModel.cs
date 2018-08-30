using WebApplication1.ViewModels.Shared;

namespace WebApplication1.ViewModels
{
    public class MessagingViewModel : MessageViewModel
    {
        public int IdGroup { get; set; }
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
