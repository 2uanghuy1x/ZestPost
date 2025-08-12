using ZestPost.Service;

namespace ZestPost
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly EventHandlerService _eventHandlerService;
        public MainWindow()
        {
            InitializeComponent();
            //Loaded += MainWindow_Loaded;
            _eventHandlerService = new EventHandlerService();
            InitializeWebView();
        }
        private async void InitializeWebView()
        {
            await WebView.EnsureCoreWebView2Async(null);

            WebView.CoreWebView2.WebMessageReceived += (s, e) =>
            {
                WebView.CoreWebView2.WebMessageReceived += (sender, args) =>
                {
                    try
                    {
                        string message = args.WebMessageAsJson;
                        var innerJson = JsonConvert.DeserializeObject<string>(args.WebMessageAsJson);
                        var eventData = JsonConvert.DeserializeObject<EventData>(innerJson);
                        if (eventData == null)
                        {
                            throw new ArgumentNullException(nameof(eventData));
                            // Hoặc xử lý theo cách khác, như return hoặc ghi log
                        }
                        _eventHandlerService.HandleEvent(eventData);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi nhận dữ liệu: {ex.Message}");
                    }
                };
            };
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Await the initialization of WebView2
                await WebView.EnsureCoreWebView2Async(null);

                // Subscribe to the WebMessageReceived event
                WebView.CoreWebView2.WebMessageReceived += (sender, args) =>
                {
                    var message = args.WebMessageAsJson;
                    MessageBox.Show($"Received from React: {message}");
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing WebView2: {ex.Message}");
            }
        }
    }
}