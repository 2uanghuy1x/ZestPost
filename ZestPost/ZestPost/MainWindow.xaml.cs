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
            
            // Apply migrations at startup
            //_dbContext.Database.Migrate();

            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            try
            {
                // Ensure the cache folder exists
                string cacheFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WebView2Cache");
                Directory.CreateDirectory(cacheFolderPath);
                var webView2Environment = await CoreWebView2Environment.CreateAsync(null, cacheFolderPath);


                await WebView.EnsureCoreWebView2Async(webView2Environment);

                // Initialize the event handler service after WebView2 is ready
                _eventHandlerService = new EventHandlerService(_dbContext, WebView);

                WebView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
                
                // Allow debugging in development
                #if DEBUG
                    WebView.CoreWebView2.OpenDevToolsWindow();
                #endif

                // Navigate to the React app
                WebView.CoreWebView2.Navigate("http://localhost:3000");

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
