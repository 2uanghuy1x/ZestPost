using System.Text;

namespace ZestPost.Base.Controller
{
    public class LoginBaseController
    {
        public async Task<string> Get2FAFacebook(string privatekey)
        {
            string result_data = "";
            try
            {
                using (var request = new HttpClient())
                {
                    var obj = new { key = privatekey };
                    var data = JsonConvert.SerializeObject(obj);
                    var content = new StringContent(data, Encoding.UTF8, "application/json");
                    var response = await request.PostAsync("https://2fa.live/", content);
                    if (response.IsSuccessStatusCode)
                    {
                        string html = await response.Content.ReadAsStringAsync();
                        JObject rawdata = JObject.Parse(html);
                        result_data = rawdata["code"]?.ToString() ?? "";
                    }
                    else
                    {
                        Log4NetSyncController.LogException(new HttpRequestException($"Request failed with status code: {response.StatusCode}"), "");
                    }
                }
            }
            catch (Exception ex)
            {
                Log4NetSyncController.LogException(ex, "");
            }
            return result_data;
        }

    }
}
