using Microsoft.EntityFrameworkCore;

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
            ZestPostContext context = new ZestPostContext();
            context.Database.Migrate();
        }
    }

}
