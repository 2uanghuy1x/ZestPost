using ZestPost.Base.Model;

namespace ZestPost.DbService
{
    public class HistoryAccount : FullAuditedEntity
    {
        public Guid Id { get; set; }
        public string Uid { get; set; }
        public string? Link { get; set; }
        public string? Timepost { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public long? Numpicture { get; set; }
        public string? Path { get; set; }
        public string? Note { get; set; }
        public string? Type { get; set; }
    }
}
