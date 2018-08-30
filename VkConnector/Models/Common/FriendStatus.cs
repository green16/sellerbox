namespace VkConnector.Models.Common
{
    public enum FriendStatus : byte
    {
        NotFriend = 0,
        OutboundRequest = 1,
        IncomingRequest = 2,
        Friend = 3
    }
}
