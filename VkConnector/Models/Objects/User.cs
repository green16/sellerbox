using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using VkConnector.Common.Converters;
using VkConnector.Models.Common;

namespace VkConnector.Models.Objects
{
    public class User
    {
        public override string ToString()
        {
            return $"{LastName} {FirstName} ({Id})";
        }

        #region Base properties

        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("first_name")]
        public string FirstName { get; set; }
        [JsonProperty("last_name")]
        public string LastName { get; set; }
        [JsonProperty("deactivated")]
        public string Deactivated { get; set; }
        [JsonProperty("hidden")]
        public bool IsHidden { get; set; }

        #endregion

        #region Additional properties

        [JsonProperty("about")]
        public string About { get; set; }
        [JsonProperty("activities")]
        public string Activities { get; set; }
        [JsonProperty("bdate"), JsonConverter(typeof(BirthdayConverter))]
        public DateTime? Birthday { get; set; }
        [JsonProperty("blacklisted")]
        public bool IsBlacklisted { get; set; }
        [JsonProperty("blacklisted_by_me")]
        public bool IsBlacklistedByMe { get; set; }
        [JsonProperty("books")]
        public string Books { get; set; }
        [JsonProperty("can_post")]
        public bool CanPost { get; set; }
        [JsonProperty("can_see_all_posts")]
        public bool CanSeeAllPosts { get; set; }
        [JsonProperty("can_see_audio")]
        public bool CanSeeAudio { get; set; }
        [JsonProperty("can_send_friend_request")]
        public bool CanSendFriendRequest { get; set; }
        [JsonProperty("can_write_private_message")]
        public bool CanWritePrivateMessage { get; set; }
        [JsonProperty("career")]
        public IEnumerable<Carrer> Carrer { get; set; }
        [JsonProperty("city")]
        public City City { get; set; }
        [JsonProperty("common_count")]
        public int CommonFriendsCount { get; set; }
        [JsonProperty("connections")]
        public Dictionary<string, string> Connections { get; set; }
        [JsonProperty("contacts")]
        public Contacts Contacts { get; set; }
        [JsonProperty("counters")]
        public Counters Counters { get; set; }
        [JsonProperty("country")]
        public Country Country { get; set; }
        [JsonProperty("crop_photo")]
        public CropPhoto CropPhoto { get; set; }
        [JsonProperty("domain")]
        public string Domain { get; set; }
        [JsonProperty("education")]
        public Education Education { get; set; }
        [JsonProperty("exports")]
        public JObject Exports { get; set; }
        [JsonProperty("first_name_nom")]
        public string FirstNameNom { get; set; }
        [JsonProperty("first_name_gen")]
        public string FirstNameGen { get; set; }
        [JsonProperty("first_name_dat")]
        public string FirstNameDat { get; set; }
        [JsonProperty("first_name_acc")]
        public string FirstNameAcc { get; set; }
        [JsonProperty("first_name_ins")]
        public string FirstNameIns { get; set; }
        [JsonProperty("first_name_abl")]
        public string FirstNameAbl { get; set; }
        [JsonProperty("followers_count")]
        public int FollowersCount { get; set; }
        [JsonProperty("friend_status")]
        public FriendStatus FriendStatus { get; set; }
        [JsonProperty("games")]
        public string Games { get; set; }
        [JsonProperty("has_mobile")]
        public bool HasMobile { get; set; }
        [JsonProperty("has_photo")]
        public bool HasPhoto { get; set; }
        [JsonProperty("home_town")]
        public string BirthTown { get; set; }
        [JsonProperty("interests")]
        public string Interests { get; set; }
        [JsonProperty("is_favorite")]
        public bool IsFavorite { get; set; }
        [JsonProperty("is_friend")]
        public bool IsFriend { get; set; }
        [JsonProperty("is_hidden_from_feed")]
        public bool IsHiddenFromFeed { get; set; }
        [JsonProperty("last_name_nom")]
        public string LastNameNom { get; set; }
        [JsonProperty("last_name_gen")]
        public string LastNameGen { get; set; }
        [JsonProperty("last_name_dat")]
        public string LastNameDat { get; set; }
        [JsonProperty("last_name_acc")]
        public string LastNameAcc { get; set; }
        [JsonProperty("last_name_ins")]
        public string LastNameIns { get; set; }
        [JsonProperty("last_name_abl")]
        public string LastNameAbl { get; set; }
        [JsonProperty("last_seen")]
        public LastSeen LastSeen { get; set; }
        [JsonProperty("lists")]
        public string Lists { get; set; }
        [JsonProperty("maiden_name")]
        public string MaidenName { get; set; }
        [JsonProperty("military")]
        public Military Military { get; set; }
        [JsonProperty("movies")]
        public string Movies { get; set; }
        [JsonProperty("music")]
        public string Music { get; set; }
        [JsonProperty("nickname")]
        public string NickName { get; set; }
        [JsonProperty("occupation")]
        public Employment Employment { get; set; }
        [JsonProperty("online")]
        public bool IsOnline { get; set; }
        [JsonProperty("personal")]
        public PersonalInfo PersonalInfo { get; set; }
        [JsonProperty("photo_50")]
        public Uri PhotoSquare50 { get; set; }
        [JsonProperty("photo_100")]
        public Uri PhotoSquare100 { get; set; }
        [JsonProperty("photo_200_orig")]
        public Uri PhotoOrig200 { get; set; }
        [JsonProperty("photo_200")]
        public Uri PhotoSquare200 { get; set; }
        [JsonProperty("photo_400_orig")]
        public Uri PhotoOrig400 { get; set; }
        [JsonProperty("photo_id")]
        public string IdPhoto { get; set; }
        [JsonProperty("photo_max")]
        public Uri PhotoSquareMax { get; set; }
        [JsonProperty("photo_max_orig")]
        public Uri PhotoOrigMax { get; set; }
        [JsonProperty("quotes")]
        public string Quotes { get; set; }
        [JsonProperty("relatives")]
        public IEnumerable<Relatives> Relatives { get; set; }
        [JsonProperty("relation")]
        public RelationTypes Relation { get; set; }
        [JsonProperty("relation_partner")]
        public RelationPartner RelationPartner { get; set; }
        [JsonProperty("schools")]
        public IEnumerable<School> Schools { get; set; }
        [JsonProperty("screen_name")]
        public string ScreenName { get; set; }
        [JsonProperty("sex")]
        public Sex Sex { get; set; }
        [JsonProperty("site")]
        public Uri Site { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("status_audio")]
        public JObject StatusAudio { get; set; }
        [JsonProperty("timezone")]
        public int TimeZone { get; set; }
        [JsonProperty("trending")]
        public bool HasTrending { get; set; }
        [JsonProperty("tv")]
        public string TVShows { get; set; }
        [JsonProperty("universities")]
        public IEnumerable<University> Universities { get; set; }
        [JsonProperty("verified")]
        public bool IsVerified { get; set; }
        [JsonProperty("wall_default")]
        public string WallDefault { get; set; }

        #endregion
    }
}
