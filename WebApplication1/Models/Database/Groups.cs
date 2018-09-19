using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.Database
{
    public class Groups
    {
        [Key]
        public long IdVk { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
        public string AccessToken { get; set; }
        public string CallbackConfirmationCode { get; set; }

        public virtual ICollection<Subscribers> Subscribers { get; set; }
        public virtual ICollection<BirthdayScenarios> BirthdayScenarios { get; set; }
        public virtual ICollection<GroupAdmins> GroupAdmins { get; set; }
        public virtual ICollection<BirthdayWallScenarios> BirthdayWallScenarios { get; set; }
    }
}
