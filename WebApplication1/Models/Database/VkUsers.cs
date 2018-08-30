using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models.Database
{
    public class VkUsers
    {
        [Key]
        public int IdVk { get; set; }
        
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

        public void Update(VkConnector.Models.Objects.User user)
        {
            Birthday = user.Birthday;
            City = user.City?.Name;
            Country = user.Country?.Name;
            FirstName = user.FirstName;
            LastName = user.LastName;
            SecondName = user.NickName;
            Link = $"{VkConnector.Methods.Base.VkUrl}/{user.Domain}";
            PhotoSquare50 = user.PhotoSquare50?.AbsoluteUri;
            PhotoOrig400 = user.PhotoOrig400?.AbsoluteUri;
            Sex = user.Sex == VkConnector.Models.Common.Sex.Unknown ? null : (bool?)(user.Sex == VkConnector.Models.Common.Sex.Male);
        }

        public static VkUsers FromUser(VkConnector.Models.Objects.User user)
        {
            var result = new VkUsers() { IdVk = user.Id };
            result.Update(user);
            return result;
        }
    }
}
