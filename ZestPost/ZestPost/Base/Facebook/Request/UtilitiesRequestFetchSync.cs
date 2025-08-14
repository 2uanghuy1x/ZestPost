using System.Text.RegularExpressions;
using ZestPost.Base.Extension;

namespace ZestPost.Base.Facebook
{
    public class UtilitiesRequestFetchSync
    {
        public string RenderDataSend(ChromeBrowser chromedriver, string uid, string variables, string FB_API_REQ_FRIENDLY_NAME, string DOC_ID)
        {
            string result_data_send = "";
            try
            {
                //get origin data
                string __jazoest = null;
                try
                {
                    string origin_data = chromedriver.GetAttributeTextByXPath("//*[@id=\"__eqmc\"]");
                    var match = !string.IsNullOrEmpty(origin_data) ? Regex.Match(origin_data, @"jazoest=(\d+)") : null;
                    __jazoest = match?.Success == true ? match.Groups[1].Value : null;
                }
                catch { }

                string fb_dtsg = chromedriver.ExecuteScript("return require('DTSGInitData').token").ToString();
                string __user = uid;
                string __a = "1";
                string dpr = chromedriver.ExecuteScript("return require(['SiteData']).pr").ToString();
                string __ccg = "EXCELLENT";
                string __rev = chromedriver.ExecuteScript("return require(['SiteData']).__spin_r").ToString();
                string __hsi = chromedriver.ExecuteScript("return require(['SiteData']).hsi").ToString();
                string __dyn = Guid.NewGuid().ToString();
                string __spin_r = chromedriver.ExecuteScript("return require(['SiteData']).__spin_r").ToString();
                string __spin_b = chromedriver.ExecuteScript("return require(['SiteData']).__spin_b").ToString();
                string __spin_t = chromedriver.ExecuteScript("return require(['SiteData']).__spin_t").ToString();
                string fb_api_caller_class = "RelayModern";
                string fb_api_req_friendly_name = FB_API_REQ_FRIENDLY_NAME;
                string doc_id = DOC_ID;
                result_data_send = $@"{{
                                        var formData = new FormData();
                                        formData.append('av', '{uid}');
                                        formData.append('__user', '{uid}');
                                        formData.append('__a', '{__a}');
                                        formData.append('dpr', '{dpr}');
                                        formData.append('__ccg', '{__ccg}');
                                        formData.append('__rev', '{__rev}');
                                        formData.append('__hsi', '{__hsi}');
                                        formData.append('__dyn', '{__dyn}');
                                        formData.append('fb_dtsg', '{fb_dtsg}');
                                        formData.append('jazoest', '{__jazoest}');
                                        formData.append('__spin_r', '{__spin_r}');
                                        formData.append('__spin_b', '{__spin_b}');
                                        formData.append('__spin_t', '{__spin_t}');
                                        formData.append('fb_api_caller_class', '{fb_api_caller_class}');
                                        formData.append('fb_api_req_friendly_name', '{fb_api_req_friendly_name}');
                                        formData.append('variables',JSON.stringify({variables}));
                                        formData.append('server_timestamps', 'true');
                                        formData.append('doc_id', '{doc_id}');
                                      }}";
            }
            catch { }
            return result_data_send;
        }
        public string RenderDataSwitch(ChromeBrowser chromedriver, string idNow, string variables, string FB_API_REQ_FRIENDLY_NAME, string DOC_ID)
        {
            string resultDataSend = "";
            try
            {
                //get origin data
                string __jazoest = null;
                try
                {
                    string origin_data = chromedriver.GetAttributeTextByXPath("//*[@id=\"__eqmc\"]");
                    var match = !string.IsNullOrEmpty(origin_data) ? Regex.Match(origin_data, @"jazoest=(\d+)") : null;
                    __jazoest = match?.Success == true ? match.Groups[1].Value : null;
                }
                catch { }

                string fb_dtsg = chromedriver.ExecuteScript("return require('DTSGInitData').token").ToString();
                string __user = idNow;
                string __a = "1";
                string dpr = chromedriver.ExecuteScript("return require(['SiteData']).pr").ToString();
                string __ccg = "EXCELLENT";
                string __rev = chromedriver.ExecuteScript("return require(['SiteData']).__spin_r").ToString();
                string __hsi = chromedriver.ExecuteScript("return require(['SiteData']).hsi").ToString();
                string __dyn = Guid.NewGuid().ToString();
                string __comet_req = "15";
                string __spin_r = chromedriver.ExecuteScript("return require(['SiteData']).__spin_r").ToString();
                string __spin_b = chromedriver.ExecuteScript("return require(['SiteData']).__spin_b").ToString();
                string __spin_t = chromedriver.ExecuteScript("return require(['SiteData']).__spin_t").ToString();
                string fb_api_caller_class = "RelayModern";
                string fb_api_req_friendly_name = FB_API_REQ_FRIENDLY_NAME;
                string doc_id = DOC_ID;
                resultDataSend = $@"{{
                                        var formData = new FormData();
                                        formData.append('av', '{idNow}');
                                        formData.append('__user', '{__user}');
                                        formData.append('__a', '{__a}');
                                        formData.append('dpr', '{dpr}');
                                        formData.append('__ccg', '{__ccg}');
                                        formData.append('__rev', '{__rev}');
                                        formData.append('__hsi', '{__hsi}');
                                        formData.append('__dyn', '{__dyn}');
                                        formData.append('__comet_req', '{__comet_req}');
                                        formData.append('fb_dtsg', '{fb_dtsg}');
                                        formData.append('jazoest', '{__jazoest}');
                                        formData.append('__spin_r', '{__spin_r}');
                                        formData.append('__spin_b', '{__spin_b}');
                                        formData.append('__spin_t', '{__spin_t}');
                                        formData.append('fb_api_caller_class', '{fb_api_caller_class}');
                                        formData.append('fb_api_req_friendly_name', '{fb_api_req_friendly_name}');
                                        formData.append('variables',JSON.stringify({variables}));
                                        formData.append('server_timestamps', 'true');
                                        formData.append('doc_id', '{doc_id}');
                                      }}";
            }
            catch { }
            return resultDataSend;
        }
        public string RenderDataUploadVideoSchedule(ChromeBrowser chromedriver, string variables, string FB_API_REQ_FRIENDLY_NAME, string DOC_ID)
        {
            string result_data_send = "";
            try
            {
                //get origin data
                string __jazoest = null;
                try
                {
                    string origin_data = chromedriver.GetAttributeTextByXPath("//*[@id=\"__eqmc\"]");
                    var match = !string.IsNullOrEmpty(origin_data) ? Regex.Match(origin_data, @"jazoest=(\d+)") : null;
                    __jazoest = match?.Success == true ? match.Groups[1].Value : null;
                }
                catch { }

                string assetId = Regex.Match(chromedriver.GetURL(), @"asset_id=(\d+)").Groups[1].Value;
                string __usid = Guid.NewGuid().ToString();
                string __aaid = "0";
                string __user = chromedriver.ExecuteScript("return require(['CurrentUserInitialData']).USER_ID").ToString();
                string __a = "1";
                string dpr = chromedriver.ExecuteScript("return require(['SiteData']).pr").ToString();
                string __ccg = "EXCELLENT";
                string __rev = chromedriver.ExecuteScript("return require(['SiteData']).__spin_r").ToString();
                string __hsi = chromedriver.ExecuteScript("return require(['SiteData']).hsi").ToString();
                string __dyn = Guid.NewGuid().ToString();
                string __comet_req = chromedriver.ExecuteScript("return require(['SiteData']).comet_env").ToString();
                string fb_dtsg = chromedriver.ExecuteScript("return require('DTSGInitData').token").ToString();
                string __spin_r = chromedriver.ExecuteScript("return require(['SiteData']).__spin_r").ToString();
                string __spin_b = chromedriver.ExecuteScript("return require(['SiteData']).__spin_b").ToString();
                string __spin_t = chromedriver.ExecuteScript("return require(['SiteData']).__spin_t").ToString();
                string fb_api_caller_class = "RelayModern";
                string fb_api_req_friendly_name = FB_API_REQ_FRIENDLY_NAME;
                string doc_id = DOC_ID;
                result_data_send = $@"{{
                                        var formData = new FormData();
                                        formData.append('av', '{assetId}');                                        
                                        formData.append('__usid', '{__usid}');
                                        formData.append('__aaid', '{__aaid}');
                                        formData.append('__user', '{__user}');
                                        formData.append('__a', '{__a}');
                                        formData.append('dpr', '{dpr}');
                                        formData.append('__ccg', '{__ccg}');
                                        formData.append('__rev', '{__rev}');
                                        formData.append('__hsi', '{__hsi}');
                                        formData.append('__dyn', '{__dyn}');                                        
                                        formData.append('__comet_req', '{__comet_req}');
                                        formData.append('fb_dtsg', '{fb_dtsg}');
                                        formData.append('jazoest', '{__jazoest}');
                                        formData.append('__spin_r', '{__spin_r}');
                                        formData.append('__spin_b', '{__spin_b}');
                                        formData.append('__spin_t', '{__spin_t}');
                                        formData.append('fb_api_caller_class', '{fb_api_caller_class}');
                                        formData.append('fb_api_req_friendly_name', '{fb_api_req_friendly_name}');
                                        formData.append('variables',JSON.stringify({variables}));
                                        formData.append('server_timestamps', 'true');
                                        formData.append('doc_id', '{doc_id}');
                                      }}";
            }
            catch { }
            return result_data_send;
        }
        public string RequestGet(ChromeBrowser chrome, string url, string website, string token = null)
        {
            try
            {
                bool flag = website.Split(new char[]
                {
                    '/'
                }).Length > 2;
                if (flag)
                {
                    website = website.Replace("//", "__");
                    website = website.Split(new char[]
                    {
                        '/'
                    })[0];
                    website = website.Replace("__", "//");
                }
                bool flag2 = !chrome.GetURL().StartsWith(website);
                if (flag2)
                {
                    chrome.GotoURL(website);
                    chrome.DelayTime(1.0);
                }

                if (string.IsNullOrEmpty(token))
                {
                    return chrome.ExecuteScript("async function RequestGet() { var output = ''; try { var response = await fetch('" + url + "'); if (response.ok) { var body = await response.text(); return body; } } catch {} return output; }; var c = await RequestGet(); return c;").ToString();
                }
                else
                {
                    return chrome.ExecuteScript("async function RequestGet() { var output = ''; try { var response = await fetch('" + url + "',{headers: {'Authorization': 'Bearer " + token + "'}}); if (response.ok) { var body = await response.text(); return body; } } catch {} return output; }; var c = await RequestGet(); return c;").ToString();
                }
            }
            catch
            {
            }
            return "";
        }

        public string RequestPost(ChromeBrowser chrome, string url, string form_data, string website, string contentType = "application/x-www-form-urlencoded")
        {
            try
            {
                bool flag = !chrome.GetURL().StartsWith(website);
                if (flag)
                {
                    chrome.GotoURL(website);
                    chrome.DelayRandom(3, 5);
                }
                chrome.DelayTime(0.1);
                string a = string.Concat(new string[]
                {
                    "async function RequestPost() {\r\n        var output = ''; " +"\r\n       try { \r\n       ",form_data,
                    "\r\n        var response = await fetch('",url,
                    "', { method: 'POST', body: formData,}); " +
                    "\r\n       if (response.ok) { var body = await response.text(); return body; } }\r\n        catch(error) {console.error(error);} return output; }; " +
                    "\r\n       var c = await RequestPost(); " +"return c;"
                });

                return chrome.ExecuteScript(string.Concat(new string[]
                {
                    "async function RequestPost() {\r\n        var output = ''; " +"\r\n       try { \r\n       ",form_data,
                    "\r\n        var response = await fetch('",url,
                    "', { method: 'POST', body: formData,}); " +
                    "\r\n       if (response.ok) { var body = await response.text(); return body; } }\r\n        catch(error) {console.error(error);} return output; }; " +
                    "\r\n       var c = await RequestPost(); " +"return c;"
                })).ToString();
            }
            catch
            {
            }
            return "";
        }

    }
}
