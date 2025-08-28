using ZestPost.Base.Extension;

namespace ZestPost.Controller
{
    public class MessageController
    {
        // POST: api/Comment/CommentByProfile
        public async Task<Result> MessageByProfile(object payload)
        {
            // Implementation for commenting by profile
            return Result.Ok("CommentByProfile successful");
        }

        // POST: api/Comment/CommentByPage
        public async Task<Result> MessageByPage(object payload)
        {
            // Implementation for commenting by page
            return Result.Ok("CommentByPage successful");
        }

        public ViModelSync SendMessageWWW(ChromeBrowser chromedriver, AccountFB acc, string friendid, List<Article> _lstPost, int mintime, int maxtime, CancellationToken token = default, int delay_upload = 2)
        {
            List<string> lst_node = new List<string>();
            Random rd = new Random();
            ViModelSync result = new ViModelSync();
            bool scan_send_message = false;
            result.status = false;
            string node = null;
            bool flag = false;
            try
            {
                //AccountInforDataSync account_infor = data_profile.getBasicInforAccountDom(chromedriver, friendid, token);
                //if (chromedriver.GetURL().Contains(friendid) && chromedriver.GetURL().Contains("messages/e2ee"))
                //{
                //    chromedriver.DelayTime(1);
                //}
                //else
                //{
                chromedriver.GotoURL("https://www.facebook.com/messages/t/" + friendid);
                chromedriver.DelayRandom(1, 3);
                //}

                lst_node = new List<string> { "//*[text()=\"Continue\"]", "//*[text()=\"Tiếp tục\"]" };
                node = chromedriver.GetElementExistFromList(3, 0.5, lst_node);
                if (!string.IsNullOrEmpty(node))
                {
                    chromedriver.Click(SelectorType.ByXPath, node);
                }

                lst_node = new List<string> { "//span[contains(text(),\"request limit\")]", "//span[contains(text(),\"đã gửi hết số\")]" };
                node = chromedriver.GetElementExistFromList(3, 1, lst_node);
                if (!string.IsNullOrEmpty(node))
                {
                    result.message = HelperCore.L("msg550");
                    return result;
                }
                lst_node = new List<string> { "[aria-label=\"Message\"]", "[aria-label=\"Tin nhắn\"]" };
                node = chromedriver.GetElementExistFromList(4, 0.5, lst_node);
                if (!string.IsNullOrEmpty(node))
                {
                    chromedriver.Click(SelectorType.ByCssSelector, node);
                    scan_send_message = true;
                }
                else
                {
                    for (int checking = 0; checking < 3; checking++)
                    {
                        if (!chromedriver.CheckPoint())
                        {
                            lst_node = new List<string> { "//*[@role=\"dialog\"] //*[@aria-label=\"Đóng\"]", "//*[@role=\"dialog\"] //*[@aria-label=\"Close\"]" };
                            node = chromedriver.GetElementExistFromList(3, 0.5, lst_node);
                            if (!string.IsNullOrEmpty(node))
                            {
                                flag = chromedriver.Click(SelectorType.ByXPath, node).IsOk();
                                chromedriver.DelayTime(1);
                            }

                            lst_node = new List<string> { "//*[@role=\"button\" and @tabindex=\"0\"] //*[contains(text(),\"Không khôi phục\")]", "//*[@role=\"button\" and @tabindex=\"0\"] //*[contains(text(),\"restore message\")]" };
                            node = chromedriver.GetElementExistFromList(3, 0.5, lst_node);
                            if (!string.IsNullOrEmpty(node))
                            {
                                flag = chromedriver.Click(SelectorType.ByXPath, node).IsOk();
                                chromedriver.DelayTime(1);
                            }

                            lst_node = new List<string> { "[role=\"main\"] [aria-label=\"Continue\"][role=\"button\"]", "[role=\"main\"] [aria-label=\"Tiếp tục\"][role=\"button\"]" };
                            node = chromedriver.GetElementExistFromList(4, 0.5, lst_node);
                            if (!string.IsNullOrEmpty(node))
                            {
                                flag = chromedriver.Click(SelectorType.ByCssSelector, node).IsOk();
                                if (flag) chromedriver.DelayRandom(10, 15);
                            }

                            lst_node = new List<string> { "[aria-label*=\"Conversation\"][role=\"main\"] [role=\"textbox\"]", "[aria-label*=\"Cuộc trò chuyện\"][role=\"main\"] [role=\"textbox\"]" };
                            node = chromedriver.GetElementExistFromList(4, 0.5, lst_node);
                            if (!string.IsNullOrEmpty(node))
                            {
                                scan_send_message = chromedriver.Click(SelectorType.ByCssSelector, node).IsOk();
                                if (!scan_send_message)
                                {
                                    lst_node = new List<string> { "[aria-label*=\"Cuộc trò chuyện\"][role=\"main\"] [role=\"button\"][aria-label*=\"Thông tin\"]", "[aria-label*=\"Conversation\"][role=\"main\"] [role=\"button\"][aria-label*=\"information\"]" };
                                    string node_info = chromedriver.GetElementExistFromList(4, 0.5, lst_node);
                                    if (!string.IsNullOrEmpty(node_info))
                                    {
                                        chromedriver.Click(SelectorType.ByCssSelector, node).IsOk();
                                        chromedriver.DelayTime(2);
                                        scan_send_message = chromedriver.Click(SelectorType.ByCssSelector, node).IsOk();
                                    }
                                }
                                chromedriver.DelayTime(1);
                                break;
                            }
                        }
                    }
                }



                if (scan_send_message)
                {
                    Article post = new Article();
                    if (_lstPost.Count > 0)
                    {
                        post = _lstPost[rd.Next(0, _lstPost.Count())];
                        //post.content = HelperCore.DetechString(post.content);
                        //post.Content = post.Content.Replace("$username", account_infor.Name);
                    }

                    //if (typeComment == TypeComment.RenderAI)
                    //{
                    //    if (_settingMain.GetValue("cmbModelAI", null).Contains("gpt"))
                    //    {
                    //        post.content = OpenAIRender.RenderMessage(messageConfig);
                    //    }
                    //    else if (_settingMain.GetValue("cmbModelAI", null).Contains("gemini"))
                    //    {
                    //        post.content = GeminiRender.RenderMessage(messageConfig);
                    //    }
                    //}

                    bool has_sendkeys = false;

                    //string pathTextImg = string.Format("{0}\\data\\virul_sendmess_textImg.txt", Application.StartupPath);
                    //if (File.Exists(pathTextImg))
                    //{
                    //    string dataFile = File.ReadAllText(pathTextImg);
                    //    //textImg = HelperCore.SpinContent(dataFile);
                    //}

                    //HelperTransTextToImg helper_translate = new HelperTransTextToImg();
                    //string path_image = string.Format("{0}Chromeprofile\\{1}\\image", Application.StartupPath, acc.Uid);
                    //if (typeComment == TypeComment.ChangeToImg)
                    //{
                    //    if (!Directory.Exists(path_image))
                    //    {
                    //        Directory.CreateDirectory(path_image);
                    //    }

                    //    path_image = string.Format("{0}Chromeprofile\\{1}\\image\\{2}.jpg", Application.StartupPath, acc.Uid, StringHelper.CreateRandomString(8, new Random()));
                    //    helper_translate.TransTextToImg(alignTextImg, textImg, path_image, fontTextImg, ImageFormat.Jpeg);
                    //    post = new Post { typepost = "media", picture = path_image, numpicture = 1 };
                    //}

                    //send image
                    if (post.LinkImg.Count > 0)
                    {
                        int num_send_image = 0;
                        List<string> lst_pathimg = post.LinkImg;

                        while (lst_pathimg.Count > 0)
                        {
                            if (token.IsCancellationRequested) break;

                            if (chromedriver.CheckElements(4, "input[type*=\"file\"]", 0, 0, null, 0, 2, 0.1))
                            {


                                string path_img = lst_pathimg[Random.Shared.Next(lst_pathimg.Count)];
                                lst_pathimg.Remove(path_img);

                                has_sendkeys = chromedriver.SendKeys(SelectorType.ByCssSelector, "input[type*=\"file\"]", path_img.TrimEnd('\n', '\t', '\r')).IsOk();
                                chromedriver.DelayTime(delay_upload);
                                num_send_image++;
                            }
                        }

                    }

                    chromedriver.ClearTextWithBackSpace(4, node, 0, 0, null, 0, 2, 0.1);
                    //if (!string.IsNullOrEmpty(post.Content))
                    //{
                    //    //lock (_syncThread)
                    //    //{
                    //    //    Invoke((MethodInvoker)delegate
                    //    //    {
                    //    //        // Đặt giá trị vào clipboard
                    //    //        Clipboard.SetText(post.content);
                    //    //        has_sendkeys = chromedriver.SendPaste(4, node, post.content, 0, 0, null, 0, true, 1);
                    //    //        Clipboard.Clear();
                    //    //    });
                    //    //}
                    //}

                    if (has_sendkeys)
                    {
                        flag = chromedriver.SendKeys(SelectorType.ByCssSelector, node, OpenQA.Selenium.Keys.Enter).IsOk();
                        if (flag)
                        {
                            result.status = true;
                            result.note = chromedriver.GetURL();
                        }
                    }

                    //if (File.Exists(path_image))
                    //{
                    //    helper_translate.DeleteImg(path_image);
                    //}
                    chromedriver.DelayRandom(2, 3);
                    //check mess
                    int checkTime = 5;
                    while (true)
                    {
                        if (checkTime == 0)
                        {
                            result.status = false;
                            //UpdateStatusAccount(dr, "status", getText.GetTextValue(HelperCore.L("msg550")));
                            break;
                        }
                        lst_node = new List<string> { "//span[text()=\"Đang gửi\"]", "//span[text()=\"Sending\"]" };
                        node = chromedriver.GetElementExistFromList(3, 2, lst_node);
                        if (!string.IsNullOrWhiteSpace(node))
                        {
                            checkTime--;
                            int timeOut = rd.Next(3, 5);
                            //UpdateStatusAccount(dr, "status", getText.GetTextValue(string.Format(HelperCore.L("mess_wait"), timeOut)));
                            chromedriver.DelayTime(timeOut);
                            continue;
                        }
                        else break;
                    }


                }
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return result;
        }

    }
}
