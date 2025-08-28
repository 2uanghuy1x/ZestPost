using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Diagnostics;
using static ZestPost.Base.Controller.IpSyncController;

namespace ZestPost.Base
{
    public class ChromeBrowser : IJavaScriptExecutor
    {
        private bool _isTerminating = false;
        private readonly Random _rd;
        private JsonConfigBuilder _settingMain { get; set; }
        public Process Process { get; set; }
        public int MainPid { get; set; }
        public Process ProcessChromeView { get; set; }
        public int ChromeviewPid { get; set; }
        public ChromeDriver ChromeDriver { get; set; }
        public ChromeOptions ChromeOptions { get; set; }
        public ChromeDriverService Service { get; set; }
        public bool HideBrowser { get; set; }
        public bool Incognito { get; set; }
        public bool DisableImage { get; set; }
        public bool DisableSound { get; set; }
        public bool AutoPlayVideo { get; set; }
        public bool Sock5 { get; set; }
        public bool IsUseEmulator { get; set; }
        public bool UseExtensionProxy { get; set; }
        public bool IsScale { get; set; }
        public bool UseChromeview { get; set; }
        private bool Headless { get; set; }
        public Point Size { get; set; }
        public Point Position { get; set; }
        public int IndexChrome;
        public int PixelRatio { get; set; }
        public int TimeWaitForSearchingElement { get; set; }
        public int TimeWaitForLoadingPage { get; set; }
        public int TypeProxy { get; set; }
        public string UserAgent { get; set; }
        public string ProfilePath { get; set; }
        public string PathChromeDriver { get; set; }
        public string ChromeExePath { get; set; }
        public string Proxy { get; set; }
        public IPSync IpBrowser = new IPSync();
        public string App { get; set; }
        public string LinkToOtherBrowser { get; set; }
        public string PathExtension { get; set; }
        public Point SizeEmulator { get; set; }
        public StatusChromeAccount Status { get; set; }

        public ChromeBrowser()
        {
            App = "";
            IndexChrome = 0;
            HideBrowser = false;
            DisableImage = false;
            DisableSound = false;
            Incognito = false;
            UserAgent = "";
            ProfilePath = "";
            ChromeExePath = "";
            Size = new Point(300, 300);
            Size = new Point(Size.X, Size.Y);
            Proxy = "";
            Sock5 = false;
            TypeProxy = 0;
            Position = new Point(Position.X, Position.Y);
            TimeWaitForLoadingPage = 1;
            IsScale = true;
            AutoPlayVideo = false;
            LinkToOtherBrowser = "";
            PathExtension = "data\\extension";
            IsUseEmulator = false;
            SizeEmulator = new Point(300, 300);
            Status = StatusChromeAccount.Empty;
            _rd = new Random();
            _settingMain = new JsonConfigBuilder(BaseConstants.CORE_SETTING_FILE_NAME);
        }

        public void OpenChrome(string name)
        {
            try
            {
                string data = AppDomain.CurrentDomain.BaseDirectory;
                // Đường dẫn đến chromedriver.exe
                string chromeDriverPath = Path.Combine(data, "chromedriver.exe"); // Đảm bảo tên file đúng

                // Đường dẫn đến thư mục profile
                string chromeProfilePath = Path.Combine(data, "ChromeProfile");
                // Tạo thư mục profile nếu chưa tồn tại
                if (!Directory.Exists(chromeProfilePath))
                {
                    Directory.CreateDirectory(chromeProfilePath);
                }

                string chromeProfileNamePath = Path.Combine(data, $"ChromeProfile/{name}");
                // Tạo thư mục profile nếu chưa tồn tại
                if (!Directory.Exists(chromeProfileNamePath))
                {
                    Directory.CreateDirectory(chromeProfileNamePath);
                }

                // Cấu hình ChromeOptions
                ChromeOptions = new ChromeOptions();
                ChromeOptions.AddArgument($"user-data-dir={chromeProfileNamePath}"); // Sử dụng thư mục profile
                ChromeOptions.AddArgument("--profile-directory=Default");  // Profile mặc định
                ChromeOptions.AddArgument("--mute-audio");
                // Đảm bảo không chạy ẩn danh
                ChromeOptions.AddExcludedArgument("incognito");
                if (!string.IsNullOrEmpty(UserAgent))
                {
                    ChromeOptions.AddArgument($"--user-agent={UserAgent}");
                }
                if (!Headless)
                {
                    ChromeOptions.AddArgument("--start-maximized"); // Tùy chọn: mở cửa sổ tối đa
                }

                var service = ChromeDriverService.CreateDefaultService(
                    Path.GetDirectoryName(chromeDriverPath)); // Sử dụng chromeDriverPath
                service.HideCommandPromptWindow = true;
                ChromeDriver = new ChromeDriver(service, ChromeOptions);
                ChromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(TimeWaitForSearchingElement);
                ChromeDriver.Manage().Timeouts().PageLoad = TimeSpan.FromMinutes(TimeWaitForLoadingPage);
                Process = Process.GetProcessById(service.ProcessId);
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi");
            }
        }

