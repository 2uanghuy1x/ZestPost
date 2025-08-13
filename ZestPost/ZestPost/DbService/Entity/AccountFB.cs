using System.ComponentModel.DataAnnotations;

namespace ZestPost.DbService
{
    public class AccountFB
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string? Privatekey { get; set; }
        public string? Token { get; set; }
        public string? Cookies { get; set; }
        public string? Uid { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? Passmail { get; set; }
        public string? Phone { get; set; }
        public string? Name { get; set; }
        public string? Birthday { get; set; }
        public string? Gender { get; set; }
        public string? Status { get; set; }
        public string? Mailrecovery { get; set; }
        public string? Useragent { get; set; }
        public string? Proxy { get; set; }
        public string? DatecreationAccount { get; set; }
        public string? DateimportAccount { get; set; }
        public string? Avatar { get; set; }
        public string? Note { get; set; }
        public string? FollowerCount { get; set; }
        public string? FollowingCount { get; set; }
        public string? LogPass { get; set; }
    }
}
