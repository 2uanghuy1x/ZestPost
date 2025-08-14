namespace ZestPost.Base
{
    public interface IAccountMail
    {
        public string? RecoveryMail { get; set; }
        public string? PassRecoveryMail { get; set; }
        public string? PassMail { get; set; }
    }
}
