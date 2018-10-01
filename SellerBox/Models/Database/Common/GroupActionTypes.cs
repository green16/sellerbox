namespace SellerBox.Models.Database.Common
{
    public enum GroupActionTypes : byte
    {
        JoinGroup,
        LeaveGroup,
        BlockMessaging,
        AcceptMessaging,
        CancelMessaging,
        Blocked,
        Unblocked
    }
}
