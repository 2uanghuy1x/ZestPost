namespace ZestPost.Base
{
    public class ViModelSync
    {
        public bool status { get; set; }
        public string data { get; set; }
        public string message { get; set; }
        public string his { get; set; }
        public string note { get; set; }
        public string hash { get; set; }
        public string userId { get; set; }
        public string expert_at { get; set; }
        public bool isChecking { get; set; }
        public int count_success { get; set; }
        public int code { get; set; }

        public ViModelSync()
        {
            status = false;
            data = null;
            message = null;
            note = null;
            hash = null;
            userId = null;
            isChecking = false;
            count_success = 0;
        }
    }

    public class StatusLink
    {
        public string Message { get; set; }
        public string Link { get; set; }
        public StatusPost Status { get; set; }
    }
}
