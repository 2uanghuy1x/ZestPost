using Microsoft.Web.WebView2.Wpf;
using Newtonsoft.Json.Serialization;
using ZestPost.Controller;
using ZestPost.DbService.Entity;
using Newtonsoft.Json.Linq; // Add this using directive for JObject and JToken
using System.Threading; // Add this using directive for CancellationTokenSource
using System.Threading.Tasks; // Add this using directive for Task
using System.Windows; // Add this using directive for MessageBox and Application
using System; // Add this for Exception
using System.Collections.Generic; // Add this for List

namespace ZestPost.Service
{
    public class EventHandlerService
    {
        private readonly AccountController _accountController;
        private readonly CategoryController _categoryController;
        private readonly ArticleController _articleController;
        private readonly HistoryAccountController _historyAccountController;
        private readonly PageAccountController _pageAccountController;
        private readonly GroupAccountController _groupAccountController;
        private readonly PostArticleController _postArticleController;
        private readonly SettingApp _settingApp;
        private readonly WebView2 _webView;

        private CancellationTokenSource _currentCancellationTokenSource; // New field for cancellation

        public EventHandlerService(ZestPostContext context, WebView2 webView)
        {
            var cachingService = new CachingService();
            _accountController = new AccountController(context, cachingService);
            _categoryController = new CategoryController(context, cachingService);
            _articleController = new ArticleController(context, cachingService);
            _historyAccountController = new HistoryAccountController(context, cachingService);
            _pageAccountController = new PageAccountController(context, cachingService);
            _groupAccountController = new GroupAccountController(context, cachingService);
            _postArticleController = new PostArticleController(context, cachingService);
            _webView = webView;
        }

        public void HandleMessage(string message)
        {
            var json = JObject.Parse(message);
            var action = json["action"]?.ToString();
            var payload = json["payload"];

            if (string.IsNullOrEmpty(action)) return;

            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    if (action.Contains("mst"))
                    {
                        await HandleMasterActions(action, payload);
                    }
                    else
                    {
                        // Handle general actions, including cancellation
                        if (action == "postArticle" || action == "scanAccounts") // Add new actions here if they need cancellation
                        {
                            // If a previous operation is still running, cancel it first (optional, depends on desired behavior)
                            _currentCancellationTokenSource?.Cancel();
                            _currentCancellationTokenSource?.Dispose();
                            
                            _currentCancellationTokenSource = new CancellationTokenSource();
                            try
                            {
                                await HandleGeneralActions(action, payload, _currentCancellationTokenSource.Token);
                                // Send specific completion messages based on action
                                if (action == "postArticle")
                                {
                                    await SendDataToWebView("postingCompleted", null); // Notify FE that posting is completed
                                }
                                else if (action == "scanAccounts")
                                {
                                    await SendDataToWebView("scanningCompleted", null); // Notify FE that scanning is completed
                                }
                            }
                            catch (OperationCanceledException)
                            {
                                // Send specific stopped messages based on action
                                if (action == "postArticle")
                                {
                                    await SendDataToWebView("postingStopped", "Operation canceled by user."); // Notify FE that posting stopped
                                    MessageBox.Show("Hoạt động đăng bài đã bị hủy bỏ.", "Hủy bỏ");
                                }
                                else if (action == "scanAccounts")
                                {
                                    await SendDataToWebView("scanningStopped", "Operation canceled by user."); // Notify FE that scanning stopped
                                    MessageBox.Show("Hoạt động quét tài khoản đã bị hủy bỏ.", "Hủy bỏ");
                                }
                            }
                            finally
                            {
                                _currentCancellationTokenSource?.Dispose();
                                _currentCancellationTokenSource = null;
                            }
                        }
                        else if (action == "cancelPostArticle")
                        {
                            _currentCancellationTokenSource?.Cancel();
                            // No need for a message box here, FE will handle UI update
                        }
                        else if (action == "cancelScanAccounts") // New cancellation action
                        {
                            _currentCancellationTokenSource?.Cancel();
                        }
                        else
                        {
                            // For other general actions that don't involve cancellation
                            await HandleGeneralActions(action, payload, CancellationToken.None);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            });
        }

