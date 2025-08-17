using OpenQA.Selenium;
using ZestPost.Base.Extension;
using ZestPost.Service;

namespace ZestPost.Controller
{
    public class PostArticleController
    {
        private readonly ZestPostContext _context;
        private readonly CachingService _cache;

        public PostArticleController(ZestPostContext context, CachingService cache)
        {
            _context = context;
            _cache = cache;
        }

        private async Task ExecuteCancellableOperation(Func<Task> operation, string operationName, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Processing {operationName}...");
            try
            {
                await operation();
                Console.WriteLine($"{operationName} completed.");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"{operationName} was cancelled.");
                // Optionally, perform cleanup here if needed after cancellation
                throw; // Re-throw to be caught by EventHandlerService
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during {operationName}: {ex.Message}");
                throw; // Re-throw the exception
            }
        }

        public async Task ProcessArticlePost(List<AccountFB> accounts, JToken generalConfig, JToken contentConfig, CancellationToken cancellationToken)
        {
            await ExecuteCancellableOperation(async () =>
            {
                // Access your data
                Console.WriteLine($"Number of accounts for posting: {accounts?.Count}");
                Console.WriteLine($"General Config: {generalConfig}");
                Console.WriteLine($"Content Config: {contentConfig}");

                // Example: Simulate a long-running operation that can be cancelled
                for (int i = 0; i < 10; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested(); // Check for cancellation request
                    Console.WriteLine($"Processing post step {i + 1}...");
                    await Task.Delay(1000, cancellationToken); // Simulate work, delay with cancellation support
                }
            }, "article post", cancellationToken);
        }

        public async Task ProcessScanAccounts(List<AccountFB> accounts, JToken scanConfig, CancellationToken cancellationToken)
        {
            await ExecuteCancellableOperation(async () =>
            {
                // Access your data
                Console.WriteLine($"Number of accounts for scanning: {accounts?.Count}");
                Console.WriteLine($"Scan Config: {scanConfig}");

                // Example: Simulate a long-running scanning operation that can be cancelled
                for (int i = 0; i < 5; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested(); // Check for cancellation request
                    Console.WriteLine($"Processing scan step {i + 1}...");
                    await Task.Delay(2000, cancellationToken); // Simulate work, longer delay for scanning
                }
            }, "account scan", cancellationToken);
        }


