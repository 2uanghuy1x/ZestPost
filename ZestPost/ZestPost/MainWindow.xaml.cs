using System;
using System.Windows;
using ZestPost.Controller;
using ZestPost.DbService;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace ZestPost
{
    public partial class MainWindow : Window
    {
        private readonly AccountController _accountController;

        public MainWindow()
        {
            InitializeComponent();
            _accountController = AccountController.Instance();
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            await WebView.EnsureCoreWebView2Async(null);
            // Thiết lập đường dẫn đến ứng dụng React của bạn
            // Đảm bảo ứng dụng React đang chạy trên port 3000
            WebView.CoreWebView2.Navigate("http://localhost:3000"); 
            WebView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
        }

        private async void CoreWebView2_WebMessageReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs args)
        {
            try
            {
                string jsonMessage = args.TryGetWebMessageAsString();
                var message = JObject.Parse(jsonMessage);
                string action = message["action"]?.ToString();
                JToken payload = message["payload"];

                switch (action)
                {
                    case "getAccounts":
                        var accounts = _accountController.GetListAccount();
                        SendDataToReact("accountsData", accounts);
                        break;

                    case "addAccount":
                        var newAccount = payload?.ToObject<AccountFB>();
                        if (newAccount != null)
                        {
                            newAccount.Id = Guid.NewGuid();
                            if (_accountController.InsertAccount(newAccount))
                            {
                                SendDataToReact("actionSuccess", null);
                            }
                        }
                        break;

                    case "updateAccount":
                        var updatedAccount = payload?.ToObject<AccountFB>();
                        if (updatedAccount != null)
                        {
                            if (await _accountController.UpdateAccountAsync(updatedAccount))
                            {
                                SendDataToReact("actionSuccess", null);
                            }
                        }
                        break;

                    case "deleteAccount":
                        var deleteAccount = payload?.ToObject<AccountFB>();
                        if (deleteAccount != null)
                        {
                            if (_accountController.DeleteAccount(deleteAccount))
                            {
                                SendDataToReact("actionSuccess", null);
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                 // Gửi lỗi về React để debug nếu cần
                 SendDataToReact("error", ex.Message);
            }
        }

        private void SendDataToReact(string action, object payload)
        {
            var data = new { action, payload };
            string jsonResponse = JsonConvert.SerializeObject(data);
            WebView.CoreWebView2.PostWebMessageAsJson(jsonResponse);
        }
    }
}