        private async Task HandleMasterActions(string action, JToken payload)
        {
            switch (action)
            {
                // Account Actions
                case "mstGetAccounts":
                    var accounts = _accountController.GetAll();
                    await SendDataToWebView("accountsData", accounts);
                    break;
                case "mstAddAccount":
                    var newAccount = payload?.ToObject<AccountFB>();
                    _accountController.Add(newAccount);
                    await SendDataToWebView("accountsData", _accountController.GetAll());
                    break;
                case "mstUpdateAccount":
                    var updatedAccount = payload?.ToObject<AccountFB>();
                    _accountController.Update(updatedAccount);
                    await SendDataToWebView("accountsData", _accountController.GetAll());
                    break;
                case "mstDeleteAccount":
                    if (payload != null && payload["id"] != null)
                    {
                        var accountId = payload["id"].ToObject<int>();
                        _accountController.Delete(accountId);
                        await SendDataToWebView("accountsData", _accountController.GetAll());
                    }
                    break;

                // Category Actions
                case "mstGetCategories":
                    var categories = _categoryController.GetAll();
                    await SendDataToWebView("categoriesData", categories);
                    break;
                case "mstAddCategory":
                    var newCategory = payload?.ToObject<Category>();
                    _categoryController.Add(newCategory);
                    await SendDataToWebView("categoriesData", _categoryController.GetAll());
                    break;
                case "mstUpdateCategory":
                    var updatedCategory = payload?.ToObject<Category>();
                    _categoryController.Update(updatedCategory);
                    await SendDataToWebView("categoriesData", _categoryController.GetAll());
                    break;
                case "mstDeleteCategory":
                    if (payload != null && payload["id"] != null)
                    {
                        var categoryId = payload["id"].ToObject<int>();
                        _categoryController.Delete(categoryId);
                        await SendDataToWebView("categoriesData", _categoryController.GetAll());
                    }
                    break;

                // Article Actions
                case "mstGetArticles":
                    var articles = _articleController.GetAll();
                    await SendDataToWebView("articlesData", articles);
                    break;
                case "mstAddArticle":
                    var newArticle = payload?.ToObject<Article>();
                    _articleController.Add(newArticle);
                    await SendDataToWebView("articlesData", _articleController.GetAll());
                    break;
                case "mstUpdateArticle":
                    var updatedArticle = payload?.ToObject<Article>();
                    _articleController.Update(updatedArticle);
                    await SendDataToWebView("articlesData", _articleController.GetAll());
                    break;
                case "mstDeleteArticle":
                    if (payload != null && payload["id"] != null)
                    {
                        var articleId = payload["id"].ToObject<int>();
                        _articleController.Delete(articleId);
                        await SendDataToWebView("articlesData", _articleController.GetAll());
                    }
                    break;

                // History Account Actions
                case "mstGetHistoryAccounts":
                    var historyAccounts = _historyAccountController.GetAll();
                    await SendDataToWebView("historyAccountsData", historyAccounts);
                    break;
                case "mstAddHistoryAccount":
                    var newHistoryAccount = payload?.ToObject<HistoryAccount>();
                    _historyAccountController.Add(newHistoryAccount);
                    await SendDataToWebView("historyAccountsData", _historyAccountController.GetAll());
                    break;
                case "mstUpdateHistoryAccount":
                    var updatedHistoryAccount = payload?.ToObject<HistoryAccount>();
                    _historyAccountController.Update(updatedHistoryAccount);
                    await SendDataToWebView("historyAccountsData", _historyAccountController.GetAll());
                    break;
                case "mstDeleteHistoryAccount":
                    if (payload != null && payload["id"] != null)
                    {
                        var historyAccountId = payload["id"].ToObject<int>();
                        _historyAccountController.Delete(historyAccountId);
                        await SendDataToWebView("historyAccountsData", _historyAccountController.GetAll());
                    }
                    break;

                // Page Account Actions
                case "mstGetPageAccounts":
                    var pageAccounts = _pageAccountController.GetAll();
                    await SendDataToWebView("pageAccountsData", pageAccounts);
                    break;
                case "mstAddPageAccount":
                    var newPageAccount = payload?.ToObject<PageAccount>();
                    _pageAccountController.Add(newPageAccount);
                    await SendDataToWebView("pageAccountsData", _pageAccountController.GetAll());
                    break;
                case "mstUpdatePageAccount":
                    var updatedPageAccount = payload?.ToObject<PageAccount>();
                    _pageAccountController.Update(updatedPageAccount);
                    await SendDataToWebView("pageAccountsData", _pageAccountController.GetAll());
                    break;
                case "mstDeletePageAccount":
                    if (payload != null && payload["id"] != null)
                    {
                        var pageAccountId = payload["id"].ToObject<int>();
                        _pageAccountController.Delete(pageAccountId);
                        await SendDataToWebView("pageAccountsData", _pageAccountController.GetAll());
                    }
                    break;

                // Group Account Actions
                case "mstGetGroupAccounts":
                    var groupAccounts = _groupAccountController.GetAll();
                    await SendDataToWebView("groupAccountsData", groupAccounts);
                    break;
                case "mstAddGroupAccount":
                    var newGroupAccount = payload?.ToObject<GroupAccount>();
                    _groupAccountController.Add(newGroupAccount);
                    await SendDataToWebView("groupAccountsData", _groupAccountController.GetAll());
                    break;
                case "mstUpdateGroupAccount":
                    var updatedGroupAccount = payload?.ToObject<GroupAccount>();
                    _groupAccountController.Update(updatedGroupAccount);
                    await SendDataToWebView("groupAccountsData", _groupAccountController.GetAll());
                    break;
                case "mstDeleteGroupAccount":
                    if (payload != null && payload["id"] != null)
                    {
                        var groupAccountId = payload["id"].ToObject<int>();
                        _groupAccountController.Delete(groupId);
                        await SendDataToWebView("groupAccountsData", _groupAccountController.GetAll());
                    }
                    break;
            }
        }

