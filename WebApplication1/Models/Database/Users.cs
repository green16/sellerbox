using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Models.Database
{
    public class Users : IdentityUser
    {
        public int IdVk { get; set; }
        public int IdCurrentGroup { get; set; }
    }
}
