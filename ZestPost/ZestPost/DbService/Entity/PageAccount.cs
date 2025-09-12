using ZestPost.Base.Model;

namespace ZestPost.DbService
{
    public class PageAccount : FullAuditedEntity
    {
        public Guid Id { get; set; }
        public string Uid { get; set; }
        public string UidFb { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public int? Groups { get; set; }
        public int? Shared { get; set; }
        public string Time { get; set; }
        public string LastTimeAction { get; set; }
        public string LastAction { get; set; }
    }
}
