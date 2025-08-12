using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ZestPost.Base.Helper
{
    public class HelperCore
    {
        public static string L(string key)
        {
            return key;
        }
        public static bool DeleteFolder(string pathFolder)
        {
            //Discarded unreachable code: IL_001e, IL_002b
            try
            {
                Directory.Delete(pathFolder, recursive: true);
                return true;
            }
            catch
            {
            }
            return false;
        }

        public static string CreateRandomString(int lengText, Random rd = null)
        {
            string text = "";
            if (rd == null)
            {
                rd = new Random();
            }
            string text2 = "abcdefghijklmnopqrstuvwxyz";
            for (int i = 0; i < lengText; i++)
            {
                text += text2[rd.Next(0, text2.Length)];
            }
            return text;
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

        public static string RunCMD(string cmd)
        {
            string text = "";
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = "/c " + cmd;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                text = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                if (string.IsNullOrEmpty(text))
                {
                    return "";
                }
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return text;
        }

        public static string GetRandomItemFromListNotRemove(List<string> lst, Random rd)
        {
            string text = "";
            try
            {
                text = lst[rd.Next(0, lst.Count)];
            }
            catch (Exception)
            {
                throw;
            }
            return text;
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

        public static int GetIndexOfPossitionApp(ref List<int> lstPossition)
        {
            int result = 0;
            try
            {
                lock (lstPossition)
                {
                    for (int i = 0; i < lstPossition.Count; i++)
                    {
                        if (lstPossition[i] == 0)
                        {
                            result = i;
                            lstPossition[i] = 1;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return result;
        }

        public static void FillIndexPossition(ref List<int> lstPossition, int indexPos)
        {
            lock (lstPossition)
            {
                lstPossition[indexPos] = 0;
            }
        }

    }
}
