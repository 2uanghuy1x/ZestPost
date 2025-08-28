using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace ZestPost.Base.Helper
{
    public class HelperSync
    {

        private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        public static string UrlEncode(string text)
        {
            return WebUtility.UrlEncode(text);
        }
        public static int CheckTypeWebFacebookFromUrl(string url)
        {
            int result = 0;
            bool flag = url.StartsWith("https://www.facebook") || url.StartsWith("https://facebook") || url.StartsWith("https://web.facebook");
            if (flag)
            {
                result = 1;
            }
            else
            {
                bool flag2 = url.StartsWith("https://m.facebook") || url.StartsWith("https://d.facebook") || url.StartsWith("https://mobile.facebook");
                if (flag2)
                {
                    result = 2;
                }
                else
                {
                    bool flag3 = url.StartsWith("https://mbasic.facebook");
                    if (flag3)
                    {
                        result = 3;
                    }
                }
            }
            return result;
        }
        public static string SpinText(string text, Random rand)
        {
            int num = -1;
            char[] anyOf = new char[2] { '{', '}' };
            text += "~";
            do
            {
                int num2 = num;
                num = -1;
                while ((num2 = text.IndexOf('{', num2 + 1)) != -1)
                {
                    int num3 = num2;
                    while ((num3 = text.IndexOfAny(anyOf, num3 + 1)) != -1 && text[num3] != '}')
                    {
                        if (num == -1)
                        {
                            num = num2;
                        }
                        num2 = num3;
                    }
                    if (num3 != -1)
                    {
                        string[] array = text.Substring(num2 + 1, num3 - 1 - (num2 + 1 - 1)).Split('|');
                        text = text.Remove(num2, num3 - (num2 - 1)).Insert(num2, array[rand.Next(array.Length)]);
                    }
                }
            }
            while (num-- != -1);
            return text.Remove(text.Length - 1);
        }
        public static bool IsNumber(string pValue)
        {
            if (pValue == "")
            {
                return false;
            }
            foreach (char c in pValue)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }
        public static bool IsContainNumber(string pValue)
        {
            foreach (char c in pValue)
            {
                if (char.IsDigit(c))
                {
                    return true;
                }
            }
            return false;
        }
        public static string HtmlEncode(string text)
        {
            return WebUtility.HtmlEncode(text);
        }
        public static void ReadHtmlText(string html)
        {
            string text = "zzz999.html";
            File.WriteAllText(text, html);
            Process.Start(text);
        }
        public static string GetRandomItemFromListNotRemove(List<string> lst, Random rd)
        {
            string text = "";
            try
            {
                text = lst[rd.Next(0, lst.Count)];
            }
            catch (Exception ex)
            {
                Log4NetSyncController.LogException(ex, "");
                throw;
            }
            return text;
        }
        public static DateTime ConvertTimeStampToDateTime(double timestamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestamp).ToLocalTime();
        }
        public static bool CheckStringContainKeyword(string content, List<string> lstKerword)
        {
            for (int i = 0; i < lstKerword.Count; i++)
            {
                bool flag = Regex.IsMatch(content, lstKerword[i]) || content.Contains(lstKerword[i]);
                if (flag)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool CheckListContainsString(string string0, List<string> lst_string)
        {
            try
            {
                for (int i = 0; i < lst_string.Count; i++)
                {
                    if (lst_string[i] == string0)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return false;
        }
        public static List<T> CloneList<T>(List<T> lstFrom)
        {
            List<T> list = new List<T>();
            try
            {
                for (int i = 0; i < lstFrom.Count; i++)
                {
                    list.Add(lstFrom[i]);
                }
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return list;
        }
        public static string GetRandomItemFromList(ref List<string> lst, Random rd)
        {
            string text = "";
            try
            {
                text = lst[rd.Next(0, lst.Count)];
                lst.Remove(text);
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return text;
        }
        public static int getSecondsĐatTimeNow()
        {
            return (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }
        public static int UnixTimeNow()
        {
            int unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            return unixTimestamp;
        }
        public static int ConvertTimeSpan()
        {
            return (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }
        public static int ConvertTimeSpan(string time)
        {
            DateTime datetime = DateTime.Parse(time);
            return (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }
        public static double ConvertDatetimeToTimestamp(DateTime value)
        {
            return (value - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime()).TotalSeconds;
        }
        public static int TimeDifference(DateTime d1, DateTime d2)
        {
            int time = 0;
            try
            {
                TimeSpan time_between = GetTimeDuration(d1, d2);
                time = Convert.ToInt32(time_between.TotalSeconds);
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return time;
        }
        private static TimeSpan GetTimeDuration(DateTime startTime, DateTime endTime)
        {
            // Trả về thời gian giữa startTime và endTime
            return endTime - startTime;
        }
        public static string FeedbackID(string postid)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes("feedback:" + postid);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64Encoded)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(base64Encoded);
                return Encoding.UTF8.GetString(bytes);
            }
            catch { return null; }
        }
    }
}
