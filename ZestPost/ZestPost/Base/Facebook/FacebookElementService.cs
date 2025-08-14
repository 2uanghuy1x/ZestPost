using System.Text.RegularExpressions;
using ZestPost.Base.Extension;
using ZestPost.Base.Facebook;

namespace ZestPost.Base
{
    public class FacebookElementService
    {

        private readonly JsonConfigBuilder _coreSettings;
        private readonly LoginBaseController _loginControllerl = new();
        private readonly HotmailController _hotmailController = new();
        //private readonly OneSecMailController _1secmailController = new();
        private List<string> _lstDomain1secmail = new List<string>();
        public FacebookElementService()
        {
            _coreSettings = new JsonConfigBuilder(BaseConstants.CORE_SETTING_FILE_NAME);
        }
        public Result<TAccount> LoginFacebook<TAccount>(ChromeBrowser chromeDriver, TAccount account, CancellationToken token = default) where TAccount : IAccount, IAccountFacebook, IAccountMail
        {
            Result<TAccount> result;
            string urlLogin = "https://www.facebook.com/";
            chromeDriver.GotoURL(urlLogin);

            if (this._coreSettings.GetValueBool(CoreSettingConstants.IS_LOGIN_COOKIES, true) && !string.IsNullOrEmpty(account.Cookie))
            {
                LoginFacebookByCookies(chromeDriver, account, urlLogin);
            }
            if (IsLoginedAccount(chromeDriver))
            {
                var cookieFb = GetDataFacebook(chromeDriver, account);
                if (cookieFb.IsOk()) return Result.Ok(cookieFb.Data);
            }
            result = LoginFacebookByElementV2(chromeDriver, account, token);
            if (result.IsOk())
            {
                return Result.Ok(result.Data);
            }
            return Result.ErrorWithData(result.Message ?? HelperCore.L("core_error_login"), result.Data);
        }
        public Result<TAccount> LoginFacebookByElementV2<TAccount>(ChromeBrowser chromeDriver, TAccount account, CancellationToken token) where TAccount : IAccount, IAccountFacebook, IAccountMail
        {
            int countElement = 0;
            int numReturn = 0;
            int numGet2FA = 0;
            int numMaxGet2Fa = 2;
            int numMaxReturn = 6;
            int numExcuseCapcha = 0;
            int numMaxExcusCapcha = _coreSettings.GetValueInt(CoreSettingConstants.NUM_MAX_SOLVE_CAPTCHA);
            string node = "";
            string nodeChecking = "";
            Result resultLogin = new Result();
            Result resultCapcha = new Result();
            List<string> lstNode = new List<string>();
            List<string> lstNodeChecking = new List<string>();
            List<string> lstUrlCheck = new List<string>();
            List<string> lst_link_errsword = new List<string>();
            try
            {

                int tickCountLogin = Environment.TickCount;
                while (true)
                {
                    numReturn++;
                    if (token.IsStopped()) return Result.Ok(account);

                    if (numReturn > numMaxReturn)
                        goto lb_end;


                    lstNode = new List<string>
                {
                    "[action*=\"login/device-based/validate-pin\"]","[action=\"login/device-based/update-nonce/\"]","a[ajaxify*=\"/login/device-based/turn-on/\"]",
                    "a[ajaxify*=\"/login/device-based/turn-on/\"]","[data-testid=\"parent_approve_consent_button\"] button", "[aria-labelledby=\"manage_cookies_title\"]",
                    "[href*=\"login/?ref=dbl\"]","[data-userid] a","[action*=\"login\"]","input[id*=\"approvals_code\"]","form[method=\"GET\"] input",
                     "[name=\"primary_consent_button\"]", "#checkpointSubmitButton","#account_recovery_initiate_view_label","[id*=\"recaptcha\"]","img[src*=\"captcha\"]","[id=\"arkose-captcha\"]",
                     "[id=\"login_form\"] [name=\"email\"]" , "[method=\"POST\"] [name=\"email\"]"
                };
                    node = chromeDriver.GetElementExistFromList(4, 0.5, lstNode);
                    if (!string.IsNullOrEmpty(node))

                        numReturn = 0;
                    switch (node)
                    {
                        default:
                            lstUrlCheck = new List<string> { "two_step_verification/two_factor", "checkpoint/465803052217681" };
                            if (HelperSync.CheckStringContainKeyword(chromeDriver.GetURL(), lstUrlCheck))

                            {
                                lstNode = new List<string> { "//div[@role=\"button\"]" };
                                node = chromeDriver.GetElementExistFromList(3, 0.5, lstNode);
                                if (!string.IsNullOrEmpty(node))
                                {
                                    countElement = chromeDriver.CountElement(3, 0.5, node);
                                    resultLogin = chromeDriver.Click(SelectorType.ByXPath, node, index: countElement - 1);
                                    if (resultLogin.IsError()) return Result.Error<TAccount>(resultLogin.Message);
                                    chromeDriver.DelayTime(1);

                                    resultLogin = chromeDriver.Click(SelectorType.ByCssSelector, "[role=\"dialog\"] input[value=\"1\"]");
                                    if (resultLogin.IsError()) return Result.Error<TAccount>(resultLogin.Message);
                                    chromeDriver.DelayTime(1);

                                    lstNode = new List<string> { "//div[@role=\"dialog\"]//div[@role=\"button\"]" };
                                    node = chromeDriver.GetElementExistFromList(3, 0.5, lstNode);
                                    if (!string.IsNullOrEmpty(node))
                                    {
                                        countElement = chromeDriver.CountElement(3, 0.5, node);
                                        resultLogin = chromeDriver.Click(SelectorType.ByXPath, node, index: countElement - 1);
                                        if (resultLogin.IsError()) return Result.Error<TAccount>(resultLogin.Message);
                                        chromeDriver.DelayTime(1);

                                    }
                                }
                            }
                            else if (chromeDriver.GetURL().Contains("facebook.com/auth_platform/codesubmit"))

                            {
                                goto lbGetOtpHotmail;
                            }
                            else if (chromeDriver.GetURL().Contains("facebook.com/login/help.php?"))

                            {
                                return Result.ErrorWithData(HelperCore.L("core_error_login_wrong_password"), account);
                            }
                            else if (chromeDriver.GetURL().Contains("/checkpoint"))
                            {
                                return Result.ErrorWithData(HelperCore.L("core_error_account_lock"), account);
                            }
                            else if (IsLoginedAccount(chromeDriver))

                            {
                                goto lb_finish;
                            }
                            goto lb_checking;

                        case "[action*=\"login/device-based/validate-pin\"]":
                            lstNodeChecking = new List<string> { "[action*=\"login/device-based/validate-pin\"] [role*=\"button\"]", "[action *=\"login/device-based/validate-pin\"] [type=\"submit\"]" };
                            nodeChecking = chromeDriver.GetElementExistFromList(4, 0.5, lstNodeChecking);
                            if (!string.IsNullOrEmpty(nodeChecking))
                            {
                                resultLogin = chromeDriver.Click(SelectorType.ByCssSelector, node);
                                if (resultLogin.IsError()) return Result.Error<TAccount>(resultLogin.Message);
                                chromeDriver.Delay(1);

                                resultLogin = chromeDriver.ClearTextAndSendKeyAndEnter(SelectorType.ByCssSelector, node, account.Password, SendKeysType.SendCharacter);
                                if (resultLogin.IsError()) return Result.Error<TAccount>(resultLogin.Message);
                                chromeDriver.DelayTime(3);

                                if (chromeDriver.CheckElements(4, "[name*=\"pass\"]"))
                                {
                                    if (chromeDriver.CheckElements(4, "[action*=\"login/device-based\"] button"))
                                    {
                                        resultLogin = chromeDriver.Click(SelectorType.ByCssSelector, "[action*=\"login/device-based\"] button");
                                        if (resultLogin.IsError()) return Result.Error<TAccount>(resultLogin.Message);
                                        chromeDriver.DelayTime(1);
                                    }

                                    if (chromeDriver.CheckElements(4, "[action=\"login/device-based/update-nonce/\"] button"))
                                    {
                                        resultLogin = chromeDriver.Click(SelectorType.ByCssSelector, "[action=\"login/device-based/update-nonce/\"] button");
                                        if (resultLogin.IsError()) return Result.Error<TAccount>(resultLogin.Message);
                                        chromeDriver.DelayTime(1);
                                    }

                                    lstNodeChecking = new List<string> { "form[action*=\"recover_method=send_emai\"]", "#login_error", "#identify_search_toggle_button", "[href*=\"_login_pw_error\"]", "#login_form [role=\"alert\"]", "#login_form #error_box" };
                                    nodeChecking = chromeDriver.GetElementExistFromList(4, 0.5, lstNodeChecking);
                                    lst_link_errsword = new List<string> { "login/?privacy_mutation_token" };
                                    if (!string.IsNullOrEmpty(nodeChecking) || HelperSync.CheckStringContainKeyword(chromeDriver.GetURL(), lst_link_errsword))
                                    {
                                        resultLogin.Message = HelperCore.L("core_error_login_wrong_password");
                                        goto lb_finish;

                                    }
                                }
                            }
                            goto lb_checking;

                        case "[action=\"login/device-based/update-nonce/\"]":
                            resultLogin = chromeDriver.Click(SelectorType.ByCssSelector, node);
                            if (resultLogin.IsError()) return Result.Error<TAccount>(resultLogin.Message);
                            goto lb_checking;

                        case "a[ajaxify*=\"/login/device-based/turn-on/\"]":
                            resultLogin = chromeDriver.Click(SelectorType.ByCssSelector, node);
                            if (resultLogin.IsError()) return Result.Error<TAccount>(resultLogin.Message);
                            goto lb_checking;

                        case "[data-testid=\"parent_approve_consent_button\"] button":
                            resultLogin = chromeDriver.Click(SelectorType.ByCssSelector, node);
                            if (resultLogin.IsError()) return Result.Error<TAccount>(resultLogin.Message);
                            goto lb_checking;

                        case "a[href*=\"/a/nux/wizard/nav.php?step=\"]":
                            resultLogin = chromeDriver.Click(SelectorType.ByCssSelector, node, delayTime: 2);
                            if (resultLogin.IsError()) return Result.Error<TAccount>(resultLogin.Message);
                            goto lb_checking;

                        case "[href*=\"login/?ref=dbl\"]":
                            resultLogin = chromeDriver.Click(SelectorType.ByCssSelector, node);
                            if (resultLogin.IsError()) return Result.Error<TAccount>(resultLogin.Message);
                            goto lb_checking;

                        case "[name=\"primary_consent_button\"]":
                            countElement = chromeDriver.CountElement(4, 0.5, node);
                            resultLogin = chromeDriver.Click(SelectorType.ByCssSelector, node, countElement - 1, delayTime: -1);
                            if (resultLogin.IsError()) return Result.Error<TAccount>(resultLogin.Message);
                            goto lb_checking;

                        case "[action^=\"/login/account_recovery/name_search/\"]":
                            chromeDriver.ClearTextWithBackSpace(4, node);
                            chromeDriver.DelayTime(0.5);
                            chromeDriver.ClearTextAndSendKeys(SelectorType.ByCssSelector, node + " [name*=\"pass\"]", account.Password ?? "");
                            chromeDriver.DelayTime(1);
                            resultLogin = chromeDriver.Click(SelectorType.ByCssSelector, "[data-testid=\"conf_login_button\"]");
                            if (resultLogin.IsError()) return Result.Error<TAccount>(resultLogin.Message);
                            goto lb_checking;

                        case "[aria-labelledby=\"manage_cookies_title\"]":
                            lstNodeChecking = new List<string> { "[aria-label*=\"Allow all cookies\"][role=\"button\"][tabindex=\"0\"]", "[aria-label*=\"nhận cookies\"][role=\"button\"][tabindex=\"0\"]" };
                            nodeChecking = chromeDriver.GetElementExistFromList(4, 0.5, lstNodeChecking);
                            if (!string.IsNullOrEmpty(nodeChecking))
                            {
                                resultLogin = chromeDriver.Click(SelectorType.ByCssSelector, nodeChecking);
                                if (resultLogin.IsError()) return Result.Error<TAccount>(resultLogin.Message);

                            }
                            goto lb_checking;

                        case "[data-userid] a":
                            resultLogin = chromeDriver.Click(SelectorType.ByCssSelector, node);
                            if (resultLogin.IsError()) return Result.Error<TAccount>(resultLogin.Message);
                            chromeDriver.DelayTime(1);
                            if (chromeDriver.CheckElements(4, "[role=\"dialog\"] [name*=\"pass\"]", 0, 0, "", 0, 1, 0.1))
                            {
                                resultLogin = chromeDriver.ClearWithBackSpace(SelectorType.ByCssSelector, "[role=\"dialog\"] [name*=\"pass\"]");
                                if (resultLogin.IsError()) return Result.Error<TAccount>(resultLogin.Message);
                                chromeDriver.DelayTime(0.5);
                                chromeDriver.ClearTextAndSendKeys(SelectorType.ByCssSelector, "[role=\"dialog\"] [name*=\"pass\"]", account.Password ?? "");
                                chromeDriver.DelayTime(1);
                                if (chromeDriver.CheckElements(4, "[action*=\"login/device-based\"] button", 0, 0, "", 0, 1, 0.1))
                                {
                                    resultLogin = chromeDriver.Click(SelectorType.ByCssSelector, "[action*=\"login/device-based\"] button");
                                    if (resultLogin.IsError()) return Result.Error<TAccount>(resultLogin.Message);
                                    chromeDriver.DelayTime(1);

                                }
                            }
                            goto lb_checking;

                        case "[action*=\"login\"]" or "[id=\"login_form\"] [name=\"email\"]":
                            if (chromeDriver.CheckElements(4, "[data-sigil*=\"login_inner\"] a[href*=\"#\"]"))
                            {
                                resultLogin = chromeDriver.Click(SelectorType.ByCssSelector, "[data-sigil*=\"login_inner\"] a[href*=\"#\"]");
                                if (resultLogin.IsError()) return Result.Error<TAccount>(resultLogin.Message);
                                chromeDriver.DelayTime(01);
                            }

                            lstNodeChecking = new List<string> { "[name*=\"username\"]", "[name*=\"email\"]" };
                            string nodeUid = chromeDriver.GetElementExistFromList(4, 0.5, lstNodeChecking);
                            chromeDriver.ClearTextWithBackSpace(4, nodeUid);
                            resultLogin = chromeDriver.ClearTextAndSendKeys(SelectorType.ByCssSelector, lstNodeChecking, account.Uid, SendKeysType.SendCharacter);
                            chromeDriver.Delay(1);
                            chromeDriver.ClearTextWithBackSpace(4, "[name*=\"pass\"]");
                            resultLogin = chromeDriver.ClearTextAndSendKeyAndEnter(SelectorType.ByCssSelector, "[name*=\"pass\"]", account.Password, SendKeysType.SendCharacter);
                            chromeDriver.Delay(3);

                            lstNodeChecking = new List<string> { "button[name*=\"login\"]", "[data-sigil*=\"m_login_button\"]", "input[data-testid*=\"royal_login_button\"]", "#loginbutton", "input[name*=\"login\"]", };
                            nodeChecking = chromeDriver.GetElementExistFromList(4, 0.5, lstNodeChecking);
                            if (!string.IsNullOrEmpty(nodeChecking))
                            {
                                resultLogin = chromeDriver.Click(SelectorType.ByCssSelector, nodeChecking);
                                if (resultLogin.IsError()) return Result.Error<TAccount>(resultLogin.Message);
                                chromeDriver.DelayTime(2);
                            }

                            if (chromeDriver.CheckElements(4, "[name*=\"pass\"]"))
                            {
                                resultLogin.Message = HelperCore.L("core_error_login_wrong_password");
                                return Result.ErrorWithData(resultLogin.Message, account);
                            }

                            lstNodeChecking = new List<string> { "form[action*=\"recover_method=send_emai\"]", "#login_error", "#identify_search_toggle_button", "[href*=\"_login_pw_error\"]", "#login_form [role=\"alert\"]", "#login_form #error_box" };
                            nodeChecking = chromeDriver.GetElementExistFromList(4, 0.5, lstNodeChecking);
                            lst_link_errsword = new List<string> { "login/?privacy_mutation_token" };
                            if (!string.IsNullOrEmpty(nodeChecking) || HelperSync.CheckStringContainKeyword(chromeDriver.GetURL(), lst_link_errsword))
                            {

                                resultLogin.Message = HelperCore.L("core_error_login_warning_Unlock956");
                                return Result.ErrorWithData(resultLogin.Message, account);
                            }


                            goto lb_checking;

                        case "input[id*=\"approvals_code\"]":
                            goto lbGet2fa;

                        case "form[method=\"GET\"] input":
                            goto lbGet2fa;

                        case "#checkpointSubmitButton":
                            if (chromeDriver.CheckElements(4, "[name=\"checkpointU2Fauth\"]"))
                            {
                                chromeDriver.GotoURL("https://www.facebook.com/checkpoint/?next&no_fido=true");
                                goto lb_checking;
                            }
                            goto lb_checking;

                        case "[id*=\"recaptcha\"]":
                            chromeDriver.DelayTime(10);
                            for (int i = 1; i < 5; i++)
                            {
                                if (chromeDriver.CheckElements(4, node))
                                {
                                    chromeDriver.DelayTime(2);
                                    chromeDriver.Click(SelectorType.ByCssSelector, "[aria-label=\"Continue\"]");
                                    continue;
                                }
                                break;
                            }
                            numExcuseCapcha++;
                            if (numExcuseCapcha > numMaxExcusCapcha)
                            {
                                return Result.ErrorWithData(HelperCore.L("core_error_login_warning_maxSolveRecaptcha"), account);
                            }
                            break;

                        case "img[src*=\"captcha\"]":
                            numExcuseCapcha++;
                            if (numExcuseCapcha > numMaxExcusCapcha)
                            {
                                resultLogin.Message = HelperCore.L("core_error_login_warning_maxSolveRecaptcha");
                                return Result.ErrorWithData(resultLogin.Message, account);
                            }

                            //resultCapcha = capchaController.SolveNormalCaptcha(chromeDriver, token);
                            //if (resultCapcha.IsError())
                            //{
                            //    resultLogin.Message = HelperCore.L("core_error_login_capcha");
                            //    return Result.ErrorWithData(resultLogin.Message, account);
                            //}
                            goto lb_checking;

                        //funCaptcha
                        case "[id=\"arkose-captcha\"]":
                            chromeDriver.DelayTime(15);
                            for (int i = 1; i < 5; i++)
                            {
                                if (chromeDriver.CheckElements(4, node))
                                {
                                    chromeDriver.DelayTime(2);
                                    int indexBtn = chromeDriver.CountElement(4, 1, "[role=\"button\"]");
                                    resultLogin = chromeDriver.Click(SelectorType.ByCssSelector, "[role=\"button\"]", indexBtn - 1);
                                    //if (resultLogin.IsError()) return Result.Error<TAccount>(resultLogin.Message);
                                    chromeDriver.DelayTime(1);
                                    continue;
                                }
                                break;
                            }
                            numExcuseCapcha++;
                            if (numExcuseCapcha > numMaxExcusCapcha)
                            {
                                resultLogin.Message = HelperCore.L("core_error_login_warning_maxSolveRecaptcha");
                                goto lb_finish;
                            }
                            goto lb_checking;

                        case "[href=\"/login/identify/\"]":
                            resultLogin.Message = HelperCore.L("core_error_login");
                            return Result.ErrorWithData(resultLogin.Message, account);

                        case "#account_recovery_initiate_view_label":
                            resultLogin.Message = HelperCore.L("core_error_login_wrong_password");
                            return Result.ErrorWithData(resultLogin.Message, account);
                    }

                lbGet2fa:
                    if (string.IsNullOrEmpty(account.Privatekey))
                    {
                        resultLogin.Message = HelperCore.L("core_error_login_no_2FA_code");
                        return Result.ErrorWithData(resultLogin.Message, account);
                    }

                lbGetOtpHotmail:
                    numGet2FA++;
                    if (numGet2FA > numMaxGet2Fa)
                    {
                        resultLogin.Message = HelperCore.L("core_error_login_2FA_code_faild");
                        return Result.ErrorWithData(resultLogin.Message, account);
                    }

                    string code_verify = null;
                    List<string> lst_url_verify_mail = new List<string> { "facebook.com/auth_platform/codesubmit", "facebook.com/checkpoint/465803052217681" };
                    if (HelperSync.CheckStringContainKeyword(chromeDriver.GetURL(), lst_url_verify_mail))
                    {
                        //verify by hotmail
                        //_lstDomain1secmail = AppInfo.SettingDetect.GetValue("domain_1sec_mail").Split('|').ToList();
                        //if (HelperSync.CheckStringContainKeyword(account.Email ?? "", _lstDomain1secmail))
                        //{
                        //    code_verify = _1secmailController.ReadMessageMail(1, account.Email ?? "", token);
                        //}
                        //else
                        //{
                        //    chromeDriver.DelayTime(10);
                        //    code_verify = _hotmailController.get_Code_Mail_WithOAuthentV3(3, account.Email, account.Passmail, 120, account.Uid, token);
                        //    if (code_verify.IsEmpty())
                        //    {
                        //        MailService mailService = new MailService();
                        //        code_verify = (mailService.GetCodeMailWithOAuthentV2(3, account, 120, "", token)).data;
                        //    }
                        //}
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(account.Privatekey))
                        {
                            code_verify = AsyncHelper.RunSync(() => _loginControllerl.Get2FAFacebook(account.Privatekey));
                        }
                    }

                    if (string.IsNullOrEmpty(code_verify))
                    {
                        resultLogin.Message = HelperCore.L("core_error_login_2FA_code_faild");
                        return Result.ErrorWithData(resultLogin.Message, account);
                    }

                    lstNodeChecking = new List<string> { "input[id*=\"approvals_code\"]", "form[method=\"GET\"] input", "form input[name=\"email\"]" };
                    resultLogin = chromeDriver.ClearTextAndSendKeyAndEnter(SelectorType.ByCssSelector, lstNodeChecking, code_verify, SendKeysType.SendCharacter);
                    if (resultLogin.IsError()) return Result.Error<TAccount>(resultLogin.Message);
                    chromeDriver.DelayTime(5);

                    for (int i = 0; i < 3; i++)
                    {
                        if (token.IsStopped()) return Result.Ok(account);
                        string currentUrl = chromeDriver.GetURL();
                        List<string> lstUrl2FAUI = new List<string> { "two_step_verification/two_factor", "two_factor/remember_browser" };
                        if (HelperSync.CheckStringContainKeyword(currentUrl, lstUrl2FAUI))
                        {
                            lstNode = new List<string> { "//div[@role=\"dialog\"]//div[@role=\"button\"]" };
                            node = chromeDriver.GetElementExistFromList(3, 0.5, lstNode);
                            if (!string.IsNullOrEmpty(node))
                            {
                                int countBtnClick = chromeDriver.CountElement(3, 0.5, node);
                                resultLogin = chromeDriver.Click(SelectorType.ByXPath, node, index: countBtnClick == 5 ? 1 : 0);
                                if (resultLogin.IsError()) return Result.Error<TAccount>(resultLogin.Message);
                                chromeDriver.DelayTime(3);
                            }

                            lstNode = new List<string> { "//*[@id=\"checkpointSubmitButton\"]", "//form[@method=\"GET\"] /following-sibling::div" };
                            node = chromeDriver.GetElementExistFromList(3, 0.5, lstNode);
                            if (!string.IsNullOrEmpty(node))
                            {
                                if (node.Equals("//form[@method=\"GET\"] /following-sibling::div"))
                                {
                                    int countButton = chromeDriver.CountElement(3, 0.5, node);
                                    resultLogin = chromeDriver.Click(SelectorType.ByXPath, node, index: countButton > 1 ? 0 : countButton - 1, delayTime: 3);
                                    if (resultLogin.IsError())
                                    {
                                        resultLogin = chromeDriver.Click(SelectorType.ByXPath, "//div[@role=\"dialog\"]//div[@role=\"button\"]");
                                        if (resultLogin.IsError()) return Result.Error<TAccount>(resultLogin.Message);
                                        continue;
                                    }

                                    chromeDriver.DelayTime(3);
                                }
                                else
                                {
                                    resultLogin = chromeDriver.Click(SelectorType.ByXPath, node, delayTime: 3);
                                    if (resultLogin.IsError()) return Result.Error<TAccount>(resultLogin.Message);
                                    chromeDriver.DelayTime(1);
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        else if (currentUrl.Contains("facebook.com/auth_platform/codesubmit"))
                        {
                            lstNode = new List<string> { "//form[@method=\"POST\"] /following-sibling::div[2]" };
                            node = chromeDriver.GetElementExistFromList(3, 0.5, lstNode);
                            if (!string.IsNullOrEmpty(node))
                            {
                                int countButton = chromeDriver.CountElement(3, 0.5, node);
                                resultLogin = chromeDriver.Click(SelectorType.ByXPath, node, index: countButton > 1 ? 0 : countButton - 1, delayTime: 3);
                                if (resultLogin.IsError()) return Result.Error<TAccount>(resultLogin.Message);
                                chromeDriver.DelayTime(1);
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            lstNode = new List<string> { "//*[@id=\"checkpointSubmitButton\"]", "#checkpointSubmitButton [type=\"submit\"]", "[name=\"submit[Get Started]\"]", "#checkpointBottomBar [name=\"submit[Continue]\"]" };
                            node = chromeDriver.GetElementExistFromList(3, 0.5, lstNode);
                            if (!string.IsNullOrEmpty(node))
                            {
                                resultLogin = chromeDriver.Click(SelectorType.ByXPath, node, delayTime: 3);
                                if (resultLogin.IsError()) return Result.Error<TAccount>(resultLogin.Message);
                                chromeDriver.DelayTime(1);
                            }
                            else
                            {
                                break;
                            }
                        }

                    }

                    if (chromeDriver.CheckElements(4, "input[type=\"text\"]"))
                    {
                        return Result.ErrorWithData(HelperCore.L("core_error_login_unknow"), account);
                    }

                    nodeChecking = chromeDriver.GetElementExistFromList(4, 0.5, lstNodeChecking);
                    if (nodeChecking.IsNotEmpty())
                    {
                        List<string> lstRegetOtpElement = new List<string> { "//form[@method=\"POST\"] /following-sibling::div[1] //*[@role=\"button\"]" };
                        string regetElement = chromeDriver.GetElementExistFromList(3, 0.5, lstRegetOtpElement);
                        if (regetElement.IsNotEmpty())
                        {
                            resultLogin = chromeDriver.Click(SelectorType.ByXPath, node, delayTime: 3);
                            if (resultLogin.IsError()) return Result.Error<TAccount>(resultLogin.Message);
                            chromeDriver.DelayTime(5);
                            continue;
                        }
                    }
                    goto lb_checking;

                lb_checkkpoint:
                    if (chromeDriver.GetURL().Contains("checkpoint/601051028565049"))
                    {
                        //accept notify automation action
                        lstNodeChecking = new List<string> { "[role=\"main\"] [role=\"button\"]", "form[action*=\"/checkpoint/601051028565049\"] [type=\"submit\"]" };
                        nodeChecking = chromeDriver.GetElementExistFromList(4, 0.5, lstNodeChecking);
                        if (!string.IsNullOrEmpty(nodeChecking))
                        {
                            resultLogin = chromeDriver.Click(SelectorType.ByXPath, nodeChecking, delayTime: 2);
                            if (resultLogin.IsError()) return Result.Error<TAccount>(resultLogin.Message);
                            chromeDriver.DelayTime(1);
                            goto lb_checking;
                        }
                    }
                    else
                    {
                        lstUrlCheck = new List<string> { "1501092823525282", "828281030927956", "checkpoint/disabled/" };
                        lstNodeChecking = new List<string> { "#SupportFormRow\\\\.382907505152522", "[href=\"/help/117450615006715\"]" };
                        nodeChecking = chromeDriver.GetElementExistFromList(4, 0.5, lstNodeChecking);
                        if (HelperSync.CheckStringContainKeyword(chromeDriver.GetURL(), lstUrlCheck) || string.IsNullOrEmpty(nodeChecking))
                        {
                            resultLogin.Message = HelperCore.L("core_error_account_lock");
                            return Result.ErrorWithData<TAccount>(resultLogin.Message, account);
                        }
                    }


                lb_checking:
                    if (token.IsCancellationRequested)
                        goto lb_finish;

                    string urlCheckLang = chromeDriver.GetURL();
                    if (!urlCheckLang.Contains("en-gb"))
                    {
                        urlCheckLang = urlCheckLang.Replace("www.facebook.com", "en-gb.facebook.com");
                        chromeDriver.GotoURL(urlCheckLang);
                    }

                    if ((Environment.TickCount - tickCountLogin < 90 * 1000))
                    {
                        chromeDriver.DelayTime(1.0);
                        continue;
                    }
                    else
                    {
                        goto lb_finish;
                    }
                }
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
        lb_finish:

            if (IsLoginedAccount(chromeDriver))
            {
                var cookieFb = GetDataFacebook(chromeDriver, account);
                if (cookieFb.IsOk()) return Result.Ok(cookieFb.Data);
            }

        lb_end:
            return Result.ErrorWithData(resultLogin.Message, account);
        }
        public Result<TAccount> GetDataFacebook<TAccount>(ChromeBrowser chromeDriver, TAccount account) where TAccount : IAccount, IAccountFacebook
        {
            Result resultLogin = new Result { Code = "400" };
            string cookies = chromeDriver.GetCookieFromChrome(".facebook.com");
            if (cookies.Contains("c_user"))
            {
                account.Cookie = cookies;
                account.FbDtsg = GetFbDtsg(chromeDriver, account);
                if (IsLoginedAccount(chromeDriver))
                {
                    string uid_page_now = chromeDriver.ExecuteScript("return require('CurrentUserInitialData').USER_ID;").ToString();
                    if (!account.Uid.Contains(uid_page_now)) SwitchPageHTTP(chromeDriver, uid_page_now, account.Uid);
                    bool change = ChangeLanguageFacebook(chromeDriver, account.Uid);
                    if (!change)
                    {
                        resultLogin.Message = HelperCore.L("core_error_login_change_language_error");
                        return Result.ErrorWithData(resultLogin.Message, account);
                    }
                    return Result.Ok(account);
                }
            }
            return Result.ErrorWithData(HelperCore.L(""), account);
        }
        public Result LoginFacebookByCookies(ChromeBrowser chromedriver, IAccount acc, string urlLogin)
        {
            Result result = new Result();
            try
            {
                chromedriver.GotoURL(urlLogin);
                if (!string.IsNullOrEmpty(acc.Cookie))
                {
                    if (chromedriver.AddCookieIntoChrome(acc.Cookie, ".facebook.com"))
                    {
                        chromedriver.Refresh(2);
                    }
                }
                CheckDetectAutomation(chromedriver);
            }
            catch (Exception ex)
            {
                Log4NetSyncController.LogException(ex, "");
                return Result.Exception<string>("Lỗi không Login được bằng Cookies", ex);
            }
            return Result.OK;
        }
        private bool SwitchPageHTTP(ChromeBrowser chromedriver, string pageIdNow, string pageIdSwitch)
        {
            UtilitiesRequestFetchSync utilities_request = new UtilitiesRequestFetchSync();
            try
            {
                string variables = $@"{{
                       profile_id:'{pageIdSwitch}'
                   }}";
                string data_send = utilities_request.RenderDataSwitch(chromedriver, pageIdNow, variables, "CometProfileSwitchMutation", "7240611932633722");
                string create_comment = utilities_request.RequestPost(chromedriver, "https://www.facebook.com/api/graphql/", data_send, chromedriver.GetURL());
                if (string.IsNullOrEmpty(create_comment)) return false;
                chromedriver.GotoURL(string.Format("https://www.facebook.com/profile.php?id={0}", pageIdSwitch));
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return true;

        }
        public bool IsLoginedAccount(ChromeBrowser chromedriver)
        {
            try
            {
                string url = chromedriver.GetURL();
                if (url.Contains("checkpoint"))
                {
                    return false;
                }

                List<string> lstUrlChecking = new List<string> { "facebook.com/profile.php", "facebook.com/home.php", "facebook.com" };
                if (HelperSync.CheckStringContainKeyword(chromedriver.GetURL(), lstUrlChecking))
                {
                    return true;
                }

                List<string> lstKeyHtml = new List<string> { "/friends/", "\"is_checkpointed\":false" };
                if (HelperSync.CheckStringContainKeyword(chromedriver.GetPageSource().Data, lstKeyHtml))
                {
                    return true;
                }

                List<string> lstNodeChecking = new List<string> { "a[href*=\"/friends/\"]", "[action=\"/logout.php?button_location=_settings&button_name=logout\"]" };
                string nodeChecking = chromedriver.GetElementExistFromList(4, 0.5, lstNodeChecking);
                if (!string.IsNullOrEmpty(nodeChecking))
                {
                    return true;
                }
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return false;
        }
        public bool ChangeLanguageFacebook(ChromeBrowser chromedriver, string accUID)
        {
            UtilitiesRequestFetchSync utilities_request = new UtilitiesRequestFetchSync();
            bool ChangeLanguage = true;
            try
            {
                //get language
                string variables = $@"{{
                   query:'',
                   suggestedLocaleLimit:4,
                   showOnlyFallbacks:false
                }}";
                string data_send = utilities_request.RenderDataSend(chromedriver, accUID, variables, "IntlLocaleSelectorTypeaheadSourceQuery", "6390723504351289");
                string create_comment = utilities_request.RequestPost(chromedriver, "https://www.facebook.com/api/graphql/", data_send, chromedriver.GetURL());
                if (string.IsNullOrEmpty(create_comment)) return false;
                string SelectLanguageAcc = HelperStringSync.getFromIndex(create_comment, create_comment.IndexOf("locale\":\"") + 9, "\"");
                if (!string.IsNullOrWhiteSpace(SelectLanguageAcc))
                {
                    switch (SelectLanguageAcc)
                    {
                        case "en_US" or "vi_VN":
                            return true;
                        default:
                            variables = $@"{{
                                     locale:'en_US',
                                     referrer:'WWW_COMET_NAVBAR',
                                     fallback_locale:null
                     }}";
                            data_send = utilities_request.RenderDataSwitch(chromedriver, accUID, variables, "useCometLocaleSelectorLanguageChangeMutation", "6451777188273168");
                            create_comment = utilities_request.RequestPost(chromedriver, "https://www.facebook.com/api/graphql/", data_send, chromedriver.GetURL());
                            if (string.IsNullOrEmpty(create_comment)) return false;
                            chromedriver.Refresh(2);
                            break;
                    }
                    return true;
                }
                //chromedriver.GotoURL(string.Format("https://www.facebook.com/profile.php?id={0}", pageIdSwitch));

            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return ChangeLanguage;

        }
        public string GetFbDtsg(ChromeBrowser chromedriver, IAccountFacebook acc)
        {
            string result = null;
            try
            {
                string FbDtsg = "";
                string html = null;
                int type = HelperSync.CheckTypeWebFacebookFromUrl(chromedriver.GetURL());
                switch (type)
                {
                    case 1:
                        FbDtsg = chromedriver.ExecuteScript("return require('DTSGInitData').token").ToString();
                        if (FbDtsg.Length > 20)
                        {
                            acc.FbDtsg = FbDtsg;
                        }
                        break;

                    case 2:
                        html = RequestGet(chromedriver, "https://m.facebook.com/help", chromedriver.GetURL());
                        if (!string.IsNullOrEmpty(html))
                        {
                            FbDtsg = Regex.Match(html, "FbDtsg\" value=\"(.*?)\"").Groups[1].Value;
                            if (!string.IsNullOrEmpty(FbDtsg) && FbDtsg.Length > 20)
                            {
                                acc.FbDtsg = FbDtsg;
                            }
                            //else
                            //{
                            //    html = RequestGet(chromedriver, "https://mobile.facebook.com/help/", chromedriver.GetURL());
                            //    result = Regex.Match(html, HelperSync.Base64Decode(HelperProtectExtension.Decode(AppInfo.SettingDetect.GetValue("base_64_get_list_group"), AppInfo.Protect))).Groups[1].Value;
                            //    if (!string.IsNullOrEmpty(FbDtsg) && FbDtsg.Length > 20)
                            //    {
                            //        acc.FbDtsg = FbDtsg;
                            //    }
                            //}
                        }
                        break;

                    case 3:
                        chromedriver.GotoURL("https://mbasic.facebook.com");
                        html = chromedriver.GetPageSource().Data;
                        if (html.Contains("fb_dtsg"))
                        {
                            result = HelperStringSync.getFromIndex(html, html.IndexOf("fb_dtsg\" value=") + 16, "\"");
                            if (FbDtsg.Length > 20)
                            {
                                acc.FbDtsg = FbDtsg;
                            }
                        }
                        break;
                }
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return result;
        }
        public void CheckDetectAutomation(ChromeBrowser chromedriver)
        {
            try
            {

                if (chromedriver.GetURL().Contains("checkpoint/601051028565049"))
                {
                    List<string> lst_node = new List<string> { "[role=\"main\"] [role=\"button\"]", "form[action*=\"/checkpoint/601051028565049\"] [type=\"submit\"]" };
                    string node = chromedriver.GetElementExistFromList(4, 0.5, lst_node);
                    if (!string.IsNullOrEmpty(node))
                    {
                        chromedriver.Click(SelectorType.ByCssSelector, node);
                    }
                }
            }
            catch
            {

            }
        }
        public static string RequestGet(ChromeBrowser chrome, string url, string website)
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
                return chrome.ExecuteScript("async function RequestGet() { var output = ''; try { var response = await fetch('" + url + "'); if (response.ok) { var body = await response.text(); return body; } } catch {} return output; }; var c = await RequestGet(); return c;").ToString();
            }
            catch
            {
            }
            return "";
        }
    }
}