        private async Task HandleGeneralActions(string action, JToken payload, CancellationToken cancellationToken)
        {
            //var numThread = _settingApp.ThreadAction ?? 1;
            // ChromeBrowser chromeBrowser = new ChromeBrowser();
            // chromeBrowser.OpenChrome("ZestPost");
            // chromeBrowser.GotoURL("https://www.facebook.com/");
            // The browser operations might need to be within the cancellable scope too.
            // For now, assuming they are quick initializations.

            switch (action)
            {
                case "postArticle":
                    var accountsForPost = payload["accounts"]?.ToObject<List<AccountFB>>();
                    var generalConfigForPost = payload["general"];
                    var contentConfigForPost = payload["content"];

                    if (accountsForPost == null || generalConfigForPost == null || contentConfigForPost == null)
                    {
                        MessageBox.Show("Dữ liệu gửi không đúng định dạng cho hành động đăng bài.", "Lỗi dữ liệu");
                        return;
                    }

                    // Pass the separated data and cancellationToken to the controller
                    await _postArticleController.ProcessArticlePost(accountsForPost, generalConfigForPost, contentConfigForPost, cancellationToken);
                    break;

                case "scanAccounts": // New action case
                    var accountsForScan = payload["accounts"]?.ToObject<List<AccountFB>>();
                    var scanConfig = payload["config"]; // Assuming a 'config' property for scan data

                    if (accountsForScan == null || scanConfig == null)
                    {
                        MessageBox.Show("Dữ liệu gửi không đúng định dạng cho hành động quét tài khoản.", "Lỗi dữ liệu");
                        return;
                    }

                    // Call a new method in PostArticleController or a new specialized controller
                    await _postArticleController.ProcessScanAccounts(accountsForScan, scanConfig, cancellationToken);
                    break;

                // Add more cases for other general actions here, following the same pattern
                // case "yourNewAction":
                //     // Extract data from payload
                //     // Call corresponding controller method with data and cancellationToken
                //     break;
            }
        }

        private async Task SendDataToWebView(string action, object data)
        {
            var message = new { action, payload = data };
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            string jsonResponse = JsonConvert.SerializeObject(message, settings);
            _webView.CoreWebView2.PostWebMessageAsJson(jsonResponse);
        }

        private async Task SendActionSuccess()
        {
            var message = new { action = "actionSuccess" };
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            string jsonResponse = JsonConvert.SerializeObject(message, settings);
            _webView.CoreWebView2.PostWebMessageAsJson(jsonResponse);
        }
    }
}
