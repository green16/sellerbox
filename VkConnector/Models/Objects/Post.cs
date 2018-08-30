using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using VkConnector.Common.Converters;
using VkConnector.Models.Common;

namespace VkConnector.Models.Objects
{
    public class Post
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("owner_id")]
        public int IdWallOwner { get; set; }
        [JsonProperty("from_id")]
        public int IdAuthor { get; set; }
        [JsonProperty("created_by")]
        public int IdPublisher { get; set; }
        [JsonProperty("date"), JsonConverter(typeof(UnixTimeConverter))]
        public DateTime Dt { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("reply_owner_id")]
        public int IdReplyOwner { get; set; }
        [JsonProperty("reply_post_id")]
        public int IdReplyPost { get; set; }
        [JsonProperty("friends_only")]
        public bool IsFriendsOnly { get; set; }
        [JsonProperty("comments")]
        public CommentsInfo CommentsInfo { get; set; }
        [JsonProperty("likes")]
        public LikesInfo LikesInfo { get; set; }
        [JsonProperty("reposts")]
        public RepostsInfo RepostsInfo { get; set; }
        [JsonProperty("views")]
        public ViewsInfo ViewsInfo { get; set; }
        [JsonProperty("post_type")]
        public string PostType { get; set; }
        [JsonProperty("post_source")]
        public PostSource PostSource { get; set; }
        [JsonProperty("attachments")]
        public IEnumerable<Attachment> Attachments { get; set; }
        [JsonProperty("geo")]
        public Geo Geo { get; set; }
        [JsonProperty("signer_id")]
        public int IdSigner { get; set; }
        [JsonProperty("copy_history")]
        public IEnumerable<Post> Copy_history { get; set; }
        [JsonProperty("can_pin")]
        public bool CanCurrentUserPinPost { get; set; }
        [JsonProperty("can_delete")]
        public bool CanCurrentUserDeletePost { get; set; }
        [JsonProperty("can_edit")]
        public bool CanCurrentUserEditPost { get; set; }
        [JsonProperty("is_pinned")]
        public bool IsPinned { get; set; }
        [JsonProperty("marked_as_ads")]
        public bool IsAds { get; set; }
    }
}
