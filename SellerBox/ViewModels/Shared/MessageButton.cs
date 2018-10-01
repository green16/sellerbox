namespace SellerBox.ViewModels.Shared
{
    public class MessageButton
    {
        public const byte MaxRows = 10;
        public const byte MaxColumns = 4;

        public byte Row { get; set; }
        public byte Column { get; set; }
        public string Text { get; set; }
        public string ButtonColor { get; set; }
        public bool CanDelete { get; set; }
        public string PropertiesPrefix { get; set; }
    }
}
