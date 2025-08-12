using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;

namespace ZestPost.Base.Extension
{
    public static class ChromeExtensions
    {
        private static readonly Random _rd = new();
        public static Result IsClose(this ChromeBrowser chrome)
        {
            try
            {
                if (chrome.Process.IsNotEmpty())
                {
                    return chrome.Process.HasExited ? Result.OK : Result.Error("Chrome đang mở!");
                }
                if (chrome.ChromeDriver.IsNotEmpty() && chrome.ChromeDriver.Title.IsNotEmpty())
                {
                    return Result.Error("Chrome đang mở!");
                }
                return Result.OK;
            }
            catch (Exception ex)
            {
                Log4NetSyncController.LogException(ex, "Lỗi khi xác định trạng thái chrome!");
                return Result.Error("Lỗi khi xác định trạng thái chrome!");
            }
        }
        public static Result IsOpen(this ChromeBrowser chrome)
        {
            try
            {
                var result = chrome.IsClose();
                if (result.IsOk())
                {
                    return Result.Error("Chrome đã đóng!");
                }
                return Result.OK;
            }
            catch (Exception ex)
            {
                Log4NetSyncController.LogException(ex, "Lỗi khi xác định trạng thái chrome!");
                return Result.Error("Lỗi khi xác định trạng thái chrome!");
            }
        }
        public static Result GoToUrl(this ChromeBrowser chrome, string url, bool waitForPageLoad = true)
        {
            try
            {
                var rsChromeStatus = chrome.IsOpen();
                if (rsChromeStatus.IsError()) return rsChromeStatus;

                var rsGetUrl = chrome.GetUrl();
                if (rsGetUrl.IsError()) return rsGetUrl;

                string urlRs = rsGetUrl.Data;

                if (!url.Equals(urlRs))
                {
                    chrome.ChromeDriver.Navigate().GoToUrl(url);
                    if (waitForPageLoad)
                    {
                        var resultWaitForPageLoad = chrome.WaitForPageLoad();
                        if (resultWaitForPageLoad.IsError()) return resultWaitForPageLoad;
                    }
                    return Result.OK;
                }
                return Result.Error($"Đi đến url thất bại [{url}]");
            }
            catch (Exception ex)
            {
                string msg = $"Xảy ra lỗi khi di chuyển chrome đến url [{url}]";
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error(msg);
            }
        }
        public static Result Back(this ChromeBrowser chrome, int numBackPage = 1, int delayTime = 1)
        {
            try
            {
                var rsChromeStatus = chrome.IsOpen();
                if (rsChromeStatus.IsError()) return rsChromeStatus;

                Result rs = new();
                for (int i = 0; i < numBackPage; i++)
                {
                    chrome.ChromeDriver.Navigate().Back();
                    rs = chrome.Delay(delayTime);
                    if (rs.IsError()) return rs;
                }

                return rs;
            }
            catch (Exception ex)
            {
                var msg = "Back thất bại";
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error<string>(msg);
            }
        }
        public static Result<string> GetUrl(this ChromeBrowser chrome)
        {
            try
            {
                var rsChromeStatus = chrome.IsOpen();
                if (rsChromeStatus.IsError()) return Result.Error<string>(rsChromeStatus.Message);

                return Result.Ok(chrome.ChromeDriver.Url);
            }
            catch (Exception ex)
            {
                Log4NetSyncController.LogException(ex, "Lỗi khi check lấy url chrome!");
                return Result.Error<string>("Lỗi khi check lấy url chrome!");
            }
        }
        public static Result<string> GetPageSource(this ChromeBrowser chrome)
        {
            try
            {
                var pageSource = chrome.ChromeDriver.PageSource;
                return Result.Ok(pageSource);
            }
            catch (Exception ex)
            {
                string msg = "Lỗi khi GetPageSource";
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error<string>(msg);
            }
        }
        public static Result AddCookies(this ChromeBrowser chrome, string cookie, string domain = ".facebook.com")
        {
            try
            {
                var rsChromeStatus = chrome.IsOpen();
                if (rsChromeStatus.IsError()) return Result.Error(rsChromeStatus.Message);

                string[] array = cookie.Split(';');
                foreach (string text in array)
                {
                    if (text.Trim() != "")
                    {
                        string[] array3 = text.Split('=');
                        if (array3.Length > 1 && array3[0].Trim() != "")
                        {
                            OpenQA.Selenium.Cookie cookie2 = new OpenQA.Selenium.Cookie(array3[0].Trim(), text[(text.IndexOf('=') + 1)..].Trim(), domain, "/", DateTime.Now.AddDays(10.0));
                            chrome.ChromeDriver.Manage().Cookies.AddCookie(cookie2);
                        }
                    }
                }
                return Result.OK;
            }
            catch (Exception ex)
            {
                var msg = "AddCookies thất bại";
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error(msg);
            }
        }
        public static Result<string> GetCookie(this ChromeBrowser chrome, string domain = "facebook")
        {
            try
            {
                var rsChromeStatus = chrome.IsOpen();
                if (rsChromeStatus.IsError()) return Result.Error<string>(rsChromeStatus.Message);

                string cookies = "";
                List<OpenQA.Selenium.Cookie> array = chrome.ChromeDriver.Manage().Cookies.AllCookies.ToList();
                foreach (OpenQA.Selenium.Cookie cookie in array)
                {
                    if (cookie.Domain.Contains(domain))
                    {
                        cookies = cookies + cookie.Name + "=" + cookie.Value + ";";
                    }
                }

                if (cookies.IsEmpty()) return Result.Error<string>("Cookie rỗng, lấy cookie thất bại!");
                return Result.Ok(cookies);
            }
            catch (Exception ex)
            {
                var msg = "GetCookie thất bại";
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error<string>(msg);
            }
        }
        public static Result<string> RunScript(this ChromeBrowser chrome, string script)
        {
            try
            {
                if (script.IsEmpty()) return Result.Error<string>("Script rỗng!");
                var rsChromeStatus = chrome.IsOpen();
                if (rsChromeStatus.IsError()) return Result.Error<string>(rsChromeStatus.Message);

                var rsRunScript = chrome.ChromeDriver.ExecuteScript(script);
                var rs = rsRunScript.AsString();
                return Result.Ok(rs);
            }
            catch (Exception ex)
            {
                string msg = $"Xảy ra lỗi khi run script!";
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error<string>(msg);
            }
        }
        public static Result Delay(this ChromeBrowser chrome, double timeDelay)
        {
            try
            {
                var rsChromeStatus = chrome.IsOpen();
                if (rsChromeStatus.IsError()) return rsChromeStatus;

                Thread.Sleep(Convert.ToInt32(timeDelay * 1000.0));
                return Result.OK;
            }
            catch (Exception ex)
            {
                string msg = $"Xảy ra lỗi khi delay!";
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error(msg);
            }
        }
        public static Result DelayRandom(this ChromeBrowser chrome, int timeFrom, int timeTo)
        {
            try
            {
                var rsChromeStatus = chrome.IsOpen();
                if (rsChromeStatus.IsError()) return rsChromeStatus;

                var time = _rd.Next(timeFrom, timeTo);
                var rsDelay = chrome.Delay(time);
                if (rsDelay.IsError()) return rsDelay;
                return Result.OK;
            }
            catch (Exception ex)
            {
                string msg = $"Xảy ra lỗi khi delay!";
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error(msg);
            }
        }
        public static Result<IWebElement> GetElement(this ChromeBrowser chrome, SelectorType selectorType, string selectorContent, int index = 0)
        {
            try
            {
                var rsChromeStatus = chrome.IsOpen();
                if (rsChromeStatus.IsError()) return Result.Error<IWebElement>(rsChromeStatus.Message);

                IWebElement element = null;
                ReadOnlyCollection<IWebElement> listElement;
                switch (selectorType)
                {
                    case SelectorType.ById:
                        listElement = chrome.ChromeDriver.FindElements(By.Id(selectorContent));
                        if (listElement.IsNotEmpty())
                        {
                            if (index <= listElement.Count - 1)
                            {
                                element = listElement[index];
                            }
                        }
                        break;
                    case SelectorType.ByTagName:
                        listElement = chrome.ChromeDriver.FindElements(By.TagName(selectorContent));
                        if (listElement.IsNotEmpty())
                        {
                            if (index <= listElement.Count - 1)
                            {
                                element = listElement[index];
                            }
                        }
                        break;
                    case SelectorType.ByXPath:
                        listElement = chrome.ChromeDriver.FindElements(By.XPath(selectorContent));
                        if (listElement.IsNotEmpty())
                        {
                            if (index <= listElement.Count - 1)
                            {
                                element = listElement[index];
                            }
                        }
                        break;
                    case SelectorType.ByCssSelector:
                        listElement = chrome.ChromeDriver.FindElements(By.CssSelector(selectorContent));
                        if (listElement.IsNotEmpty())
                        {
                            if (index <= listElement.Count - 1)
                            {
                                element = listElement[index];
                            }
                        }
                        break;
                }
                if (element.IsNotEmpty()) return Result.Ok(element);
                return Result.Error<IWebElement>($"Không tồn tại element {selectorContent}!");
            }
            catch (Exception ex)
            {
                string msg = $"Lỗi khi lấy element [{selectorContent}]!";
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error<IWebElement>(msg);
            }
        }
        public static Result<IWebElement> GetElement(this ChromeBrowser chrome, SelectorType selectorType, List<string> listSelectorContent, int index = 0)
        {
            try
            {
                var rsChromeStatus = chrome.IsOpen();
                if (rsChromeStatus.IsError()) return Result.Error<IWebElement>(rsChromeStatus.Message);

                IWebElement element = null;
                foreach (var selectorContent in listSelectorContent)
                {
                    ReadOnlyCollection<IWebElement> listElement;
                    switch (selectorType)
                    {
                        case SelectorType.ById:
                            listElement = chrome.ChromeDriver.FindElements(By.Id(selectorContent));
                            if (listElement.IsNotEmpty())
                            {
                                if (index <= listElement.Count - 1)
                                {
                                    element = listElement[index];
                                }
                            }
                            break;
                        case SelectorType.ByTagName:
                            listElement = chrome.ChromeDriver.FindElements(By.TagName(selectorContent));
                            if (listElement.IsNotEmpty())
                            {
                                if (index <= listElement.Count - 1)
                                {
                                    element = listElement[index];
                                }
                            }
                            break;
                        case SelectorType.ByXPath:
                            listElement = chrome.ChromeDriver.FindElements(By.XPath(selectorContent));
                            if (listElement.IsNotEmpty())
                            {
                                if (index <= listElement.Count - 1)
                                {
                                    element = listElement[index];
                                }
                            }
                            break;
                        case SelectorType.ByCssSelector:
                            listElement = chrome.ChromeDriver.FindElements(By.CssSelector(selectorContent));
                            if (listElement.IsNotEmpty())
                            {
                                if (index <= listElement.Count - 1)
                                {
                                    element = listElement[index];
                                }
                            }
                            break;
                    }
                    if (element.IsNotEmpty()) return Result.Ok(element);
                }
                return Result.Error<IWebElement>($"Không tồn tại element {(listSelectorContent != null && listSelectorContent.Any() ? string.Join(", ", listSelectorContent) : "none")}!");
            }
            catch (Exception ex)
            {
                string msg = $"Lỗi khi lấy element! [{(listSelectorContent != null && listSelectorContent.Any() ? string.Join(", ", listSelectorContent) : "none")}]";
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error<IWebElement>(msg);
            }
        }
        public static Result<List<IWebElement>> GetAllElement(this ChromeBrowser chrome, SelectorType selectorType, string selectorContent)
        {
            try
            {
                var rsChromeStatus = chrome.IsOpen();
                if (rsChromeStatus.IsError()) return Result.Error<List<IWebElement>>(rsChromeStatus.Message);

                List<IWebElement> element = null;
                switch (selectorType)
                {
                    case SelectorType.ById:
                        element = chrome.ChromeDriver.FindElements(By.Id(selectorContent)).ToList();
                        break;
                    case SelectorType.ByTagName:
                        element = chrome.ChromeDriver.FindElements(By.TagName(selectorContent)).ToList();
                        break;
                    case SelectorType.ByXPath:
                        element = chrome.ChromeDriver.FindElements(By.XPath(selectorContent)).ToList();
                        break;
                    case SelectorType.ByCssSelector:
                        element = chrome.ChromeDriver.FindElements(By.CssSelector(selectorContent)).ToList();
                        break;
                }
                if (element.IsNotEmpty()) return Result.Ok(element);
                return Result.Error<List<IWebElement>>("Không tồn tại elements!");
            }
            catch (Exception ex)
            {
                string msg = $"Lỗi khi lấy all elements! [{selectorContent}]";
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error<List<IWebElement>>(msg);
            }
        }
        public static Result<List<IWebElement>> GetAllElement(this ChromeBrowser chrome, SelectorType selectorType, List<string> listSelectorContent)
        {
            try
            {
                var rsChromeStatus = chrome.IsOpen();
                if (rsChromeStatus.IsError()) return Result.Error<List<IWebElement>>(rsChromeStatus.Message);

                List<IWebElement> element = null;
                foreach (var selectorContent in listSelectorContent)
                {
                    switch (selectorType)
                    {
                        case SelectorType.ById:
                            element = chrome.ChromeDriver.FindElements(By.Id(selectorContent)).ToList();
                            break;
                        case SelectorType.ByTagName:
                            element = chrome.ChromeDriver.FindElements(By.TagName(selectorContent)).ToList();
                            break;
                        case SelectorType.ByXPath:
                            element = chrome.ChromeDriver.FindElements(By.XPath(selectorContent)).ToList();
                            break;
                        case SelectorType.ByCssSelector:
                            element = chrome.ChromeDriver.FindElements(By.CssSelector(selectorContent)).ToList();
                            break;
                    }
                    if (element.IsNotEmpty()) return Result.Ok(element);
                }
                return Result.Error<List<IWebElement>>("Không tồn tại elements!");
            }
            catch (Exception ex)
            {
                string msg = $"Lỗi khi lấy all elements! [{(listSelectorContent != null && listSelectorContent.Any() ? string.Join(", ", listSelectorContent) : "none")}]";
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error<List<IWebElement>>(msg);
            }
        }
        public static Result<bool> IsExistElement(this ChromeBrowser chrome, SelectorType selectorType, string selectorContent)
        {
            try
            {
                var rsChromeStatus = chrome.IsOpen();
                if (rsChromeStatus.IsError()) return Result.Error<bool>(rsChromeStatus.Message);

                var rsGetElement = chrome.GetElement(selectorType, selectorContent);
                var result = new Result<bool>(rsGetElement, rsGetElement.IsNotEmpty());
                return result;
            }
            catch (Exception ex)
            {
                string msg = $"Lỗi khi check exist element! [{selectorContent}]";
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error<bool>(msg);
            }
        }
        public static Result IsExistElement(this ChromeBrowser chrome, SelectorType selectorType, List<string> listSelectorContent)
        {
            try
            {
                var rsChromeStatus = chrome.IsOpen();
                if (rsChromeStatus.IsError()) return Result.Error(rsChromeStatus.Message);

                var rsGetElement = chrome.GetElement(selectorType, listSelectorContent);
                return rsGetElement;
            }
            catch (Exception ex)
            {
                string msg = $"Lỗi khi check exist element! [{(listSelectorContent != null && listSelectorContent.Any() ? string.Join(", ", listSelectorContent) : "none")}]";
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error<bool>(msg);
            }
        }
        public static Result ScrollToElement(this ChromeBrowser chrome, SelectorType selectoryType, string selecttorContent, int index = 0)
        {
            try
            {
                var rsChromeStatus = chrome.IsOpen();
                if (rsChromeStatus.IsError()) return Result.Error(rsChromeStatus.Message);

                var rsGetElement = chrome.GetElement(selectoryType, selecttorContent, index);
                if (rsGetElement.IsError()) return rsGetElement;

                IWebElement element = rsGetElement.Data;
                chrome.ChromeDriver.ExecuteScript("arguments[0].scrollIntoView({ behavior: 'smooth', block: 'center' });", element);

                return Result.OK;
            }
            catch (Exception ex)
            {
                string msg = $"Lỗi khi ScrollToElement {selecttorContent}";
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error<IWebElement>(msg);
            }
        }
        public static Result ScrollToElement(this ChromeBrowser chrome, SelectorType selectoryType, List<string> listSelecttorContent, int index = 0)
        {
            try
            {
                var rsChromeStatus = chrome.IsOpen();
                if (rsChromeStatus.IsError()) return Result.Error(rsChromeStatus.Message);

                var rsGetElement = chrome.GetElement(selectoryType, listSelecttorContent, index);
                if (rsGetElement.IsError()) return rsGetElement;

                IWebElement element = rsGetElement.Data;
                chrome.ChromeDriver.ExecuteScript("arguments[0].scrollIntoView({ behavior: 'smooth', block: 'center' });", element);

                return Result.OK;
            }
            catch (Exception ex)
            {
                string msg = $"Lỗi khi ScrollToElement [{(listSelecttorContent != null && listSelecttorContent.Any() ? string.Join(", ", listSelecttorContent) : "none")}]";
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error<IWebElement>(msg);
            }
        }
        public static Result ScrollToElement(this ChromeBrowser chrome, IWebElement element)
        {
            try
            {
                var rsChromeStatus = chrome.IsOpen();
                if (rsChromeStatus.IsError()) return Result.Error(rsChromeStatus.Message);

                chrome.ChromeDriver.ExecuteScript("arguments[0].scrollIntoView({ behavior: 'smooth', block: 'center' });", element);

                return Result.OK;
            }
            catch (Exception ex)
            {
                string msg = $"Lỗi khi ScrollToElement";
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error<IWebElement>(msg);
            }
        }
        public static Result Scroll(this ChromeBrowser chrome, int pixcelScroll, int time = 1, int delay = 1, ScrollType scrollType = ScrollType.Down)
        {
            try
            {
                var rsChromeStatus = chrome.IsOpen();
                if (rsChromeStatus.IsError()) return Result.Error(rsChromeStatus.Message);

                string script = "window.scrollBy({0}, {1});";
                switch (scrollType)
                {
                    case ScrollType.Down:
                        script = string.Format(script, "0", pixcelScroll);
                        break;
                    case ScrollType.Up:
                        script = string.Format(script, "0", $"-{pixcelScroll}");
                        break;
                    case ScrollType.Left:
                        script = string.Format(script, pixcelScroll, "0");
                        break;
                    case ScrollType.Right:
                        script = string.Format(script, $"-{pixcelScroll}", "0");
                        break;
                }
                Result rs = Result.Error("Scroll thất bại");
                for (int i = 0; i < time; i++)
                {
                    rs = chrome.RunScript(script);
                    if (rs.IsError()) return rs;

                    if (time > 1)
                    {
                        var resultDelay = chrome.Delay(delay);
                        if (resultDelay.IsError()) return resultDelay;
                    }
                }
                return rs;
            }
            catch (Exception ex)
            {
                var msg = "Scroll thất bại";
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error(msg);
            }
        }
        public static Result ScrollToTop(this ChromeBrowser chrome)
        {
            try
            {
                var rsChromeStatus = chrome.IsOpen();
                if (rsChromeStatus.IsError()) return Result.Error(rsChromeStatus.Message);

                string script = "window.scrollBy(0, 0);";
                var rs = chrome.RunScript(script);
                return rs;
            }
            catch (Exception ex)
            {
                var msg = "Scroll top thất bại";
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error(msg);
            }
        }
        public static Result ScrollToBottom(this ChromeBrowser chrome)
        {
            try
            {
                var rsChromeStatus = chrome.IsOpen();
                if (rsChromeStatus.IsError()) return Result.Error(rsChromeStatus.Message);

                string script = "window.scrollTo(0, document.body.scrollHeight);";
                var rs = chrome.RunScript(script);
                return rs;
            }
            catch (Exception ex)
            {
                var msg = "Scroll top thất bại";
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error(msg);
            }
        }
        public static Result Click(this ChromeBrowser chrome, SelectorType selectoryType, string selecttorContent, int numOfClick = 1, int delayTime = 1, int index = 0)
        {
            try
            {
                var rsChromeStatus = chrome.IsOpen();
                if (rsChromeStatus.IsError()) return Result.Error(rsChromeStatus.Message);

                var rsGetElement = chrome.GetElement(selectoryType, selecttorContent, index);
                if (rsGetElement.IsError()) return rsGetElement;

                IWebElement element = rsGetElement.Data;
                var rsScrollToElement = chrome.ScrollToElement(element);
                if (rsScrollToElement.IsError()) return rsScrollToElement;

                var rsDelay = chrome.Delay(delayTime);
                if (rsDelay.IsError()) return rsDelay;

                for (int i = 0; i < numOfClick; i++)
                {
                    element.Click();
                }
                return Result.Ok();
            }
            catch (Exception ex)
            {
                string msg = $"Lỗi khi click element [{selecttorContent}]";
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error<IWebElement>(msg);
            }
        }
        public static Result Click(this ChromeBrowser chrome, SelectorType selectoryType, List<string> listSelecttorContent, int numOfClick = 1, int delayTime = 1, int index = 0)
        {
            try
            {
                var rsChromeStatus = chrome.IsOpen();
                if (rsChromeStatus.IsError()) return Result.Error(rsChromeStatus.Message);

                var rsGetElement = chrome.GetElement(selectoryType, listSelecttorContent, index);
                if (rsGetElement.IsError()) return rsGetElement;

                IWebElement element = rsGetElement.Data;
                var rsScrollToElement = chrome.ScrollToElement(element);
                if (rsScrollToElement.IsError()) return rsScrollToElement;

                var rsDelay = chrome.Delay(delayTime);
                if (rsDelay.IsError()) return rsDelay;

                for (int i = 0; i < numOfClick; i++)
                {
                    element.Click();
                }
                return Result.Ok();
            }
            catch (Exception ex)
            {
                string msg = $"Lỗi khi click element [{(listSelecttorContent != null && listSelecttorContent.Any() ? string.Join(", ", listSelecttorContent) : "none")}]";
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error<IWebElement>(msg);
            }
        }
        public static Result ClickRandom(this ChromeBrowser chrome, SelectorType selectoryType, string selecttorContent, int numOfClick = 1, int delayTime = 1)
        {
            try
            {
                var rsChromeStatus = chrome.IsOpen();
                if (rsChromeStatus.IsError()) return Result.Error(rsChromeStatus.Message);

                var rsGetElement = chrome.GetAllElement(selectoryType, selecttorContent);
                if (rsGetElement.IsError()) return rsGetElement;

                List<IWebElement> listElement = rsGetElement.Data;
                int indexRandom = _rd.Next(0, listElement.Count);

                IWebElement element = listElement[indexRandom];
                var rsScrollToElement = chrome.ScrollToElement(element);
                if (rsScrollToElement.IsError()) return rsScrollToElement;

                var rsDelay = chrome.Delay(delayTime);
                if (rsDelay.IsError()) return rsDelay;

                for (int i = 0; i < numOfClick; i++)
                {
                    element.Click();
                }
                return Result.Ok();
            }
            catch (Exception ex)
            {
                string msg = $"Lỗi khi click random element [{selecttorContent}]";
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error<IWebElement>(msg);
            }
        }
        public static Result ClickRandom(this ChromeBrowser chrome, SelectorType selectoryType, List<string> listSelecttorContent, int numOfClick = 1, int delayTime = 1)
        {
            try
            {
                var rsChromeStatus = chrome.IsOpen();
                if (rsChromeStatus.IsError()) return Result.Error(rsChromeStatus.Message);

                var rsGetElement = chrome.GetAllElement(selectoryType, listSelecttorContent);
                if (rsGetElement.IsError()) return rsGetElement;

                List<IWebElement> listElement = rsGetElement.Data;
                int indexRandom = _rd.Next(0, listElement.Count);

                IWebElement element = listElement[indexRandom];
                var rsScrollToElement = chrome.ScrollToElement(element);
                if (rsScrollToElement.IsError()) return rsScrollToElement;

                var rsDelay = chrome.Delay(delayTime);
                if (rsDelay.IsError()) return rsDelay;

                for (int i = 0; i < numOfClick; i++)
                {
                    element.Click();
                }
                return Result.Ok();
            }
            catch (Exception ex)
            {
                string msg = $"Lỗi khi click element [{(listSelecttorContent != null && listSelecttorContent.Any() ? string.Join(", ", listSelecttorContent) : "none")}]";
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error<IWebElement>(msg);
            }
        }
        public static Result Hover(this ChromeBrowser chrome, SelectorType selectoryType, string selecttorContent, int index = 0, int timeHover = 4, int delayTime = 1)
        {
            try
            {
                var rsChromeStatus = chrome.IsOpen();
                if (rsChromeStatus.IsError()) return Result.Error(rsChromeStatus.Message);

                var rsGetElement = chrome.GetElement(selectoryType, selecttorContent, index);
                if (rsGetElement.IsError()) return rsGetElement;

                IWebElement element = rsGetElement.Data;
                var rsScrollToElement = chrome.ScrollToElement(element);
                if (rsScrollToElement.IsError()) return rsScrollToElement;

                var rsDelay = chrome.Delay(delayTime);
                if (rsDelay.IsError()) return rsDelay;

                Actions action = new(chrome.ChromeDriver);
                action.MoveToElement(element).Perform();
                chrome.Delay(timeHover);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                string msg = $"Lỗi khi hover element {selecttorContent}";
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error<IWebElement>(msg);
            }
        }
        public static Result Hover(this ChromeBrowser chrome, SelectorType selectoryType, List<string> listSelecttorContent, int index = 0, int timeHover = 4, int delayTime = 1)
        {
            try
            {
                var rsChromeStatus = chrome.IsOpen();
                if (rsChromeStatus.IsError()) return Result.Error(rsChromeStatus.Message);

                var rsGetElement = chrome.GetElement(selectoryType, listSelecttorContent, index);
                if (rsGetElement.IsError()) return rsGetElement;

                IWebElement element = rsGetElement.Data;
                var rsScrollToElement = chrome.ScrollToElement(element);
                if (rsScrollToElement.IsError()) return rsScrollToElement;

                var rsDelay = chrome.Delay(delayTime);
                if (rsDelay.IsError()) return rsDelay;

                Actions action = new(chrome.ChromeDriver);
                action.MoveToElement(element).Perform();
                chrome.Delay(timeHover);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                string msg = $"Lỗi khi hover element {listSelecttorContent}";
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error<IWebElement>(msg);
            }
        }
        public static Result HoverRandom(this ChromeBrowser chrome, SelectorType selectoryType, string selecttorContent, int timeHover = 4, int delayTime = 1)
        {
            try
            {
                var rsChromeStatus = chrome.IsOpen();
                if (rsChromeStatus.IsError()) return Result.Error(rsChromeStatus.Message);

                var rsGetElement = chrome.GetAllElement(selectoryType, selecttorContent);
                if (rsGetElement.IsError()) return rsGetElement;

                List<IWebElement> listElement = rsGetElement.Data;
                int indexRandom = _rd.Next(0, listElement.Count);

                IWebElement element = listElement[indexRandom];
                var rsScrollToElement = chrome.ScrollToElement(element);
                if (rsScrollToElement.IsError()) return rsScrollToElement;

                var rsDelay = chrome.Delay(delayTime);
                if (rsDelay.IsError()) return rsDelay;

                Actions action = new(chrome.ChromeDriver);
                action.MoveToElement(element).Perform();
                chrome.Delay(timeHover);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                string msg = $"Lỗi khi hover element {selecttorContent}";
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error<IWebElement>(msg);
            }
        }
        public static Result HoverRandom(this ChromeBrowser chrome, SelectorType selectoryType, List<string> listSelecttorContent, int timeHover = 4, int delayTime = 1)
        {
            try
            {
                var rsChromeStatus = chrome.IsOpen();
                if (rsChromeStatus.IsError()) return Result.Error(rsChromeStatus.Message);

                var rsGetElement = chrome.GetAllElement(selectoryType, listSelecttorContent);
                if (rsGetElement.IsError()) return rsGetElement;

                List<IWebElement> listElement = rsGetElement.Data;
                int indexRandom = _rd.Next(0, listElement.Count);

                IWebElement element = listElement[indexRandom];
                var rsScrollToElement = chrome.ScrollToElement(element);
                if (rsScrollToElement.IsError()) return rsScrollToElement;

                var rsDelay = chrome.Delay(delayTime);
                if (rsDelay.IsError()) return rsDelay;

                Actions action = new(chrome.ChromeDriver);
                action.MoveToElement(element).Perform();
                chrome.Delay(timeHover);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                string msg = $"Lỗi khi hover element {listSelecttorContent}";
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error<IWebElement>(msg);
            }
        }
        public static Result SendKeys(this ChromeBrowser chrome, SelectorType selectoryType, string selecttorContent, string content, SendKeysType sendType = SendKeysType.Patse, int index = 0, bool isClick = true)
        {
            try
            {
                var rsChromeStatus = chrome.IsOpen();
                if (rsChromeStatus.IsError()) return Result.Error(rsChromeStatus.Message);

                var rsGetElement = chrome.GetElement(selectoryType, selecttorContent, index);
                if (rsGetElement.IsError()) return rsGetElement;

                IWebElement element = rsGetElement.Data;
                var rsScrollToElement = chrome.ScrollToElement(element);
                if (rsScrollToElement.IsError()) return rsScrollToElement;

                if (isClick)
                {
                    element.Click();
                }

                switch (sendType)
                {
                    case SendKeysType.Patse:
                        element.SendKeys(content);
                        break;
                    case SendKeysType.SendCharacter:
                        for (int i = 0; i < content.Length; i++)
                        {
                            var text = content[i];
                            var c = text == '\n' ? Keys.Shift + Keys.Enter : text.ToString();
                            element.SendKeys(c);
                            chrome.Delay(0.01);
                        }
                        break;
                }

                return Result.OK;
            }
            catch (Exception ex)
            {
                var msg = "Send keys thất bại";
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error(msg);
            }
        }
        public static Result SendKeys(this ChromeBrowser chrome, SelectorType selectoryType, List<string> listSelecttorContent, string content, SendKeysType sendType = SendKeysType.Patse, int index = 0, bool isClick = true)
        {
            try
            {
                var rsChromeStatus = chrome.IsOpen();
                if (rsChromeStatus.IsError()) return Result.Error(rsChromeStatus.Message);

                var rsGetElement = chrome.GetElement(selectoryType, listSelecttorContent, index);
                if (rsGetElement.IsError()) return rsGetElement;

                IWebElement element = rsGetElement.Data;
                var rsScrollToElement = chrome.ScrollToElement(element);
                if (rsScrollToElement.IsError()) return rsScrollToElement;

                if (isClick)
                {
                    element.Click();
                }

                switch (sendType)
                {
                    case SendKeysType.Patse:
                        element.SendKeys(content);
                        break;
                    case SendKeysType.SendCharacter:
                        for (int i = 0; i < content.Length; i++)
                        {
                            var text = content[i];
                            var c = text == '\n' ? Keys.Shift + Keys.Enter : text.ToString();
                            element.SendKeys(c);
                            chrome.Delay(0.01);
                        }
                        break;
                }

                return Result.OK;
            }
            catch (Exception ex)
            {
                var msg = "Send keys thất bại";
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error(msg);
            }
        }
        public static Result ClearText(this ChromeBrowser chrome, SelectorType selectoryType, string selecttorContent, int index = 0)
        {
            try
            {
                var rsGetElement = chrome.GetElement(selectoryType, selecttorContent, index);
                if (rsGetElement.IsError()) return rsGetElement;

                IWebElement element = rsGetElement.Data;
                var rsScrollToElement = chrome.ScrollToElement(element);
                if (rsScrollToElement.IsError()) return rsScrollToElement;

                element.Clear();

                return Result.OK;
            }
            catch (Exception ex)
            {
                var msg = $"ClearText thất bại [{selecttorContent}]";
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error(msg);
            }
        }
        public static Result ClearText(this ChromeBrowser chrome, SelectorType selectoryType, List<string> listSelecttorContent, int index = 0)
        {
            try
            {
                var rsGetElement = chrome.GetElement(selectoryType, listSelecttorContent, index);
                if (rsGetElement.IsError()) return rsGetElement;

                IWebElement element = rsGetElement.Data;
                var rsScrollToElement = chrome.ScrollToElement(element);
                if (rsScrollToElement.IsError()) return rsScrollToElement;

                element.Clear();

                return Result.OK;
            }
            catch (Exception ex)
            {
                var msg = $"ClearText thất bại [{(listSelecttorContent != null && listSelecttorContent.Any() ? string.Join(", ", listSelecttorContent) : "none")}]";
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error(msg);
            }
        }
        public static Result ClearWithBackSpace(this ChromeBrowser chrome, SelectorType selectoryType, string selecttorContent, int index = 0)
        {
            try
            {
                var rsChromeStatus = chrome.IsOpen();
                if (rsChromeStatus.IsError()) return Result.Error(rsChromeStatus.Message);

                var rsGetElement = chrome.GetElement(selectoryType, selecttorContent, index);
                if (rsGetElement.IsError()) return rsGetElement;

                IWebElement element = rsGetElement.Data;
                var rsScrollToElement = chrome.ScrollToElement(element);
                if (rsScrollToElement.IsError()) return rsScrollToElement;

                string text = element.GetAttribute("value");
                Actions actions = new Actions(chrome.ChromeDriver);
                for (int i = 0; i < text.Length; i++)
                {
                    actions.SendKeys(Keys.Backspace).Perform();
                }
                return Result.OK;
            }
            catch (Exception ex)
            {
                var msg = "ClearWithBackSpace thất bại";
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error(msg);
            }

        }
        public static Result ClearWithBackSpace(this ChromeBrowser chrome, SelectorType selectoryType, List<string> listSelecttorContent, int index = 0)
        {
            try
            {
                var rsChromeStatus = chrome.IsOpen();
                if (rsChromeStatus.IsError()) return Result.Error(rsChromeStatus.Message);

                var rsGetElement = chrome.GetElement(selectoryType, listSelecttorContent, index);
                if (rsGetElement.IsError()) return rsGetElement;

                IWebElement element = rsGetElement.Data;
                var rsScrollToElement = chrome.ScrollToElement(element);
                if (rsScrollToElement.IsError()) return rsScrollToElement;

                string text = element.GetAttribute("value");
                Actions actions = new Actions(chrome.ChromeDriver);
                for (int i = 0; i < text.Length; i++)
                {
                    actions.SendKeys(Keys.Backspace).Perform();
                }
                return Result.OK;
            }
            catch (Exception ex)
            {
                var msg = "ClearWithBackSpace thất bại";
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error(msg);
            }

        }
        public static Result ClearTextAndSendKeys(this ChromeBrowser chrome, SelectorType selectoryType, string selecttorContent, string content, SendKeysType sendType = SendKeysType.Patse, int index = 0, bool isClick = true)
        {
            var clear = chrome.ClearText(selectoryType, selecttorContent, index);
            if (clear.IsError()) return clear;
            chrome.Delay(1);
            return chrome.SendKeys(selectoryType, selecttorContent, content, sendType, index, isClick);
        }
        public static Result ClearTextAndSendKeys(this ChromeBrowser chrome, SelectorType selectoryType, List<string> selecttorContent, string content, SendKeysType sendType = SendKeysType.Patse, int index = 0, bool isClick = true)
        {
            var clear = chrome.ClearText(selectoryType, selecttorContent, index);
            if (clear.IsError()) return clear;
            chrome.Delay(1);
            return chrome.SendKeys(selectoryType, selecttorContent, content, sendType, index, isClick);
        }

