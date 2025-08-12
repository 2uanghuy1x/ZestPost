namespace ZestPost.DbService
{
    public class Article
    {
        public Guid Id { get; set; }
        public Guid IdCategory { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public long? NumMedia { get; set; }
        public string? PathMedia { get; set; }
        public string? Note { get; set; }
        public string? Type { get; set; }
    }
}