        public ViModelSync PostArticleProfile(AccountFB acc, ChromeBrowser chromedriver, Article post, bool is_media, bool has_background = false, bool waitupload = false, int timewait = 1, bool tagFriend = false, bool isLocal_Image = true, CancellationToken token = default)
        {
            ViModelSync viresult = new ViModelSync() { status = false };
            bool flag = false;
            object lock_post = new object();
            List<string> lst_node = new List<string>();
            List<string> lstNodePost = new List<string>();
            string node = "";
            try
            {
                chromedriver.GoToUrl($"https://www.facebook.com/profile.php?id={acc.Uid}");
                chromedriver.DelayTime(1);
                if (token.IsCancellationRequested)
                {
                    goto lb_finish;
                }
                else
                {
                    //Tải ảnh xuống
                    List<string> lst_image = new List<string>();

                    lst_node = new List<string> { "[role=\"dialog\"] [aria-label=\"Close\"]", "[role=\"dialog\"] [aria-label=\"Đóng\"]" };
                    node = chromedriver.GetElementExistFromList(4, 0.5, lst_node);
                    if (!string.IsNullOrEmpty(node))
                    {
                        chromedriver.Click(SelectorType.ByCssSelector, node);
                        chromedriver.DelayTime(1);
                    }
                    string url = chromedriver.GetURL();
                    if (url.EndsWith("/"))
                    {
                        url = url.Substring(0, url.Length - 1);
                    }
                    string nodeComment = "//a[@href=\"" + url + "\"]/../div[@role=\"button\"]";
                    lst_node = new List<string> { nodeComment };
                    node = chromedriver.GetElementExistFromList(3, 0.5, lst_node);
                    if (!string.IsNullOrEmpty(node))
                    {
                        chromedriver.Click(SelectorType.ByXPath, node);
                        chromedriver.DelayTime(1);
                        //facebookservice.ChoosePublicWhenPost(chromedriver);
                        lst_node = new List<string> { "//form[@method=\"POST\"]//span[text()=\"Công khai\"]", "//form[@method=\"POST\"]//span[text()=\"Public\"]" };
                        node = chromedriver.GetElementExistFromList(3, 0.5, lst_node);
                        if (string.IsNullOrWhiteSpace(node))
                        {
                            lst_node = new List<string> { "//form[@method=\"POST\"]//div[contains(@aria-label,\"Chỉnh sửa quyền riêng tư\")]", "//form[@method=\"POST\"]//div[contains(@aria-label,\"Edit privacy\")]", "//span[text()=\"Công khai\"]", "//span[text()=\"Public\"]", "//input[@name=\"checkbox\"]", "//div[@aria-label=\"Lưu\"]", "//div[@aria-label=\"Save\"]" };
                            while (true)
                            {
                                node = chromedriver.GetElementExistFromList(3, 0.5, lst_node);
                                if (!string.IsNullOrWhiteSpace(node))
                                {
                                    lst_node.Remove(node);
                                    chromedriver.Click(SelectorType.ByXPath, node);
                                    chromedriver.DelayTime(2);
                                    continue;
                                }
                                break;
                            }
                        }
                    }

                    lstNodePost = new List<string> { "#scrollview [method=\"POST\"] [role=\"textbox\"] [aria-hidden=\"true\"]", "[method=\"POST\"] [role=\"textbox\"]", "form[method*=\"Post\"] [src*=\"images/composer\"]", "[aria-label^=\"Tag \"]", "[aria-label^=\"Photo/\"][role=\"button\"]", "[aria-label^=\"Ảnh/\"][role=\"button\"]" };
                    while (true)
                    {
                        if (token.IsCancellationRequested) goto lb_finish;
                        node = chromedriver.GetElementExistFromList(4, 2, lstNodePost);
                        if (!string.IsNullOrWhiteSpace(node))
                        {
                            lstNodePost.Remove(node);
                            if (lstNodePost.Count < 0) break;
                            switch (node)
                            {
                                case "#scrollview [method=\"POST\"] [role=\"textbox\"] [aria-hidden=\"true\"]" or "[method=\"POST\"] [role=\"textbox\"]":
                                    {
                                        if (token.IsCancellationRequested) goto lb_finish;
                                        int count_element = chromedriver.CountElement(4, 0.5, node);
                                        chromedriver.ClearTextWithBackSpace(4, node, count_element == 0 ? 0 : count_element - 1, 0, null, 0, 2, 0.1);
                                        chromedriver.DelayTime(1);

                                        //flag = HelperController.CopyToClipboardAndSendkeys(chromedriver, 4, node, count_element == 0 ? 0 : count_element - 1, content);
                                        //chromedriver.DelayTime(1);
                                        //if (!flag)
                                        //{
                                        flag = (chromedriver.SendKeys(SelectorType.ByCssSelector, node, post.Content, index: count_element == 0 ? 0 : count_element - 1)).IsOk();
                                        //}
                                        continue;
                                    }

                                case "form[method*=\"Post\"] [src*=\"images/composer\"]":
                                    if (has_background && is_media == false)
                                    {
                                        ChoseBackGround(chromedriver, lst_node, node, token);
                                    }
                                    continue;

                                case "[aria-label^=\"Tag \"]":
                                    if (tagFriend)
                                    {
                                        TagFriend(chromedriver, lst_node, node, token);
                                    }
                                    continue;

                                case "[aria-label^=\"Photo/\"][role=\"button\"]" or "[aria-label^=\"Ảnh/\"][role=\"button\"]":
                                    if (is_media && post.LinkImg.Count > 0)
                                    {
                                        ChoseImg(chromedriver, post, lst_node, lst_image, node, waitupload, timewait, token);
                                    }
                                    continue;
                            }

                        }
                        break;
                    }
                    flag = ClickSubmitPost(chromedriver, lst_node, node);
                    if (flag)
                    {
                        viresult.status = true;
                    }
                    chromedriver.DelayTime(2);


                }
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
        lb_finish:
            return viresult;
        }

        public ViModelSync ChoseBackGround(ChromeBrowser chromedriver, List<string> lst_node, string node, CancellationToken token = default)
        {
            ViModelSync viresult = new ViModelSync();
            viresult.status = false;
            chromedriver.DelayTime(1);
            if (chromedriver.CheckElements(4, "form[method*=\"Post\"] [src*=\"images/composer\"]"))
            {
                chromedriver.Click(SelectorType.ByCssSelector, "form[method*=\"Post\"] [src*=\"images/composer\"]");
                lst_node = new List<string> { "form[method*=\"Post\"] [aria-label =\"Background Options\"]", "form[method*=\"Post\"] [aria-label *=\"Tùy chọn phông\"]" };
                node = chromedriver.GetElementExistFromList(4, 0.5, lst_node);
                if (!string.IsNullOrEmpty(node))
                {
                    chromedriver.Click(SelectorType.ByCssSelector, node);
                }
            }

            lst_node = new List<string> { "form[method*=\"Post\"] [aria-label *=\"background image\"][role=\"button\"]", "form[method*=\"Post\"] [aria-label *=\"hình nền\"][role=\"button\"]" };
            node = chromedriver.GetElementExistFromList(4, 0.5, lst_node);
            if (!string.IsNullOrEmpty(node))
            {
                chromedriver.ClickRandom(SelectorType.ByCssSelector, node, 1);
            }

            lst_node = new List<string> { "form[method*=\"Post\"] [role=\"dialog\"] [aria-label =\"Back\"]", "form[method*=\"Post\"] [role=\"dialog\"] [aria-label =\"Quay lại\"]" };
            node = chromedriver.GetElementExistFromList(4, 0.5, lst_node);
            if (!string.IsNullOrEmpty(node))
            {
                viresult.status = true;
                chromedriver.Click(SelectorType.ByCssSelector, node);
            }
            return viresult;
        }
        public ViModelSync ChoseImg(ChromeBrowser chromedriver, Article post, List<string> lst_node, List<string> lst_image, string node, bool waitupload = false, int timewait = 1, CancellationToken token = default)
        {
            ViModelSync viresult = new ViModelSync() { status = false };
            chromedriver.DelayTime(2);
            //chromedriver.ExecuteScript("HTMLInputElement.prototype.click = function() { if(this.type !== 'file') HTMLElement.prototype.click.call(this); };");
            //send image

            for (int img = 0; img < post.LinkImg.Count; img++)
            {
                if (token.IsCancellationRequested) return viresult;
                string path_img = post.LinkImg[0];
                lst_image.Remove(path_img);
                if (string.IsNullOrEmpty(path_img) || File.Exists(path_img) == false)
                {
                    continue;
                }
                //checking after send image
                lst_node = new List<string> { "//form[contains(@method,\"POST\")] //*[contains(@aria-label,\"Tiếp\")]", "//form[contains(@method,\"POST\")] //*[contains(@aria-label,\"Next\")]", "form[method=\"Post\"] [aria-label*=\"Đăng\"][aria-disabled=\"true\"]", "form[method=\"Post\"] [aria-label*=\"Post\"][aria-disabled=\"true\"]" };
                node = chromedriver.GetElementExistFromList(4, 0.5, lst_node);
                int count_image = chromedriver.CountElement(4, 0.2, "form[method=\"Post\"] [role=\"group\"] img");
                if (count_image > 0 && !string.IsNullOrEmpty(node))
                {
                    lst_node = new List<string> { "form[method=\"Post\"] [aria-label*=\"Remove post\"]", "form[method=\"Post\"] [aria-label*=\"Gỡ file đính kèm\"]" };
                    node = chromedriver.GetElementExistFromList(4, 0.5, lst_node);
                    if (!string.IsNullOrEmpty(node))
                    {
                        viresult.status = false;
                        viresult.message = HelperCore.L("msg186");
                        return viresult;
                    }
                }
            }
            return viresult;
        }
        public ViModelSync TagFriend(ChromeBrowser chromedriver, List<string> lst_node, string node, CancellationToken token = default)
        {
            ViModelSync viresult = new ViModelSync();
            chromedriver.DelayTime(1.0);
            //chromedriver.ScrollPageIfNotExistOnScreen("document.querySelector('" + node + "')");
            chromedriver.DelayTime(1.0);
            //chromedriver.Click(SelectorType.ByCssSelector, "document.querySelector('" + node + "')", 0, 0, null, 0, 2);
            chromedriver.DelayRandom(2, 3);
            while (true)
            {
                if (token.IsCancellationRequested) goto lb_finish;
                //danh sách friend
                if (chromedriver.CheckElements(4, "input[role=\"textbox\"]"))
                {
                    bool flag5 = false;
                    for (int l = 0; l < 10; l++)
                    {

                        if (chromedriver.CheckElements(4, "[role=\"listbox\"] li[role=\"option\"]"))
                        {
                            if (chromedriver.CheckElements(3, "//*[text()=\"No people found\"]"))
                            {
                                break;
                            }
                            chromedriver.DelayTime(1.0);
                            continue;
                        }
                        flag5 = true;
                        break;
                    }
                    if (flag5)
                    {
                        chromedriver.DelayTime(2.0);
                        chromedriver.SendKeys(SelectorType.ByCssSelector, "input[role=\"textbox\"]", Keys.Enter);
                        chromedriver.DelayTime(0.5);
                        chromedriver.SendKeys(SelectorType.ByCssSelector, "input[role=\"textbox\"]", Keys.Enter);
                    }
                    chromedriver.CheckElements(4, "input[role=\"textbox\"]");
                }
            }
        lb_finish:
            return viresult;
        }
        public bool ClickSubmitPost(ChromeBrowser chromedriver, List<string> lst_node, string node)
        {
            lst_node = new List<string> { "[aria-label=\"Next\"]", "[aria-label=\"Tiếp\"]", "[aria-label=\"Đăng\"]", "[aria-label = \"Post\"]", "[aria-label=\"Submit\"]", "[aria-label=\"Gửi\"]" };
            while (true)
            {
                node = chromedriver.GetElementExistFromList(4, 2, lst_node);
                if (!string.IsNullOrWhiteSpace(node))
                {
                    lst_node.Remove(node);
                    chromedriver.Click(SelectorType.ByCssSelector, node);
                    chromedriver.DelayTime(2);
                    continue;
                }
                return true;
            }
        }
    }
}