        public static Result ClearTextAndSendKeyAndEnter(this ChromeBrowser chrome, SelectorType selectoryType, string selecttorContent, string content, SendKeysType sendType = SendKeysType.Patse, int index = 0, bool isClick = true)
        {
            var clear = chrome.ClearText(selectoryType, selecttorContent, index);
            if (clear.IsError()) return clear;
            chrome.Delay(1);
            clear = chrome.SendKeys(selectoryType, selecttorContent, content, sendType, index, isClick);
            if (clear.IsError()) return clear;
            return chrome.SendKeys(selectoryType, selecttorContent, Keys.Enter, sendType, index);
        }
        public static Result ClearTextAndSendKeyAndEnter(this ChromeBrowser chrome, SelectorType selectoryType, List<string> selecttorContent, string content, SendKeysType sendType = SendKeysType.Patse, int index = 0, bool isClick = true)
        {
            var clear = chrome.ClearText(selectoryType, selecttorContent, index);
            if (clear.IsError()) return clear;
            chrome.Delay(1);
            clear = chrome.SendKeys(selectoryType, selecttorContent, content, sendType, index, isClick);
            if (clear.IsError()) return clear;
            return chrome.SendKeys(selectoryType, selecttorContent, Keys.Enter, sendType, index);
        }
        public static Result<string> GetAttibuteValueFromElement(this ChromeBrowser chrome, SelectorType selectoryType, string selecttorContent, string attribute, int index = 0)
        {
            try
            {
                var rsGetElement = chrome.GetElement(selectoryType, selecttorContent, index);
                if (rsGetElement.IsError()) return Result.Error<string>(rsGetElement.Message);

                IWebElement element = rsGetElement.Data;

                string attrValue = element.GetAttribute(attribute);

                return Result.Ok(attrValue);
            }
            catch (Exception ex)
            {
                var msg = "GetAttibuteFromElement thất bại";
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error<string>(msg);
            }
        }
        public static Result<string> GetAttibuteValueFromElement(this ChromeBrowser chrome, SelectorType selectoryType, List<string> listSelecttorContent, string attribute, int index = 0)
        {
            try
            {
                var rsGetElement = chrome.GetElement(selectoryType, listSelecttorContent, index);
                if (rsGetElement.IsError()) return Result.Error<string>(rsGetElement.Message);

                IWebElement element = rsGetElement.Data;

                string attrValue = element.GetAttribute(attribute);

                return Result.Ok(attrValue);
            }
            catch (Exception ex)
            {
                var msg = $"GetAttibuteFromElement [{(listSelecttorContent != null && listSelecttorContent.Any() ? string.Join(", ", listSelecttorContent) : "none")}] thất bại";
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error<string>(msg);
            }
        }
        //public static Result<Size> GetSizeElement(this ChromeBrowser chrome, SelectorType selectoryType, string selecttorContent, int index = 0)
        //{
        //    try
        //    {
        //        var rsGetElement = chrome.GetElement(selectoryType, selecttorContent, index);
        //        if (rsGetElement.IsError()) return Result.Error<Size>(rsGetElement.Message);

