using Microsoft.Extensions.Caching.Memory;

namespace ZestPost.Base
{
    public class AppInfo
    {
        public static string[] SettingLanguage { get; set; }
        public static string BasePath { get; set; }
        public static string ProjectPath { get; set; }
        public static string ProjectName { get; set; }
        public static IMemoryCache Cache { get; set; }
        public static string FolderNameOfSharedLibrary { get; set; }
        public static string DbPath { get; set; }
        public static string PathSyncData { get; set; }
        public static int Day { get; set; } = 0;
        public static string Version { get; set; }
        public static string Timeupdate { get; set; }
        public static string Email { get; set; } = null;
        public static string Password { get; set; } = null;
        public static string Token { get; set; } = null;
        public static string ExpertatToken { get; set; }
        public static string RefreshToken { get; set; } = null;
        public static string ExpertatRefreshToken { get; set; }
        public static string IpMachine { get; set; }
        public static string HardWareId { get; set; }
        public static string Liense { get; set; }
        public static string NewHardWareId { get; set; }
        public static string Storage { get; set; }
        public static string LinkWinrar { get; set; }

        public static string Ram { get; set; }
        public static string Protect { get; set; }
        public static string TikhubApiKey { get; set; }
        public static string RapidApiKey { get; set; }
        public static string PathProfile { get; set; }
        public static JsonConfigBuilder SettingDetect { get; set; }
        public static List<FFPMEG> SettingFfpmeg { get; set; }
        public static string Language { get; set; } = "vi";
        public static bool IsFirstLogin { get; set; } = false;
        public static List<string> DomainMailV2;

        public static EnumPlatFormName PlatFormName;
    }

    public class FFPMEG
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Demo { get; set; }
    }
}
