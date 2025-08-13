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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
                if (!Directory.Exists(dataDirectory))
                {
                    Directory.CreateDirectory(dataDirectory);
                }
                string dbPath = Path.Combine(dataDirectory, "zestPost.db");
                // Configure the database to be used (e.g., SQLite)
                // Assumes a database file named "zestpost.db" in the application's root directory.
                optionsBuilder.UseSqlite($"Data Source={dbPath}");

            }
        }

    }
}
