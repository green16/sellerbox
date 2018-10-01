using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SellerBox.Models.Database
{
    public class VkUsers
    {
        [Key]
        public long IdVk { get; set; }

        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string LastName { get; set; }

        public string Link { get; set; }
        public string PhotoSquare50 { get; set; }
        public string PhotoOrig400 { get; set; }
        public bool? Sex { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime? Birthday { get; set; }

        public void Update(VkNet.Model.User user)
        {
            Birthday = SellerBox.Common.Helpers.VkHelper.BirtdayConvert(user.BirthDate);
            City = user.City?.Title;
            Country = user.Country?.Title;
            FirstName = user.FirstName;
            LastName = user.LastName;
            SecondName = user.Nickname;
            if (!string.IsNullOrWhiteSpace(user.Domain))
                Link = $"https://vk.com/{user.Domain}";
            else if (user.Id > 0)
                Link = $"https://vk.com/id{user.Id}";
            else
                Link = null;
            PhotoSquare50 = user.Photo50?.AbsoluteUri;
            PhotoOrig400 = user.Photo400Orig?.AbsoluteUri;
            Sex = user.Sex == VkNet.Enums.Sex.Unknown || user.Sex == VkNet.Enums.Sex.Deactivated ? null : (bool?)(user.Sex == VkNet.Enums.Sex.Male);
        }

        public static VkUsers FromUser(VkNet.Model.User user)
        {
            var result = new VkUsers() { IdVk = (int)user.Id };
            result.Update(user);
            return result;
        }
    }
}
