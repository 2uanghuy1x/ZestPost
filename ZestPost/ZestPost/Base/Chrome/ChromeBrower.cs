using OpenQA.Selenium.Chrome;
using System.Diagnostics;
using static ZestPost.Base.Controller.IpSyncController;

namespace ZestPost.Base
{
    public class ChromeBrowser
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

    }
}