        //        IWebElement element = rsGetElement.Data;

        //        return Result.Ok(element.Size);
        //    }
        //    catch (Exception ex)
        //    {
        //        var msg = $"GetSizeElement [{selecttorContent}] thất bại";
        //        Log4NetSyncController.LogException(ex, msg);
        //        return Result.Error<Size>(msg);
        //    }
        //}
        //public static Result<Size> GetSizeElement(this ChromeBrowser chrome, SelectorType selectoryType, List<string> listSelecttorContent, int index = 0)
        //{
        //    try
        //    {
        //        var rsGetElement = chrome.GetElement(selectoryType, listSelecttorContent, index);
        //        if (rsGetElement.IsError()) return Result.Error<Size>(rsGetElement.Message);

        //        IWebElement element = rsGetElement.Data;

        //        return Result.Ok(element.Size);
        //    }
        //    catch (Exception ex)
        //    {
        //        var msg = $"GetSizeElement [{(listSelecttorContent != null && listSelecttorContent.Any() ? string.Join(", ", listSelecttorContent) : "none")}] thất bại";
        //        Log4NetSyncController.LogException(ex, msg);
        //        return Result.Error<Size>(msg);
        //    }
        //}
        public static Result Refresh(this ChromeBrowser chrome, int delayTime)
        {
            try
            {
                var rsChromeStatus = chrome.IsOpen();
                if (rsChromeStatus.IsError()) return Result.Error(rsChromeStatus.Message);

                chrome.ChromeDriver.Navigate().Refresh();
                var rs = chrome.Delay(delayTime);
                return rs;
            }
            catch (Exception ex)
            {
                var msg = "Refresh thất bại";
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error(msg);
            }
        }
        public static Result WaitForPageLoad(this ChromeBrowser chrome, int timeoutInSeconds = 10)
        {
            try
            {
                var rsChromeStatus = chrome.IsOpen();
                if (rsChromeStatus.IsError()) return Result.Error(rsChromeStatus.Message);

                WebDriverWait wait = new WebDriverWait(chrome.ChromeDriver, TimeSpan.FromSeconds(timeoutInSeconds));
                wait.Until(driver => ((IJavaScriptExecutor)driver)
                    .ExecuteScript("return document.readyState").ToString() == "complete");
                return Result.OK;
            }
            catch (Exception ex)
            {
                var msg = "Chờ page load thất bại";
                Log4NetSyncController.LogException(ex, msg);
                return Result.Exception(msg, ex);
            }
        }
        public static Result WaitForElement(this ChromeBrowser chrome, SelectorType selectorType, string selectorContent, int index = 0, int timeoutInSeconds = 10)
        {
            try
            {
                var rsChromeStatus = chrome.IsOpen();
                if (rsChromeStatus.IsError()) return Result.Error<IWebElement>(rsChromeStatus.Message);

                var wait = new WebDriverWait(chrome.ChromeDriver, TimeSpan.FromSeconds(timeoutInSeconds));

                bool isElementFound = wait.Until(driver =>
                {
                    try
                    {
                        var result = chrome.GetElement(selectorType, selectorContent, index);
                        return result.IsOk(); // Trả về true nếu phần tử tìm thấy
                    }
                    catch (NoSuchElementException)
                    {
                        return false; // Phần tử chưa có trong DOM
                    }
                    catch (StaleElementReferenceException)
                    {
                        return false; // Phần tử bị thay đổi hoặc mất
                    }
                });

                return isElementFound ? Result.OK : Result.Error("Phần tử không xuất hiện sau " + timeoutInSeconds + " giây");
            }
            catch (WebDriverTimeoutException)
            {
                return Result.Error("Timeout: Phần tử không xuất hiện sau " + timeoutInSeconds + " giây");
            }
            catch (Exception ex)
            {
                var msg = "Lỗi khi đợi phần tử: " + ex.Message;
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error(msg);
            }
        }
        public static Result WaitForElement(this ChromeBrowser chrome, SelectorType selectorType, List<string> listSelecttorContent, int index = 0, int timeoutInSeconds = 10)
        {
            try
            {
                var rsChromeStatus = chrome.IsOpen();
                if (rsChromeStatus.IsError()) return Result.Error<IWebElement>(rsChromeStatus.Message);

                var wait = new WebDriverWait(chrome.ChromeDriver, TimeSpan.FromSeconds(timeoutInSeconds));

                bool isElementFound = wait.Until(driver =>
                {
                    try
                    {
                        var result = chrome.GetElement(selectorType, listSelecttorContent, index);
                        return result.IsOk(); // Trả về true nếu phần tử tìm thấy
                    }
                    catch (NoSuchElementException)
                    {
                        return false; // Phần tử chưa có trong DOM
                    }
                    catch (StaleElementReferenceException)
                    {
                        return false; // Phần tử bị thay đổi hoặc mất
                    }
                });

                return isElementFound ? Result.OK : Result.Error("Phần tử không xuất hiện sau " + timeoutInSeconds + " giây");
            }
            catch (WebDriverTimeoutException)
            {
                return Result.Error("Timeout: Phần tử không xuất hiện sau " + timeoutInSeconds + " giây");
            }
            catch (Exception ex)
            {
                var msg = "Lỗi khi đợi phần tử: " + ex.Message;
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error(msg);
            }
        }
        public static Result WaitForElementToDisappear(this ChromeBrowser chrome, SelectorType selectorType, string selectorContent, int index = 0, int timeoutInSeconds = 10)
        {
            try
            {
                var rsChromeStatus = chrome.IsOpen();
                if (rsChromeStatus.IsError()) return Result.Error<IWebElement>(rsChromeStatus.Message);

                var wait = new WebDriverWait(chrome.ChromeDriver, TimeSpan.FromSeconds(timeoutInSeconds));

                bool isElementGone = wait.Until(driver =>
                {
                    try
                    {
                        var result = chrome.GetElement(selectorType, selectorContent, index);
                        return !result.IsOk(); // Trả về true nếu phần tử không còn tồn tại
                    }
                    catch (NoSuchElementException)
                    {
                        return true; // Phần tử đã bị xóa khỏi DOM
                    }
                    catch (StaleElementReferenceException)
                    {
                        return true; // Phần tử đã mất khỏi DOM
                    }
                });

                return isElementGone ? Result.OK : Result.Error("Phần tử vẫn tồn tại sau " + timeoutInSeconds + " giây");
            }
            catch (WebDriverTimeoutException)
            {
                return Result.Error("Timeout: Phần tử vẫn tồn tại sau " + timeoutInSeconds + " giây");
            }
            catch (Exception ex)
            {
                var msg = "Lỗi khi đợi phần tử biến mất: " + ex.Message;
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error(msg);
            }
        }
        public static Result WaitForElementToDisappear(this ChromeBrowser chrome, SelectorType selectorType, List<string> listSelecttorContent, int index = 0, int timeoutInSeconds = 10)
        {
            try
            {
                var rsChromeStatus = chrome.IsOpen();
                if (rsChromeStatus.IsError()) return Result.Error<IWebElement>(rsChromeStatus.Message);

                var wait = new WebDriverWait(chrome.ChromeDriver, TimeSpan.FromSeconds(timeoutInSeconds));

                bool isElementGone = wait.Until(driver =>
                {
                    try
                    {
                        var result = chrome.GetElement(selectorType, listSelecttorContent, index);
                        return !result.IsOk(); // Trả về true nếu phần tử không còn tồn tại
                    }
                    catch (NoSuchElementException)
                    {
                        return true; // Phần tử đã bị xóa khỏi DOM
                    }
                    catch (StaleElementReferenceException)
                    {
                        return true; // Phần tử đã mất khỏi DOM
                    }
                });

                return isElementGone ? Result.OK : Result.Error("Phần tử vẫn tồn tại sau " + timeoutInSeconds + " giây");
            }
            catch (WebDriverTimeoutException)
            {
                return Result.Error("Timeout: Phần tử vẫn tồn tại sau " + timeoutInSeconds + " giây");
            }
            catch (Exception ex)
            {
                var msg = "Lỗi khi đợi phần tử biến mất: " + ex.Message;
                Log4NetSyncController.LogException(ex, msg);
                return Result.Error(msg);
            }
        }
    }
}
