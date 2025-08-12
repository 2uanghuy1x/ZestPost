namespace ZestPost.Base
{
    public interface IAccount
    {
        public string IdAccount { get; set; }

        public string Proxy { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string Cookie { get; set; }

        public string Status { get; set; }
    }
}
