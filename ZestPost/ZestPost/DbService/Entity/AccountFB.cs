using System.ComponentModel.DataAnnotations;

namespace ZestPost.DbService
{
    public class AccountFB : IAccount, IAccountFacebook, IAccountMail
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
        public string? FbDtsg { get; set; }
        public string? Mailrecovery { get; set; }
        public string? MailrecoveryPass { get; set; }
        public string? Useragent { get; set; }
        public string? Proxy { get; set; }
        public string? DatecreationAccount { get; set; }
        public string? DateimportAccount { get; set; }
        public string? Avatar { get; set; }
        public string? Note { get; set; }
        public string? FollowerCount { get; set; }
        public string? FollowingCount { get; set; }
        public string? LogPass { get; set; }
        public string? TokenMail { get; set; }

        string IAccount.IdAccount { get => Uid; set => throw new NotImplementedException(); }
        string IAccount.Password { get => Password; set => Password = value; }
        string IAccount.Cookie { get => Cookies; set => Cookies = value; }
        string IAccount.Email { get => Email; set => Email = value; }
        string IAccount.Status { get => Status; set => throw new NotImplementedException(); }
        string IAccountFacebook.Uid { get => Uid; set => throw new NotImplementedException(); }
        string IAccountFacebook.Privatekey { get => Privatekey; set => Privatekey = value; }
        string IAccountFacebook.FbDtsg { get => FbDtsg; set => FbDtsg = value; }
        string IAccountFacebook.Passmail { get => Passmail; set => Passmail = value; }
        string IAccountFacebook.Note { get => Note; set => Note = value; }
        string IAccountMail.RecoveryMail { get => Mailrecovery; set => Mailrecovery = value; }
        string IAccountMail.PassRecoveryMail { get => MailrecoveryPass; set => MailrecoveryPass = value; }
        string IAccountMail.PassMail { get => Passmail; set => Passmail = value; }
    }
}
