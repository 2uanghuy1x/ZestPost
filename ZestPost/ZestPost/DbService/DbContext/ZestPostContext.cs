using Microsoft.EntityFrameworkCore;
namespace ZestPost.DbService
{
    public class ZestPostContext : DbContext
    {

        public DbSet<AccountFB> AccountFBs { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<GroupAccount> GroupAccounts { get; set; }
        public DbSet<HistoryAccount> HistoryAccounts { get; set; }
        public DbSet<PageAccount> PageAccounts { get; set; }

        public ZestPostContext()
        {
        }
        public ZestPostContext(DbContextOptions<ZestPostContext> options)
            : base(options)
        {
        }
    }
}
