using Microsoft.EntityFrameworkCore;
using ZestPost.Base.Model;
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
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    // Áp dụng soft delete cho tất cả entity kế thừa FullAuditedEntity
        //    foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        //    {
        //        if (typeof(FullAuditedEntity).IsAssignableFrom(entityType.ClrType))
        //        {
        //            modelBuilder.Entity(entityType.ClrType)
        //                .HasQueryFilter(e => EF.Property<bool>(e, "IsDelete") == false);
        //        }
        //    }

        //    // Cấu hình List<string> LinkImg thành JSON
        //    modelBuilder.Entity<Article>()
        //        .Property(a => a.LinkImg)
        //        .HasConversion(
        //            v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
        //            v => JsonSerializer.Deserialize<List<string>>(v, new JsonSerializerOptions()) ?? new List<string>()
        //        )
        //        .HasColumnType("TEXT");
        //}

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Tự động cập nhật UpdatedAt khi sửa bản ghi
            foreach (var entry in ChangeTracker.Entries<FullAuditedEntity>()
                .Where(e => e.State == EntityState.Modified))
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

    }
}
