using System;

namespace SellerBox.ViewModels.Reposts
{
    public class IndexRepostScenarioViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid IdCheckingChain { get; set; }
        public string CheckingChainName { get; set; }
        public Uri PostLink { get; set; }
        public int MessageIndex { get; set; }
        public string MessageTextPart { get; set; }
        public bool HasGoToChain { get; set; }
        public bool HasGoToChain2 { get; set; }
        public string GoToChainName { get; set; }
        public string GoToChain2Name { get; set; }
    }
}
