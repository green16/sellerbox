using System;

namespace SellerBox.ViewModels.Statistics
{
    public class ChatContentsViewModel
    {
        public class ChatScenarioContentInfo
        {
            public string Step { get; set; }
            public long MessagesCount { get; set; }
        }

        public class ChatScenarioInfo
        {
            public string Name { get; set; }
            public ChatScenarioContentInfo[] MessagesBySteps { get; set; }
        }

        public class MessagesPerChatScenario
        {
            public DateTime Date { get; set; }
            public ChatScenarioInfo[] MessagesInChatScenarios { get; set; }
        }

        public long TotalReceived { get; set; }
        public MessagesPerChatScenario[] MessagesPerChatScenarios { get; set; }
    }
}
