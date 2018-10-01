using Microsoft.AspNetCore.Identity;

namespace SellerBox.Models.Database
{
    public class Users : IdentityUser
    {
        public long IdVk { get; set; }
        public long IdCurrentGroup { get; set; }
    }
}
