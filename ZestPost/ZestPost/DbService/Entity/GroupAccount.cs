using ZestPost.Base.Model;

namespace ZestPost.DbService
{
    public class GroupAccount : FullAuditedEntity
    {
        public string UidFb { get; set; }
        public string Uid { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
    }
}
