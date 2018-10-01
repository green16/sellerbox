using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SellerBox.ViewModels
{
    public class ManageUserViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationScheme> OtherLogins { get; set; }

        public bool ShowRemoveButton { get; set; }

        [Display(Name = "Имя пользователя")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Электронная почта")]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Мобильный телефон")]
        public string PhoneNumber { get; set; }

        public string StatusMessage { get; set; }
    }
}
