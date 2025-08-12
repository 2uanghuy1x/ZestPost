
using System.ComponentModel.DataAnnotations;

namespace ZestPost.Base.Enum
{
    public enum StatusChromeAccount
    {
        Empty,
        ChromeClosed,
        LoginWithUserPass,
        LoginWithSelectAccount,
        Checkpoint,
        Logined,
        NoInternet,
        Blocked,
        Noveri
    }

    public enum SelectorType
    {
        ById,
        ByTagName,
        ByXPath,
        ByCssSelector
    }

    public enum SendKeysType
    {
        Patse,
        SendCharacter
    }

    public enum ScrollType
    {
        Up,
        Down,
        Left,
        Right,
    }

    public enum IpType
    {
        NoChange,

        [Display(Name = "Proxy tinsoft")]
        TinisoftProxy,

        [Display(Name = "Mobi proxy")]
        MobiProxy,

        [Display(Name = "TM proxy")]
        TmProxy,

        [Display(Name = "Proxy v6 rotating")]
        ProxyV6Rotating,

        [Display(Name = "WWProxy")]
        WwProxy,

        [Display(Name = "Proxy mart")]
        ProxyMartIpV6Scrile,

        [Display(Name = "Proxy mart")]
        ProxyMartKeyScrile,

        [Display(Name = "Rotate Proxy")]
        RotateProxy,

        [Display(Name = "Proxy Vn")]
        ProxyVn,

        [Display(Name = "Net Proxy")]
        NetProxy,
    }
}
