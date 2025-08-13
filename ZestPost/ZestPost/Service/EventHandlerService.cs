using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows;
using ZestPost.Controller;
using ZestPost.DbService;
using ZestPost.DbService.Entity;
using ZestPost.Service;

namespace ZestPost.Service
{
    public class EventHandlerService
    {
        private readonly AccountController _accountController;
        private readonly CategoryController _categoryController;
        private readonly ArticleController _articleController;
        private readonly PostArticleController _postArticleController; // Add this
        private readonly WebView2 _webView;

        public EventHandlerService(ZestPostContext context, WebView2 webView)
        {
            var cachingService = new CachingService(); // Initialize CachingService here

            _accountController = new AccountController(context, cachingService);
            _categoryController = new CategoryController(context, cachingService);
            _articleController = new ArticleController(context, cachingService);
            _postArticleController = new PostArticleController(context, cachingService); // Initialize PostArticleController
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
                            // Assuming payload.Id is available for deletion
                            if (payload != null && payload["id"] != null)
                            {
                                var accountId = payload["id"].ToObject<int>();
                                _accountController.Delete(accountId);
                                await SendActionSuccess();
                            }
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
                            // Assuming payload.Id is available for deletion
                            if (payload != null && payload["id"] != null)
                            {
                                var categoryId = payload["id"].ToObject<int>();
                                _categoryController.Delete(categoryId);
                                await SendActionSuccess();
                            }
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
                            // Assuming payload.Id is available for deletion
                            if (payload != null && payload["id"] != null)
                            {
                                var articleId = payload["id"].ToObject<int>();
                                _articleController.Delete(articleId);
                                await SendActionSuccess();
                            }
                            break;

                        // Post Article Action (Placeholder)
                        case "postArticle":
                            _postArticleController.ProcessArticlePost(payload);
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
