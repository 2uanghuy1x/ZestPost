
using ZestPost.Base.Model;

namespace ZestPost.DbService
{
    public class Category : FullAuditedEntity
    {
        public string? Name { get; set; }
        public string? Type { get; set; }

    }
}
