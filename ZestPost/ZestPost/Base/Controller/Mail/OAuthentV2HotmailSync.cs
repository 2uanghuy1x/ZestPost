namespace ZestPost.Base.Controller
{
    public class OAuthentV2HotmailSync
    {
        private string get_Domain_Hotmail(string email)
        {
            string result = "";
            if (email.Contains("@hotmail.") || email.Contains("@live.") || email.Contains("@rickystar.") || email.Contains("@outlook."))
            {
                result = "outlook.office365.com|imap-mail.outlook.com";
            }
            else if (email.Contains("@yandex."))
            {
                result = "imap.yandex.com";
            }
            else if (email.Contains("@gmail."))
            {
                result = "imap.gmail.com";
            }
            else
            {
                result = string.Format("mail.{0}", email.Split('@')[1]);
            }
            return result;
        }

        //public TokenMailSync GetTokenHotmail(string email, string password, string code_get_token)
        //{
        //    TokenMailSync token_mail = new TokenMailSync();
        //    try
        //    {
        //        var client = new RestClient("https://login.microsoftonline.com/common/oauth2/v2.0/token");
        //        var request = new RestRequest(Method.POST);
        //        request.AddParameter("code", code_get_token);
        //        request.AddParameter("grant_type", "authorization_code");
        //        request.AddParameter("redirect_uri", "https://localhost/");
        //        request.AddParameter("client_id", "9e5f94bc-e8a4-4e73-b8be-63364c29d753");
        //        IRestResponse response = client.Execute(request);
        //        string data = response.Content.ToString();
        //        if (data.Contains("Bearer"))
        //        {
        //            JObject jObject = JObject.Parse(data);
        //            token_mail.status = true;
        //            token_mail.email = email;
        //            token_mail.password = password;
        //            token_mail.access_token = jObject["access_token"].ToString();
        //            token_mail.refresh_token = jObject["refresh_token"].ToString();
        //            token_mail.scope = jObject["scope"].ToString();
        //            token_mail.expired = jObject["expires_in"].ToString();
        //            token_mail.time = HelperSyncController.ConvertDatetimeToTimestamp(DateTime.Now).ToString();
        //        }
        //    }
        //    catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
        //    return token_mail;
        //}

        //public TokenMailSync GetAccessTokenHotmail(string email, string password, string refresh_token)
        //{
        //    TokenMailSync token_mail = new TokenMailSync();
        //    try
        //    {
        //        var client = new RestClient("https://login.microsoftonline.com/common/oauth2/v2.0/token");
        //        var request = new RestRequest(Method.POST);
        //        request.AddParameter("client_id", "9e5f94bc-e8a4-4e73-b8be-63364c29d753");
        //        request.AddParameter("grant_type", "refresh_token");
        //        request.AddParameter("refresh_token", refresh_token);
        //        request.AddParameter("scope", "https://outlook.office.com/IMAP.AccessAsUser.All https://outlook.office.com/POP.AccessAsUser.All https://outlook.office.com/EWS.AccessAsUser.All https://outlook.office.com/SMTP.Send");
        //        IRestResponse response = client.Execute(request);
        //        string data = response.Content.ToString();
        //        if (data.Contains("Bearer"))
        //        {
        //            JObject jObject = JObject.Parse(data);
        //            token_mail.status = true;
        //            token_mail.email = email;
        //            token_mail.password = password;
        //            token_mail.access_token = jObject["access_token"].ToString();
        //            token_mail.refresh_token = refresh_token;
        //            token_mail.scope = jObject["scope"].ToString();
        //            token_mail.expired = jObject["expires_in"].ToString();
        //            token_mail.time = HelperSyncController.ConvertDatetimeToTimestamp(DateTime.Now).ToString();
        //        }
        //    }
        //    catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
        //    return token_mail;
        //}

        //public ImapClient connect_IMapClientWithOAuthentV2(string email, string passmail, string accessToken)
        //{
        //    ImapClient result = null;
        //    string domain = get_Domain_Hotmail(email);
        //    if (!string.IsNullOrEmpty(domain))
        //    {
        //        List<string> list = domain.Split('|').ToList();
        //        foreach (string host in list)
        //        {
        //            try
        //            {
        //                result = new ImapClient();
        //                result.Connect(host, 993, SecureSocketOptions.SslOnConnect);
        //                var oauth2 = new SaslMechanismOAuth2(email, accessToken);
        //                result.Authenticate(oauth2);
        //            }
        //            catch (Exception ex)
        //            {
        //                Log4NetSyncController.LogException(ex, "");
        //                Console.WriteLine($"Lỗi: {ex.Message}");
        //            }
        //        }
        //    }
        //    return result;
        //}
    }
}
