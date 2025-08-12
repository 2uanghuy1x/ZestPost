namespace ZestPost.Base
{
    public class TokenMailSync
    {
        public bool status { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string expired { get; set; }
        public string time { get; set; }
        public string scope { get; set; }
        public string Message { get; set; }

        public TokenMailSync()
        {
            status = false;
            access_token = null;
            refresh_token = null;
            email = null;
            password = null;
            expired = null;
            scope = null;
        }
    }
}
