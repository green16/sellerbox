using System;

namespace SellerBox.ViewModels.Shared
{
    public class FileModel
    {
        public uint Index { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PropertiesPrefix { get; set; }
    }
}
