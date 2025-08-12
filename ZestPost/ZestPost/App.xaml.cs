using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ZestPost
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            string dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            Directory.CreateDirectory(dataDirectory);
            string dbPath = Path.Combine(dataDirectory, "zestPost.db");

            var services = new ServiceCollection();
            services.AddDbContext<ZestPostContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));

            ServiceProvider = services.BuildServiceProvider();

            // Đảm bảo database được tạo nếu chưa tồn tại
            using (var scope = ServiceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ZestPostContext>();
                context.Database.EnsureCreated();
                context.Database.Migrate();
            }
        }
    }

}
