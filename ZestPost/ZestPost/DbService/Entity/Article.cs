using ZestPost.Base.Model;

namespace ZestPost.DbService
{
    public class Article : FullAuditedEntity
    {
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string timepost { get; set; }
        public List<string> LinkImg { get; set; }
    }
}
