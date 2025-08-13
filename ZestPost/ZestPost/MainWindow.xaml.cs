using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using System.Windows;
using ZestPost.DbService;
using ZestPost.Service;

namespace ZestPost
{
    public partial class MainWindow : Window
    {
        private EventHandlerService _eventHandlerService;
        private readonly ZestPostContext _dbContext;

        public MainWindow()
        {
            InitializeComponent();
            
            // Initialize DbContext
            _dbContext = new ZestPostContext();
            
            // Apply migrations at startup (uncomment if you have migrations set up)
            // _dbContext.Database.Migrate();

            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            try
            {
                // Ensure the cache folder exists
                string cacheFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WebView2Cache");
                if (!Directory.Exists(cacheFolderPath))
 {
     Directory.CreateDirectory(cacheFolderPath);
 }
                // Set CreationProperties BEFORE calling EnsureCoreWebView2Async
                // This is the correct way to provide a custom environment
                WebView.CreationProperties = new CoreWebView2CreationProperties
                {
                    UserDataFolder = cacheFolderPath
                };

                // Call EnsureCoreWebView2Async without arguments or with null.
                // It will now use the CreationProperties set above.
                await WebView.EnsureCoreWebView2Async(null);

                // Initialize the event handler service after WebView2 is ready
                if (WebView.CoreWebView2 != null)
                {
                    _eventHandlerService = new EventHandlerService(_dbContext, WebView);

                    WebView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
                    
                    // Allow debugging in development
                    #if DEBUG
                        WebView.CoreWebView2.OpenDevToolsWindow();
                    #endif

                    // Navigate to the React app AFTER WebView2 is initialized
                    WebView.CoreWebView2.Navigate("http://localhost:3000");
                }
                else
                {
                     MessageBox.Show("WebView2 CoreWebView2 could not be initialized.", "Initialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not initialize WebView2: {ex.Message}", "Initialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            string jsonMessage = args.TryGetWebMessageAsString();
            if (!string.IsNullOrEmpty(jsonMessage))
            {
                _eventHandlerService.HandleMessage(jsonMessage);
            }
        }
    }
}
