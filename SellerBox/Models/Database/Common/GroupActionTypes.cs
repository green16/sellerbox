namespace SellerBox.Models.Database.Common
{
    public enum GroupActionTypes : int
    {
        JoinGroup = 0,
        LeaveGroup,
        BlockMessaging,
        AcceptMessaging,
        CancelMessaging,
        Blocked,
        Unblocked
    }
}