        public void CloseChrome()
        {
            try
            {
                if (!_isTerminating && ChromeDriver != null)
                {
                    _isTerminating = true;
                    ChromeDriver.Quit();
                    ChromeDriver.Dispose();
                    ChromeDriver = null;

                    if (Process != null)
                    {
                        if (!Process.HasExited)
                        {
                            Process.Kill();
                            Process.Dispose();
                        }
                        Process = null;
                    }

                    Status = StatusChromeAccount.ChromeClosed;
                    Console.WriteLine("Chrome closed successfully.");
                }
            }
            catch (Exception ex)
            {
                Log4NetSyncController.LogException(ex, "Error closing Chrome");
                Status = StatusChromeAccount.Error;
            }
            finally
            {
                _isTerminating = false;
            }
        }
        public bool IsOpenChrome()
        {
            return !IsCloseChrome();
        }
        public bool IsCloseChrome()
        {
            if (Process != null)
            {
                return Process.HasExited;
            }
            bool result = true;
            try
            {
                string title = ChromeDriver?.Title;
                result = false;
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return result;
        }
        public string GetURL()
        {
            string result = "";
            if (IsOpenChrome())
            {
                try
                {
                    result = ChromeDriver.Url;
                }
                catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            }
            return result;
        }
        public bool GotoURL(string url)
        {
            bool flag = false;
            if (IsOpenChrome())
            {
                try
                {
                    if (GetURL() != url)
                    {
                        ChromeDriver.Navigate().GoToUrl(url);
                        flag = true;
                    }
                }
                catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            }
            return flag;
        }
        public string GetElementExistFromList(int type, double timeWait_Second, List<string> list_querySelector)
        {
            if (IsOpenChrome())
            {
                try
                {
                    int num = 0;
                    int tickCount = Environment.TickCount;
                    while (true)
                    {
                        switch (type)
                        {
                            case 1:
                                num = Convert.ToInt32(ChromeDriver.ExecuteScript(
                                    "var arr='" + string.Join("|", list_querySelector) + "'.split('|');" +
                                    "var output=0;" +
                                    "for (i=0; i<arr.length; i++) { " +
                                    "if (document.getElementById(arr[i]) !== null) { output = i + 1; break; } } " +
                                    "return (output + '');"));
                                break;
                            case 2:
                                num = Convert.ToInt32(ChromeDriver.ExecuteScript(
                                    "var arr='" + string.Join("|", list_querySelector) + "'.split('|');" +
                                    "var output=0;" +
                                    "for (i=0; i<arr.length; i++) { " +
                                    "if (document.getElementsByName(arr[i]).length > 0) { output = i + 1; break; } } " +
                                    "return (output + '');"));
                                break;
                            case 3:
                                string js = "var arr='" + string.Join("|", list_querySelector) + "'.split('|');" +
                                            "var output=0;" +
                                            "for (i=0; i<arr.length; i++) { " +
                                            "if (document.evaluate(arr[i], document, null, XPathResult.UNORDERED_NODE_SNAPSHOT_TYPE, null).snapshotLength > 0) { output = i + 1; break; } } " +
                                            "return (output + '');";
                                num = Convert.ToInt32(ChromeDriver.ExecuteScript(js));
                                break;
                            case 4:
                                string js_selector = "var arr='" + string.Join("|", list_querySelector) + "'.split('|');" +
                                                     "var output=0;" +
                                                     "for (i=0; i<arr.length; i++) { " +
                                                     "if (document.querySelectorAll(arr[i]).length > 0) { output = i + 1; break; } } " +
                                                     "return (output + '');";
                                num = Convert.ToInt32(ChromeDriver.ExecuteScript(js_selector));
                                break;
                        }

                        if (num <= 0 || num > list_querySelector.Count)
                        {
                            if (Environment.TickCount - tickCount > timeWait_Second * 1000)
                            {
                                return null; // Timeout
                            }
                            Thread.Sleep(1);
                            continue;
                        }

                        Thread.Sleep(1000);
                        return list_querySelector[num - 1];
                    }
                }
                catch (Exception ex)
                {
                    Log4NetSyncController.LogException(ex, "Error in GetElementExistFromList");
                }
            }
            return null;
        }
        public int CountElement(int type, double timeWait_Second, string attributeValue)
        {
            int result = 0;
            if (IsOpenChrome())
            {
                try
                {
                    int num = 0;
                    int tickCount = Environment.TickCount;
                    while (true)
                    {
                        switch (type)
                        {
                            case 1:
                                num = Convert.ToInt32(ChromeDriver.ExecuteScript("return document.getElementById('" + attributeValue + "').length;"));
                                break;
                            case 2:
                                num = Convert.ToInt32(ChromeDriver.ExecuteScript("return document.getElementsByName('" + attributeValue + "').length;"));
                                break;
                            case 3:
                                string js_xpath = "return document.evaluate('" + attributeValue + "', document, null, XPathResult.UNORDERED_NODE_SNAPSHOT_TYPE, null).snapshotLength";
                                num = Convert.ToInt32(ChromeDriver.ExecuteScript(js_xpath));
                                break;
                            case 4:
                                string js = "return document.querySelectorAll('" + attributeValue + "').length;";
                                num = Convert.ToInt32(ChromeDriver.ExecuteScript(js));
                                break;
                        }

                        if (num == 0)
                        {
                            if (!(Environment.TickCount - tickCount > timeWait_Second * 1000))
                            {
                                Thread.Sleep(1);
                                continue;
                            }
                        }
                        Thread.Sleep(1000);
                        return num;
                    }
                }
                catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            }
            return result;
        }
        public void DelayTime(double timeDelay)
        {
            try
            {
                if (IsOpenChrome())
                {
                    Thread.Sleep(Convert.ToInt32(timeDelay * 1000.0));
                }
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
        }
        public bool CheckElements(int typeAttribute, string attributeValue, int index = 0, int subTypeAttribute = 0, string subAttributeValue = "", int subIndex = 0, int times = 1, double delaytime = 0.1)
        {
            bool flag = false;
            try
            {
                if (IsOpenChrome())
                {
                    for (int i = 0; i < times; DelayTime(delaytime), i++)
                    {
                        try
                        {
                            if (subTypeAttribute == 0)
                            {
                                switch (typeAttribute)
                                {
                                    case 1:
                                        flag = ChromeDriver.FindElements(By.XPath(attributeValue))[index] != null ? true : false;
                                        break;
                                    case 2:
                                        flag = ChromeDriver.FindElements(By.ClassName(attributeValue))[index] != null ? true : false;
                                        break;
                                    case 3:
                                        flag = ChromeDriver.FindElements(By.XPath(attributeValue))[index] != null ? true : false;
                                        break;
                                    case 4:
                                        flag = ChromeDriver.FindElements(By.CssSelector(attributeValue))[index] != null ? true : false;
                                        break;
                                }
                            }
                            else
                            {
                                switch (typeAttribute)
                                {
                                    case 1:
                                        flag = ChromeDriver.FindElements(By.XPath(attributeValue))[index].FindElements(By.Id(subAttributeValue))[subIndex] != null ? true : false;
                                        break;
                                    case 2:
                                        flag = ChromeDriver.FindElements(By.ClassName(attributeValue))[index].FindElements(By.Name(subAttributeValue))[subIndex] != null ? true : false;
                                        break;
                                    case 3:
                                        flag = ChromeDriver.FindElements(By.XPath(attributeValue))[index].FindElements(By.XPath(subAttributeValue))[subIndex] != null ? true : false;
                                        break;
                                    case 4:
                                        flag = ChromeDriver.FindElements(By.CssSelector(attributeValue))[index].FindElements(By.CssSelector(subAttributeValue))[subIndex] != null ? true : false;
                                        break;
                                }
                            }
                            if (flag)
                                break;
                        }
                        catch
                        {
                            continue;
                        }
                        DelayTime(delaytime);
                    }
                }
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return flag;
        }
        public bool ClearTextWithBackSpace(int typeAttribute, string attributeValue, int index = 0, int subTypeAttribute = 0, string subAttributeValue = "", int subIndex = 0, int times = 1, double delaytime = 0.1)
        {
            bool flag = false;
            try
            {
                if (IsOpenChrome())
                {
                    string js_function = null;
                    for (int i = 0; i < times; DelayTime(delaytime), i++)
                    {
                        try
                        {
                            if (subTypeAttribute == 0)
                            {
                                switch (typeAttribute)
                                {
                                    case 1:
                                        flag = ChromeDriver.FindElements(By.XPath(attributeValue))[index] != null ? true : false;
                                        js_function = "async function RequestGet(){ var item = document.getElementById('" + attributeValue + "').value;if(item==null){item = document.querySelector('" + attributeValue + "').innerText;} return item.length;} var c = await RequestGet(); return c;";
                                        break;
                                    case 2:
                                        flag = ChromeDriver.FindElements(By.ClassName(attributeValue))[index] != null ? true : false;
                                        js_function = "async function RequestGet(){ var item = document.getElementsByName('" + attributeValue + "').value;if(item==null){item = document.querySelector('" + attributeValue + "').innerText;} return item.length;} var c = await RequestGet(); return c;";
                                        break;
                                    case 3:
                                        flag = ChromeDriver.FindElements(By.XPath(attributeValue))[index] != null ? true : false;
                                        js_function = "async function RequestGet(){ var item = document.evaluate(" + attributeValue + ", document, null, XPathResult.UNORDERED_NODE_SNAPSHOT_TYPE, null).value;if(item==null){item = document.querySelector('" + attributeValue + "').innerText;} return item.length;} var c = await RequestGet(); return c;";
                                        break;
                                    case 4:
                                        flag = ChromeDriver.FindElements(By.CssSelector(attributeValue))[index] != null ? true : false;
                                        js_function = "async function RequestGet(){ var item = document.querySelector('" + attributeValue + "').value;if(item==null){item = document.querySelector('" + attributeValue + "').innerText;} return item.length;} var c = await RequestGet(); return c;";
                                        break;
                                }
                            }
                            else
                            {
                                switch (typeAttribute)
                                {
                                    case 1:
                                        flag = ChromeDriver.FindElements(By.XPath(attributeValue))[index].FindElements(By.Id(subAttributeValue))[subIndex] != null ? true : false;
                                        js_function = "return document.getElementById('" + attributeValue + "').value.length;";
                                        break;
                                    case 2:
                                        flag = ChromeDriver.FindElements(By.ClassName(attributeValue))[index].FindElements(By.Name(subAttributeValue))[subIndex] != null ? true : false;
                                        js_function = "return document.getElementsByName('" + attributeValue + "').value.length;";
                                        break;
                                    case 3:
                                        flag = ChromeDriver.FindElements(By.XPath(attributeValue))[index].FindElements(By.XPath(subAttributeValue))[subIndex] != null ? true : false;
                                        js_function = "return document.evaluate(" + attributeValue + ", document, null, XPathResult.UNORDERED_NODE_SNAPSHOT_TYPE, null).value.length;";
                                        break;
                                    case 4:
                                        flag = ChromeDriver.FindElements(By.CssSelector(attributeValue))[index].FindElements(By.CssSelector(subAttributeValue))[subIndex] != null ? true : false;
                                        js_function = "return document.querySelector('" + attributeValue + "')[" + subIndex + "].value.length;";
                                        break;
                                }
                            }

                            if (flag)
                            {
                                break;
                            }
                        }
                        catch
                        {
                            continue;
                        }
                        DelayTime(delaytime);
                    }

                    if (flag)
                    {
                        string length = ChromeDriver.ExecuteScript(js_function).ToString();
                        if (!string.IsNullOrEmpty(length))
                        {
                            for (int j = 0; j < int.Parse(length); j++)
                            {
                                SendBackspace(typeAttribute, attributeValue);
                                DelayTime(0.1);
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return flag;
        }
        public bool SendBackspace(int typeAttribute, string attributeValue, int index = 0, int subTypeAttribute = 0, string subAttributeValue = "", int subIndex = 0)
        {
            bool flag = false;
            if (IsOpenChrome())
            {
                try
                {
                    if (subTypeAttribute == 0)
                    {
                        switch (typeAttribute)
                        {
                            case 1:
                                ChromeDriver.FindElements(By.XPath(attributeValue))[index].SendKeys(Keys.Backspace);
                                break;

                            case 2:
                                ChromeDriver.FindElements(By.ClassName(attributeValue))[index].SendKeys(Keys.Backspace);
                                break;

                            case 3:
                                ChromeDriver.FindElements(By.XPath(attributeValue))[index].SendKeys(Keys.Backspace);
                                break;

                            case 4:
                                ChromeDriver.FindElements(By.CssSelector(attributeValue))[index].SendKeys(Keys.Backspace);
                                break;
                        }
                        flag = true;
                    }
                    else
                    {
                        switch (typeAttribute)
                        {
                            case 1:
                                ChromeDriver.FindElements(By.XPath(attributeValue))[index].FindElements(By.Id(subAttributeValue))[subIndex].SendKeys(Keys.Backspace);
                                break;

                            case 2:
                                ChromeDriver.FindElements(By.ClassName(attributeValue))[index].FindElements(By.Name(subAttributeValue))[subIndex].SendKeys(Keys.Backspace);
                                break;

                            case 3:
                                ChromeDriver.FindElements(By.XPath(attributeValue))[index].FindElements(By.XPath(subAttributeValue))[subIndex].SendKeys(Keys.Backspace);
                                break;

                            case 4:
                                ChromeDriver.FindElements(By.CssSelector(attributeValue))[index].FindElements(By.CssSelector(subAttributeValue))[subIndex].SendKeys(Keys.Backspace);
                                break;
                        }
                    }
                }
                catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            }
            return flag;
        }
        public bool AddCookieIntoChrome(string cookie, string domain = ".facebook.com")
        {
            bool flag = false;
            if (IsOpenChrome())
            {
                try
                {
                    if (!string.IsNullOrEmpty(cookie))
                    {
                        string[] array = cookie.Split(';');
                        string[] array2 = array;
                        foreach (string text in array2)
                        {
                            if (text.Trim() != "")
                            {
                                string[] array3 = text.Split('=');
                                if (array3.Count() > 1 && array3[0].Trim() != "")
                                {
                                    OpenQA.Selenium.Cookie cookie2 = new OpenQA.Selenium.Cookie(array3[0].Trim(), text.Substring(text.IndexOf('=') + 1).Trim(), domain, "/", DateTime.Now.AddDays(10.0));
                                    ChromeDriver.Manage().Cookies.AddCookie(cookie2);
                                }
                            }
                        }
                        ChromeDriver.Navigate().Refresh();
                        flag = true;
                    }
                }
                catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            }
            return flag;
        }
        public string GetCookieFromChrome(string domain = "facebook")
        {
            string cookies = "";
            if (IsOpenChrome())
            {
                try
                {
                    OpenQA.Selenium.Cookie[] array = ChromeDriver.Manage().Cookies.AllCookies.ToArray();
                    OpenQA.Selenium.Cookie[] array2 = array;
                    foreach (OpenQA.Selenium.Cookie cookie in array2)
                    {
                        if (cookie.Domain.Contains(domain))
                        {
                            cookies = cookies + cookie.Name + "=" + cookie.Value + ";";
                        }
                    }
                }
                catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            }
            return cookies;
        }
        public string GetAttributeTextByXPath(string attributeValue, double timeDelay = 0.1)
        {
            string result = "";
            if (IsOpenChrome())
            {
                try
                {
                    string scriptXpath = string.Format(@"
                        async function Namdubai() {{
                        var result = document.evaluate('{0}', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.innerText;
                        return result;
                        }}
                        var c = await Namdubai();
                        return c;
                        ", attributeValue);
                    result = ChromeDriver.ExecuteScript(scriptXpath).ToString();
                    DelayTime(timeDelay);
                }
                catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            }
            return result;
        }
        public object? ExecuteScript(string script, params object?[] args)
        {
            if (ChromeDriver == null)
            {
                throw new InvalidOperationException("ChromeDriver is not initialized.");
            }
            return ((IJavaScriptExecutor)ChromeDriver).ExecuteScript(script, args);
        }
        public object? ExecuteScript(PinnedScript script, params object?[] args)
        {
            throw new NotImplementedException();
        }
        public object? ExecuteAsyncScript(string script, params object?[] args)
        {
            if (ChromeDriver == null)
            {
                throw new InvalidOperationException("ChromeDriver is not initialized.");
            }
            return ((IJavaScriptExecutor)ChromeDriver).ExecuteAsyncScript(script, args);
        }
        public void ScrollPageRandom(int distance, int num = 1, int timemin = 1, int timemax = 1)
        {
            if (IsOpenChrome())
            {
                try
                {
                    for (int i = 0; i < num; i++)
                    {
                        ChromeDriver.ExecuteScript("window.scrollBy({ top: " + distance + ",behavior: 'smooth'});");
                        DelayTime(0.5);
                    }
                }
                catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            }
        }
        public void ScrollPage(string JSpath)
        {
            if (IsOpenChrome())
                try
                {
                    ExecuteScript(JSpath + ".scrollIntoView({ behavior: 'smooth', block: 'center'});");
                }
                catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
        }
        public void ScrollPage(int distance)
        {
            if (IsOpenChrome())
            {
                try
                {
                    ChromeDriver.ExecuteScript("window.scrollBy({ top: " + distance + ",behavior: 'smooth'});");
                    DelayTime(1);
                }
                catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            }
        }

    }
}
