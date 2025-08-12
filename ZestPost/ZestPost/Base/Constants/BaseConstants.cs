namespace ZestPost.Base.Constants
{
    public class BaseConstants
    {
        public const string CORE_SETTING_FILE_NAME = "core-setting";
        public static readonly Dictionary<string, string> CORE_SETTING_CHROME = new Dictionary<string, string>()
            {
                { "--no-sandbox", "" },
                { "--no-first-run", "" },
                { "--no-crashpad", "" },
                { "--disable-crashpad", "" },
                { "--metrics-recording-only", "" },
                { "--no-default-browser-check", "" },
                { "--disable-features", "FlashDeprecationWarning,EnablePasswordsAccountStorage,CalculateNativeWinOcclusion,OptimizationHints,AcceleratedVideoDecode,ChromeLabs,ReadLater,ChromeWhatsNewUI,TrackingProtection3pcd" },
                { "--disable-crash-reporter", "" },
                { "--disable-background-timer-throttling", "" },
                { "--disable-backgrounding-occluded-windows", "" },
                { "--disable-renderer-backgrounding", "" },
                { "--hide-crash-restore-bubble", "" },
                { "--disable-background-mode", "" },
                { "--disable-timer-throttling", "" },
                { "--disable-render-backgrounding", "" },
                { "--disable-background-media-suspend", "" },
                { "--disable-external-intent-requests", "" },
                { "--disable-field-trial-config", "" },
                { "--enable-features=PdfOopif", "" },
                { "--disable-ipc-flooding-protection", "" },
                { "--enable-unsafe-webgpu", "" }
            };
        public static readonly Dictionary<string, string> CORE_SETTING_CHROME_OLD = new Dictionary<string, string>()
            {
                { "--no-sandbox", "" },
                { "--no-first-run", "" },
                { "--no-crashpad", "" },
                { "--disable-crashpad", "" },
                { "--metrics-recording-only", "" },
                { "--no-default-browser-check", "" },
                { "--disable-features", "FlashDeprecationWarning,EnablePasswordsAccountStorage,CalculateNativeWinOcclusion,OptimizationHints,AcceleratedVideoDecode,ChromeLabs,ReadLater,ChromeWhatsNewUI,TrackingProtection3pcd" },
                { "--disable-crash-reporter", "" },
                { "--disable-background-timer-throttling", "" },
                { "--disable-backgrounding-occluded-windows", "" },
                { "--disable-renderer-backgrounding", "" },
                { "--hide-crash-restore-bubble", "" },
                { "--disable-background-mode", "" },
                { "--disable-timer-throttling", "" },
                { "--disable-render-backgrounding", "" },
                { "--disable-background-media-suspend", "" },
                { "--disable-external-intent-requests", "" },
                { "--enable-features=PdfOopif", "" },
                { "--disable-ipc-flooding-protection", "" },
                { "--enable-unsafe-webgpu", "" }
            };
    }
}
