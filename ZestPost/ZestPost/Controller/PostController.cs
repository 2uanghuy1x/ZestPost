using OpenQA.Selenium;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using ZestPost.Base.Extension;
using ZestPost.Service;

namespace ZestPost.Controllers
{
    public class PostController
    {

        private readonly ZestPostContext _context;
        private readonly CachingService _cache;
        private object _syncThread = new object();

        public PostController(ZestPostContext context, CachingService cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<Result> PostByProfile(object payload)
        {
            // Implementation for posting by profile
            return Result.Ok("PostByProfile successful");
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
        public async Task<Result> PostReelByProfile(object payload)
        {
            // Implementation for posting by profile
            return Result.Ok("PostByProfile successful");
        }
        public ViModelSync ReelFacebookPageProfile(ChromeBrowser chromedriver, AccountFB acc, string url_video, string bio, int mintime, int maxtime, int timeupload, CancellationToken token = default, Button btn = null)
        {
            ViModelSync result = new ViModelSync() { status = false };
            bool flag = false;
            object lock_Clipboard = new object();
            List<string> lst_node = new List<string>();
            string node = "";
            try
            {
                //?surface=ADDL_PROFILE_PLUS
                chromedriver.GoToUrl("https://www.facebook.com/reels/create/");
                chromedriver.DelayRandom(1, 3);
                flag = chromedriver.GetElement(SelectorType.ByCssSelector, "[method=\"POST\"] [type='file']").IsOk();
                if (flag)
                {
                    lock (_syncThread)
                    {
                        chromedriver.Click(SelectorType.ByCssSelector, "form [role*=\"complementary\"]");
                        flag = chromedriver.SendKeys(SelectorType.ByCssSelector, "[method=\"POST\"] [type=\"file\"]", url_video).IsOk();
                        chromedriver.DelayTime(2);
                    }
                    if (flag)
                    {
                        if (CheckHanCheReels(chromedriver))
                        {
                            result.note = "Page bị hạn chế!";
                            goto Lb_finish;
                        }
                        lst_node = new List<string> { "//*[contains(text(),\"cannot exceed 60 second\")]", "//*[contains(text(),\"không được dài quá 60 giây\")]" };
                        node = chromedriver.GetElementExistFromList(3, 0.5, lst_node);
                        if (!string.IsNullOrEmpty(node))
                        {
                            result.note = HelperCore.L("msg595");
                            goto Lb_finish;
                        }

                        lst_node = new List<string> { "[method =\"POST\"] [aria-label*=\"1/3\"]", "[method =\"POST\"] [aria-label*=\"1 of 3\"]" };
                        node = chromedriver.GetElementExistFromList(4, 0.5, lst_node);
                        if (!string.IsNullOrEmpty(node))
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                if (token.IsCancellationRequested)
                                    goto Lb_finish;
                                //Bước 1

                                node = chromedriver.GetElementExistFromList(4, 0.5, lst_node);
                                if (!string.IsNullOrEmpty(node))
                                {
                                    lst_node = new List<string> { "[method= \"POST\"] [role=\"button\"][aria-label=\"Tiếp\"]", "[method= \"POST\"] [role=\"button\"][aria-label=\"Next\"]" };
                                    node = chromedriver.GetElementExistFromList(4, 0.5, lst_node);
                                    if (!string.IsNullOrEmpty(node))
                                    {
                                        flag = chromedriver.Click(SelectorType.ByCssSelector, node).IsOk();
                                        chromedriver.DelayTime(1);
                                    }
                                }

                                lst_node = new List<string> { "//*[contains(text(),\"cannot exceed 60 second\")]", "//*[contains(text(),\"không được dài quá 60 giây\")]" };
                                node = chromedriver.GetElementExistFromList(3, 0.5, lst_node);
                                if (!string.IsNullOrEmpty(node))
                                {
                                    result.note = HelperCore.L("msg595");
                                    goto Lb_finish;
                                }
                                //Bước 2
                                lst_node = new List<string> { "[method=\"POST\"] [aria-label*=\"2/3\"]", "[method=\"POST\"] [aria-label*=\"2 of 3\"]" };
                                node = chromedriver.GetElementExistFromList(4, 0.5, lst_node);
                                if (!string.IsNullOrEmpty(node))
                                {
                                    lst_node = new List<string> { "[method= \"POST\"] [role=\"button\"][aria-label=\"Tiếp\"]", "[method= \"POST\"] [role=\"button\"][aria-label=\"Next\"]" };
                                    node = chromedriver.GetElementExistFromList(4, 0.5, lst_node);
                                    if (!string.IsNullOrEmpty(node))
                                    {
                                        int count = chromedriver.CountElement(4, 0.5, node);
                                        flag = chromedriver.Click(SelectorType.ByCssSelector, node, index: count > 0 ? count - 1 : 0).IsOk();
                                        chromedriver.DelayTime(1);
                                    }
                                }

                                //Bước 3
                                lst_node = new List<string> { "[method =\"POST\"] [aria-label*=\"3/3\"]", "[method =\"POST\"] [aria-label*=\"3 of 3\"]" };
                                node = chromedriver.GetElementExistFromList(4, 0.5, lst_node);
                                if (!string.IsNullOrEmpty(node))
                                {

                                    flag = chromedriver.SendKeys(SelectorType.ByCssSelector, "[method=\"POST\"] [role=\"textbox\"]", bio).IsOk();
                                    chromedriver.DelayTime(1);

                                    lst_node = new List<string> { "[method=\"POST\"] [aria-label=\"Đăng\"][tabindex=\"0\"]", "[method=\"POST\"] [aria-label=\"Post\"][tabindex=\"0\"]", "[method=\"POST\"] [aria-label=\"Publish\"][tabindex=\"0\"]" };
                                    node = chromedriver.GetElementExistFromList(4, 0.5, lst_node);
                                    if (!string.IsNullOrEmpty(node))
                                    {
                                        flag = chromedriver.Click(SelectorType.ByCssSelector, node).IsOk();
                                        chromedriver.DelayTime(4.0);
                                        if (flag)
                                        {
                                            if (chromedriver.GetURL().Contains("reel"))
                                            {
                                                int upload = 30;
                                                while (upload > 0)
                                                {
                                                    if (token.IsCancellationRequested)
                                                        break;
                                                    upload--;
                                                    if (IsSuccess(chromedriver))
                                                    {
                                                        result.status = true;
                                                        goto Lb_finish;
                                                    }
                                                    else
                                                    {
                                                        chromedriver.DelayTime(0.3);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                chromedriver.GotoURL("https://www.facebook.com/profile.php");
                                                chromedriver.DelayTime(1.0);
                                                if (chromedriver.GetAllElement(SelectorType.ByCssSelector, "div[id*=\":R\"] [href*=\"/reel/\"]").IsOk())
                                                {
                                                    result.status = true;
                                                    goto Lb_finish;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            lst_node = new List<string> { "[method =\"POST\"] [aria-label*=\"1/2\"]", "[method =\"POST\"] [aria-label*=\"1 of 2\"]" };
                            node = chromedriver.GetElementExistFromList(4, 0.5, lst_node);
                            if (!string.IsNullOrEmpty(node))
                            {
                                for (int i = 0; i < 3; i++)
                                {
                                    if (token.IsCancellationRequested)
                                        goto Lb_finish;
                                    //Bước 1
                                    flag = chromedriver.GetAllElement(SelectorType.ByCssSelector, "[method=\"POST\"] [aria-label*=\"1/2\"]").IsOk();
                                    if (flag)
                                    {
                                        lst_node = new List<string> { "[method= \"POST\"] [role=\"button\"][aria-label=\"Tiếp\"]", "[method= \"POST\"] [role=\"button\"][aria-label=\"Next\"]" };
                                        node = chromedriver.GetElementExistFromList(4, 0.5, lst_node);
                                        if (!string.IsNullOrEmpty(node))
                                        {
                                            flag = chromedriver.Click(SelectorType.ByCssSelector, node).IsOk();
                                            chromedriver.DelayTime(1);
                                        }
                                    }

                                    lst_node = new List<string> { "//*[contains(text(),\"cannot exceed 60 second\")]", "//*[contains(text(),\"không được dài quá 60 giây\")]" };
                                    node = chromedriver.GetElementExistFromList(3, 0.5, lst_node);
                                    if (!string.IsNullOrEmpty(node))
                                    {
                                        result.note = HelperCore.L("msg595");
                                        goto Lb_finish;
                                    }

                                    //Bước 2 Send mô tả
                                    flag = chromedriver.GetAllElement(SelectorType.ByCssSelector, "[method=\"POST\"] [role=\"textbox\"]").IsOk();
                                    if (flag)
                                    {
                                        chromedriver.ClearTextWithBackSpace(4, "[method=\"POST\"] [role=\"textbox\"]", 0, 0, null, 2, 1);
                                        chromedriver.DelayTime(0.5);
                                        flag = chromedriver.SendKeys(SelectorType.ByCssSelector, "[method=\"POST\"] [role=\"textbox\"]", bio).IsOk();
                                        chromedriver.DelayTime(1);
                                    }

                                    //Bước 3 Đăng bài
                                    lst_node = new List<string> { "[method=\"POST\"] [aria-label=\"Đăng\"][tabindex=\"0\"]", "[method=\"POST\"] [aria-label=\"Post\"][tabindex=\"0\"]" };
                                    node = chromedriver.GetElementExistFromList(4, 0.5, lst_node);
                                    if (!string.IsNullOrEmpty(node))
                                    {
                                        flag = chromedriver.Click(SelectorType.ByCssSelector, node).IsOk();
                                        chromedriver.DelayTime(4);
                                        if (flag)
                                        {
                                            if (chromedriver.GetURL().Contains("reel"))
                                            {
                                                int upload = 30;
                                                while (upload > 0)
                                                {
                                                    if (token.IsCancellationRequested)
                                                        break;
                                                    upload--;
                                                    if (IsSuccess(chromedriver))
                                                    {
                                                        result.status = true;
                                                        goto Lb_finish;
                                                    }
                                                    else
                                                    {
                                                        chromedriver.DelayTime(0.3);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                chromedriver.GotoURL("https://www.facebook.com/profile.php");
                                                chromedriver.DelayTime(1.0);
                                                if (chromedriver.GetAllElement(SelectorType.ByCssSelector, "div[id*=\":R\"] [href*=\"/reel/\"]").IsOk())
                                                {
                                                    result.status = true;
                                                    goto Lb_finish;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (chromedriver.GetAllElement(SelectorType.ByCssSelector, "[method =\"POST\"] [style=\"width: calc(50%);\"]").IsOk())
                                {
                                    for (int i = 0; i < 3; i++)
                                    {
                                        if (token.IsCancellationRequested)
                                            goto Lb_finish;
                                        //Bước 1
                                        lst_node = new List<string> { "[method= \"POST\"] [role=\"button\"][aria-label=\"Tiếp\"]", "[method= \"POST\"] [role=\"button\"][aria-label=\"Next\"]" };
                                        node = chromedriver.GetElementExistFromList(4, 0.5, lst_node);
                                        if (!string.IsNullOrEmpty(node))
                                        {
                                            int count = chromedriver.CountElement(4, 0.5, node);
                                            flag = chromedriver.Click(SelectorType.ByCssSelector, node, index: count > 0 ? count - 1 : 0).IsOk();
                                            chromedriver.DelayTime(1);
                                        }

                                        lst_node = new List<string> { "//*[contains(text(),\"cannot exceed 60 second\")]", "//*[contains(text(),\"không được dài quá 60 giây\")]" };
                                        node = chromedriver.GetElementExistFromList(3, 0.5, lst_node);
                                        if (!string.IsNullOrEmpty(node))
                                        {
                                            result.note = HelperCore.L("msg595");
                                            goto Lb_finish;
                                        }

                                        //Bước 2
                                        flag = chromedriver.SendKeys(SelectorType.ByCssSelector, "[method=\"POST\"] [role=\"textbox\"]", bio).IsOk();
                                        chromedriver.DelayTime(1);

                                        lst_node = new List<string> { "[method=\"POST\"] [aria-label=\"Đăng\"][tabindex=\"0\"]", "[method=\"POST\"] [aria-label=\"Post\"][tabindex=\"0\"]", "[method=\"POST\"] [aria-label=\"Publish\"][tabindex=\"0\"]" };
                                        node = chromedriver.GetElementExistFromList(4, 0.5, lst_node);
                                        if (!string.IsNullOrEmpty(node))
                                        {
                                            flag = chromedriver.Click(SelectorType.ByCssSelector, node).IsOk();
                                            chromedriver.DelayTime(4);
                                            if (flag)
                                            {
                                                if (chromedriver.GetURL().Contains("reel"))
                                                {
                                                    int upload = 30;
                                                    while (upload > 0)
                                                    {
                                                        if (token.IsCancellationRequested)
                                                            break;
                                                        upload--;
                                                        if (IsSuccess(chromedriver))
                                                        {
                                                            result.status = true;
                                                            goto Lb_finish;
                                                        }
                                                        else
                                                        {
                                                            chromedriver.DelayTime(0.3);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    chromedriver.GotoURL("https://www.facebook.com/profile.php");
                                                    chromedriver.DelayTime(1.0);
                                                    if (chromedriver.GetAllElement(SelectorType.ByCssSelector, "div[id*=\":R\"] [href*=\"/reel/\"]").IsOk())
                                                    {
                                                        goto Lb_finish;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            Lb_finish:;
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return result;
        }
        public async Task<Result> PostByPage(object payload)
        {
            // Implementation for posting by page
            return Result.Ok("PostByPage successful");
        }
        public async Task<Result> PostByGroup(object payload)
        {
            // Implementation for posting by group
            return Result.Ok("PostByGroup successful");
        }

        public bool CheckHanCheReels(ChromeBrowser chromedriver)
        {
            try
            {
                List<string> lst_node = new List<string> { "//*[@role=\"dialog\"]//*[contains(text(), \"Tài khoản của bạn bị hạn chế\")]", "//*[@role=\"dialog\"]//*[contains(text(), \"Your account is restricted\")]" };
                string node = chromedriver.GetElementExistFromList(3, 0.3, lst_node);
                if (!string.IsNullOrEmpty(node)) return true;
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return false;
        }

        private bool IsSuccess(ChromeBrowser chromedriver)
        {
            bool status = false;
            try
            {
                List<string> lst_url = new List<string> { "s=reel_composer" };
                if (HelperSync.CheckStringContainKeyword(chromedriver.GetURL(), lst_url))
                {
                    status = true;
                }
                else if (chromedriver.CheckElements(4, "[data-pagelet=\"Reels\"]", 0, 0, null, 0, 2, 0.1))
                {
                    status = true;
                }
                else
                {
                    string pattern = @"(\d{6,10})";
                    Match regex_math = Regex.Match(chromedriver.GetURL(), pattern);
                    if (regex_math.Success)
                    {
                        status = true;
                    }
                }
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return status;
        }
    }
}
