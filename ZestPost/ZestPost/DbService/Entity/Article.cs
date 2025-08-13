using System.ComponentModel.DataAnnotations;

namespace ZestPost.DbService
{
    public class Article
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
