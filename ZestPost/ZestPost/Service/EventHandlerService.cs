using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows;
using ZestPost.Controller;
using ZestPost.DbService;
using ZestPost.DbService.Entity;

namespace ZestPost.Service
{
    public class EventHandlerService
    {
        private readonly AccountController _accountController;
        private readonly CategoryController _categoryController;
        private readonly ArticleController _articleController;
        private readonly WebView2 _webView;

        public EventHandlerService(ZestPostContext context, WebView2 webView)
        {
            _accountController = new AccountController(context);
            _categoryController = new CategoryController(context);
            _articleController = new ArticleController(context);
            _webView = webView;
        }

        public void HandleMessage(string message)
        {
            var json = JObject.Parse(message);
            var action = json["action"]?.ToString();
            var payload = json["payload"];

            if (string.IsNullOrEmpty(action)) return;

            // Use Application.Current.Dispatcher to ensure UI updates are on the main thread
            Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    switch (action)
                    {
                        // Account Actions
                        case "getAccounts":
                            var accounts = _accountController.GetAll();
                            await SendDataToWebView("accountsData", accounts);
                            break;
                        case "addAccount":
                            var newAccount = payload?.ToObject<AccountFB>();
                            _accountController.Add(newAccount);
                            await SendActionSuccess();
                            break;
                        case "updateAccount":
                            var updatedAccount = payload?.ToObject<AccountFB>();
                            _accountController.Update(updatedAccount);
                            await SendActionSuccess();
                            break;
                        case "deleteAccount":
                            var accountToDelete = payload?.ToObject<AccountFB>();
                            if(accountToDelete != null) _accountController.Delete(accountToDelete.Id);
                            await SendActionSuccess();
                            break;

                        // Category Actions
                        case "getCategories":
                            var categories = _categoryController.GetAll();
                            await SendDataToWebView("categoriesData", categories);
                            break;
                        case "addCategory":
                            var newCategory = payload?.ToObject<Category>();
                            _categoryController.Add(newCategory);
                            await SendActionSuccess();
                            break;
                        case "updateCategory":
                            var updatedCategory = payload?.ToObject<Category>();
                            _categoryController.Update(updatedCategory);
                            await SendActionSuccess();
                            break;
                        case "deleteCategory":
                             var categoryToDelete = payload?.ToObject<Category>();
                            if(categoryToDelete != null) _categoryController.Delete(categoryToDelete.Id);
                            await SendActionSuccess();
                            break;

                        // Article Actions
                        case "getArticles":
                            var articles = _articleController.GetAll();
                            await SendDataToWebView("articlesData", articles);
                            break;
                        case "addArticle":
                            var newArticle = payload?.ToObject<Article>();
                            _articleController.Add(newArticle);
                            await SendActionSuccess();
                            break;
                        case "updateArticle":
                            var updatedArticle = payload?.ToObject<Article>();
                            _articleController.Update(updatedArticle);
                            await SendActionSuccess();
                            break;
                        case "deleteArticle":
                            var articleToDelete = payload?.ToObject<Article>();
                            if(articleToDelete != null) _articleController.Delete(articleToDelete.Id);
                            await SendActionSuccess();
                            break;

                        default:
                            // Optionally handle unknown actions
                            break;
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions, maybe log them or show a message
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            });
        }

        private async Task SendDataToWebView(string action, object data)
        {
            var message = new { action, payload = data };
            var jsonMessage = JsonConvert.SerializeObject(message, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            await _webView.ExecuteScriptAsync($"window.chrome.webview.postMessage({jsonMessage}, '*')");
        }

        private async Task SendActionSuccess()
        {
            var message = new { action = "actionSuccess" };
            var jsonMessage = JsonConvert.SerializeObject(message);
            await _webView.ExecuteScriptAsync($"window.chrome.webview.postMessage({jsonMessage}, '*')");
        }
    }
}
