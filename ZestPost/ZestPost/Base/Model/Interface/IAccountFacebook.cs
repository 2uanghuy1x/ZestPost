namespace ZestPost.Base
{
    public interface IAccountFacebook
    {
        public string? Privatekey { get; set; }
        public string? Uid { get; set; }
        public string? Passmail { get; set; }
        public string Note { get; set; }
        public string? FbDtsg { get; set; }
        public string? TokenMail { get; set; }
    }
}
