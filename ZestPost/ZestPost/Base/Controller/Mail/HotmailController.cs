namespace ZestPost.Base.Controller
{
    public class HotmailController
    {
        OAuthentV2HotmailSync oauthent_hotmail = new OAuthentV2HotmailSync();
        //public ViModelSync CheckHotmail(string email, string passmail, CancellationToken token = default)
        //{
        //    ViModelSync result = new ViModelSync();
        //    TokenMailSync token_mail = new TokenMailSync();
        //    try
        //    {
        //        string path_mail = string.Format("{0}\\{1}_token_mail.json", AppInfo.PathProfile, email);
        //        if (File.Exists(path_mail))
        //        {
        //            token_mail = JsonConvert.DeserializeObject<TokenMailSync>(File.ReadAllText(path_mail));
        //        }

        //        if (!string.IsNullOrEmpty(token_mail.access_token))
        //        {
        //            //try connect with access_token
        //            for (int index_connext = 0; index_connext < 2; index_connext++)
        //            {
        //                var imapClient = oauthent_hotmail.connect_IMapClientWithOAuthentV2(email, passmail, token_mail.access_token);
        //                try
        //                {
        //                    result.status = imapClient.IsConnected;
        //                }
        //                catch (OperationCanceledException)
        //                {
        //                    result.data = "fail";
        //                }
        //                catch (Exception ex)
        //                {
        //                    result.data = "fail";
        //                    Console.WriteLine(ex.Message);
        //                }

        //                imapClient.Disconnect(true);

        //                if (!result.status)
        //                {
        //                    token_mail = oauthent_hotmail.GetAccessTokenHotmail(email, passmail, token_mail.refresh_token);
        //                    if (!string.IsNullOrEmpty(token_mail.refresh_token))
        //                    {
        //                        string token_mail_json = JsonConvert.SerializeObject(token_mail);
        //                        File.WriteAllText(path_mail, token_mail_json);
        //                    }
        //                }
        //                else
        //                {
        //                    break;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            result.status = false;
        //            result.message = HelperCore.L("msg_711");
        //            goto lb_finish;
        //        }
        //    }
        //    catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
        //lb_finish:;
        //    return result;
        //}

        //public ViModelSync get_Code_Mail_WithOAuthentV2(int type, string email, string passmail, int time = 120, string normal = "", CancellationToken token = default)
        //{
        //    ViModelSync result = new ViModelSync();
        //    TokenMailSync token_mail = new TokenMailSync();
        //    bool has_message_mail = false;
        //    try
        //    {
        //        string path_mail = string.Format("{0}\\{1}_token_mail.json", AppInfo.PathProfile, email);
        //        if (File.Exists(path_mail))
        //        {
        //            token_mail = JsonConvert.DeserializeObject<TokenMailSync>(File.ReadAllText(path_mail));
        //        }

        //        if (!string.IsNullOrEmpty(token_mail.access_token))
        //        {
        //            //try connect with access_token
        //            for (int index_connext = 0; index_connext < 3; index_connext++)
        //            {
        //                var imapClient = oauthent_hotmail.connect_IMapClientWithOAuthentV2(email, passmail, token_mail.access_token);
        //                try
        //                {
        //                    int tickCount = Environment.TickCount;
        //                    int countMessage = 0;
        //                    do
        //                    {
        //                        if (token.IsCancellationRequested)
        //                            break;

        //                        imapClient.Inbox.Open(FolderAccess.ReadWrite);
        //                        countMessage = imapClient.Inbox.Count;
        //                        if (countMessage > 0)
        //                        {
        //                            has_message_mail = true;
        //                            SearchQuery searchQuery;
        //                            IList<UniqueId> lst_mesage = new List<UniqueId>();
        //                            if (type == 3)
        //                            {
        //                                searchQuery = SearchQuery.ToContains(email);
        //                                lst_mesage = imapClient.Inbox.Search(searchQuery);
        //                            }
        //                            else
        //                            {
        //                                searchQuery = SearchQuery.FromContains("security@facebookmail.com").And(SearchQuery.NotSeen);
        //                                lst_mesage = imapClient.Inbox.Search(searchQuery);
        //                                if (lst_mesage == null || lst_mesage.Count == 0)
        //                                {
        //                                    searchQuery = SearchQuery.FromContains("registration@facebookmail.com").And(SearchQuery.NotSeen);
        //                                    lst_mesage = imapClient.Inbox.Search(searchQuery);
        //                                }
        //                            }

        //                            if (lst_mesage.Count > 0)
        //                            {
        //                                lst_mesage = lst_mesage.OrderByDescending(uid => uid).ToList();
        //                                foreach (var uid in lst_mesage)
        //                                {
        //                                    var message = imapClient.Inbox.GetMessage(uid);
        //                                    TimeSpan khoangCach = DateTime.Today - message.Date.Date;
        //                                    if (khoangCach.Days >= 0 && khoangCach.Days < 3)
        //                                    {
        //                                        var body = (TextPart)message.BodyParts.FirstOrDefault(x => x is TextPart);
        //                                        if (body != null)
        //                                        {
        //                                            string content = body.Text;
        //                                            imapClient.Inbox.AddFlags(uid, MessageFlags.Deleted, true);
        //                                            result.data = mailUtilitiesSync.mapping_Code(content, type, normal);
        //                                            if (!string.IsNullOrEmpty(result.data))
        //                                                goto lb_finish;
        //                                        }
        //                                    }
        //                                    else
        //                                        continue;
        //                                }

        //                            }
        //                            else
        //                            {
        //                                Thread.Sleep(10000);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            Thread.Sleep(5000);
        //                        }
        //                    }
        //                    while (Environment.TickCount - tickCount < time * 1000);
        //                }
        //                catch (OperationCanceledException)
        //                {
        //                    result.data = "fail";
        //                }
        //                catch (Exception ex)
        //                {
        //                    Log4NetSyncController.LogException(ex, "");
        //                    result.data = "fail";
        //                    Console.WriteLine(ex.Message);
        //                }

        //                imapClient.Disconnect(true);

        //                if (!has_message_mail)
        //                {
        //                    token_mail = oauthent_hotmail.GetAccessTokenHotmail(email, passmail, token_mail.refresh_token);
        //                    if (!string.IsNullOrEmpty(token_mail.refresh_token))
        //                    {
        //                        string token_mail_json = JsonConvert.SerializeObject(token_mail);
        //                        File.WriteAllText(path_mail, token_mail_json);
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            result.status = false;
        //            result.message = HelperCore.L("msg_711");
        //            goto lb_finish;
        //        }
        //    }
        //    catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
        //lb_finish:;
        //    return result;
        //}
        //public string get_Code_Mail_WithOAuthentV3(int type, string email, string passmail, int time = 120, string normal = "", CancellationToken token = default)
        //{
        //    Result result = new Result();
        //    string codeMail = "";

        //    TokenMailSync token_mail = new TokenMailSync();
        //    bool has_message_mail = false;
        //    try
        //    {
        //        string path_mail = string.Format("{0}\\{1}_token_mail.json", AppInfo.PathProfile, email);
        //        if (File.Exists(path_mail))
        //        {
        //            token_mail = JsonConvert.DeserializeObject<TokenMailSync>(File.ReadAllText(path_mail));
        //        }

        //        if (!string.IsNullOrEmpty(token_mail.access_token))
        //        {
        //            //try connect with access_token
        //            for (int index_connext = 0; index_connext < 3; index_connext++)
        //            {
        //                var imapClient = oauthent_hotmail.connect_IMapClientWithOAuthentV2(email, passmail, token_mail.access_token);
        //                try
        //                {
        //                    int tickCount = Environment.TickCount;
        //                    int countMessage = 0;
        //                    do
        //                    {
        //                        if (token.IsCancellationRequested)
        //                            break;

        //                        imapClient.Inbox.Open(FolderAccess.ReadWrite);
        //                        countMessage = imapClient.Inbox.Count;
        //                        if (countMessage > 0)
        //                        {
        //                            has_message_mail = true;
        //                            SearchQuery searchQuery;
        //                            IList<UniqueId> lst_mesage = new List<UniqueId>();
        //                            if (type == 3)
        //                            {
        //                                searchQuery = SearchQuery.ToContains(email);
        //                                lst_mesage = imapClient.Inbox.Search(searchQuery);
        //                            }
        //                            else
        //                            {
        //                                searchQuery = SearchQuery.FromContains("security@facebookmail.com").And(SearchQuery.NotSeen);
        //                                lst_mesage = imapClient.Inbox.Search(searchQuery);
        //                                if (lst_mesage == null || lst_mesage.Count == 0)
        //                                {
        //                                    searchQuery = SearchQuery.FromContains("registration@facebookmail.com").And(SearchQuery.NotSeen);
        //                                    lst_mesage = imapClient.Inbox.Search(searchQuery);
        //                                }
        //                            }

        //                            if (lst_mesage.Count > 0)
        //                            {
        //                                lst_mesage = lst_mesage.OrderByDescending(uid => uid).ToList();
        //                                foreach (var uid in lst_mesage)
        //                                {
        //                                    var message = imapClient.Inbox.GetMessage(uid);
        //                                    TimeSpan khoangCach = DateTime.Today - message.Date.Date;
        //                                    if (khoangCach.Days >= 0 && khoangCach.Days < 3)
        //                                    {
        //                                        var body = (TextPart)message.BodyParts.FirstOrDefault(x => x is TextPart);
        //                                        if (body != null)
        //                                        {
        //                                            string content = body.Text;
        //                                            imapClient.Inbox.AddFlags(uid, MessageFlags.Deleted, true);
        //                                            codeMail = mailUtilitiesSync.mapping_Code(content, type, normal);
        //                                            if (!string.IsNullOrEmpty(codeMail))
        //                                                goto lb_finish;
        //                                        }
        //                                    }
        //                                    else
        //                                        continue;
        //                                }

        //                            }
        //                            else
        //                            {
        //                                Thread.Sleep(10000);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            Thread.Sleep(5000);
        //                        }
        //                    }
        //                    while (Environment.TickCount - tickCount < time * 1000);
        //                }
        //                catch (OperationCanceledException)
        //                {
        //                    return "";
        //                }
        //                catch (Exception ex)
        //                {
        //                    Log4NetSyncController.LogException(ex, "");
        //                    Console.WriteLine(ex.Message);
        //                    return "";
        //                }

        //                imapClient.Disconnect(true);

        //                if (!has_message_mail)
        //                {
        //                    token_mail = oauthent_hotmail.GetAccessTokenHotmail(email, passmail, token_mail.refresh_token);
        //                    if (!string.IsNullOrEmpty(token_mail.refresh_token))
        //                    {
        //                        string token_mail_json = JsonConvert.SerializeObject(token_mail);
        //                        File.WriteAllText(path_mail, token_mail_json);
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            result.IsError();
        //            result.Message = HelperCore.L("msg_711");
        //            goto lb_finish;
        //        }
        //    }
        //    catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
        //lb_finish:;
        //    return codeMail;
        //}

        //public Result<TAccount> GetCodeMailWithOAuthentV3<TAccount>(int type, TAccount account, int timeout = 120, string normal = "", CancellationToken token = default) where TAccount : IAccount, IAccountFacebook
        //{
        //    Result result = new Result();
        //    TokenMailSync token_mail = new TokenMailSync();
        //    bool has_message_mail = false;
        //    try
        //    {

        //        //token_mail = account.TokenMail;

        //        //get new access token
        //        if (token_mail != null)
        //        {
        //            token_mail = oauthent_hotmail.GetAccessTokenHotmail(account.Email ?? "", account.Passmail ?? "", token_mail.refresh_token);
        //            if (!string.IsNullOrEmpty(token_mail.refresh_token))
        //            {
        //                string token_mail_json = JsonConvert.SerializeObject(token_mail);
        //                File.WriteAllText(path_mail, token_mail_json);
        //            }

        //            if (!token_mail.access_token.IsEmpty())
        //            {
        //                for (int index_connext = 0; index_connext < 3; index_connext++)
        //                {
        //                    var imapClient = oauthent_hotmail.connect_IMapClientWithOAuthentV2(account.Email ?? "", account.Passmail ?? "", token_mail.access_token);
        //                    try
        //                    {
        //                        int tickCount = Environment.TickCount;
        //                        int countMessage = 0;
        //                        do
        //                        {
        //                            if (token.IsCancellationRequested)
        //                                break;

        //                            imapClient.Inbox.Open(FolderAccess.ReadWrite);
        //                            countMessage = imapClient.Inbox.Count;
        //                            if (countMessage > 0)
        //                            {
        //                                SearchQuery searchQuery;
        //                                IList<UniqueId> lst_mesage = new List<UniqueId>();
        //                                if (type == 3)
        //                                {
        //                                    searchQuery = SearchQuery.ToContains(account.Email);
        //                                    lst_mesage = imapClient.Inbox.Search(searchQuery);
        //                                }
        //                                else
        //                                {
        //                                    searchQuery = SearchQuery.FromContains("security@facebookmail.com").And(SearchQuery.NotSeen);
        //                                    lst_mesage = imapClient.Inbox.Search(searchQuery);
        //                                    if (lst_mesage == null || lst_mesage.Count == 0)
        //                                    {
        //                                        searchQuery = SearchQuery.FromContains("registration@facebookmail.com").And(SearchQuery.NotSeen);
        //                                        lst_mesage = imapClient.Inbox.Search(searchQuery);
        //                                    }
        //                                }

        //                                if (lst_mesage.Count > 0)
        //                                {
        //                                    lst_mesage = lst_mesage.OrderByDescending(uid => uid).ToList();
        //                                    foreach (var uid in lst_mesage)
        //                                    {
        //                                        var message = imapClient.Inbox.GetMessage(uid);
        //                                        TimeSpan khoangCach = DateTime.Today - message.Date.Date;
        //                                        if (khoangCach.Days >= 0 && khoangCach.Days < 3)
        //                                        {
        //                                            TextPart? body = message.BodyParts.FirstOrDefault(x => x is TextPart) as TextPart;
        //                                            if (body != null)
        //                                            {
        //                                                string content = body.Text;
        //                                                imapClient.Inbox.AddFlags(uid, MessageFlags.Deleted, true);
        //                                                if (!string.IsNullOrEmpty(mailUtilitiesSync.mapping_Code(content, type, normal)))
        //                                                    goto lb_finish;
        //                                            }
        //                                        }
        //                                    }

        //                                }
        //                                else
        //                                {
        //                                    Task.Delay(10000).Wait();
        //                                }
        //                            }
        //                            else
        //                            {
        //                                Task.Delay(5000).Wait();
        //                            }
        //                        }
        //                        while (Environment.TickCount - tickCount < timeout * 1000);
        //                    }
        //                    catch (OperationCanceledException)
        //                    {
        //                        return Result.Error<TAccount>("fail");
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        Log4NetSyncController.LogException(ex, "");
        //                        Console.WriteLine(ex.Message);
        //                        return Result.Error<TAccount>("fail");
        //                    }

        //                    imapClient.Disconnect(true);
        //                    if (!has_message_mail)
        //                    {
        //                        token_mail = oauthent_hotmail.GetAccessTokenHotmail(account.Email, account.Passmail, token_mail.refresh_token);
        //                        if (!string.IsNullOrEmpty(token_mail.refresh_token))
        //                        {
        //                            string token_mail_json = JsonConvert.SerializeObject(token_mail);
        //                            File.WriteAllText(path_mail, token_mail_json);
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                result.IsError();
        //                result.Message = HelperCore.L("core_error_loginHostMail_warning_not_get_code");
        //                goto lb_finish;
        //            }
        //        }
        //        else
        //        {
        //            result.IsError();
        //            result.Message = HelperCore.L("core_error_loginHostMail_warning_not_get_code");
        //            goto lb_finish;
        //        }
        //    }
        //    catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
        //lb_finish:;
        //    return Result.Ok<TAccount>(account);
        //}
    }
}
