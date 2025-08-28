using System.Text;
using System.Text.RegularExpressions;

namespace ZestPost.Base.Helper
{
    public static class HelperStringSync
    {
        private static readonly Random rd = new Random();
        public static List<string> SupportedIcons = new List<string> { "❤️", "☠️", "✊", "✌️", "☝️", "✋", "☝️", "✍️", "‍♀️", "‍♂️", "‍⚕️", "✈️", "‍⚖️", "⛑", "☘️", "☀️", "☄️", "⚡️", "⛅️", "⛈", "☔️", "☂️", "⚽️", "⚾️", "⛳️", "⛹️", "⛰", "⛪️", "⚔️", "✉️", "✝️", "☦️", "☸️", "♌️", "♍️", "♎️", "⛔️", "❌", "⚧", "⚧", "1️⃣", "3️⃣", "4️⃣", "▶️", "⏸", "⏺", "⏭", "☑️", "⚪️", "◼️", "♥️", "✨", "✨", "⚡️", "❄️", "☕", "⚽️", "⛹️‍♂️", "⛽️", "⛲️", "⌚️", "☎️", "⏲", "⚙️", "⛓", "✏️", "⛎", "♈️", "♋️", "♎️", "⚛️", "♓️", "♑️", "㊗️", "❌", "♨️", "⁉️", "‼️", "❕", "⚜️", "❎", "✳️", "Ⓜ️", "♈️", "♋️", "♎️", "⚛️", "♓️", "♑️", "㊗️", "❌", "♨️", "⁉️", "‼️", "❕", "⚜️", "❎", "✳️", "Ⓜ️", "⚧", "ℹ️", "0️⃣", "2️⃣", "4️⃣", "#️⃣", "9️⃣", "6️⃣", "⏏️", "⏯", "⏺", "➡️", "⏬", "⬅️", "⬆️", "⬇️", "↖️", "↩️", "↔️", "➗", "♾️", "✔️", "☑️", "⚪️", "▪️", "♠️", "⚽", "⚾", "⛳", "⛸", "♟", "⛑", "☂️" };
        public static void CreateDirectoryIfNotExists(string filePath)
        {
            try
            {
                // Lấy đường dẫn thư mục từ đường dẫn tệp
                string directory = Path.GetDirectoryName(filePath);

                // Kiểm tra xem thư mục có tồn tại không
                if (!Directory.Exists(directory))
                {
                    // Tạo thư mục nếu không tồn tại
                    Directory.CreateDirectory(directory);
                    Console.WriteLine("Directory created at: " + directory);
                }
                else
                {
                    Console.WriteLine("Directory already exists at: " + directory);
                }
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
        }
        public static string detechString(string string_1)
        {
            try
            {
                if (string_1.Contains("$number"))
                {
                    string_1 = string_1.Replace("$number", rd.Next(9999).ToString());
                }
                if (string_1.Contains("$timespan"))
                {
                    string_1 = string_1.Replace("$timespan", getSecondDataTimeNow().ToString());
                }
                if (string_1.Contains("$random"))
                {
                    string_1 = string_1.Replace("$random", rd.Next(999999999).ToString());
                }
                if (string_1.Contains("$time"))
                {
                    string_1 = string_1.Replace("$time", rd.Next(0, 12).ToString() + ":" + rd.Next(0, 60).ToString());
                }
                if (string_1.Contains("$date"))
                {
                    string_1 = string_1.Replace("$date", DateTime.Now.ToString());
                }
                if (string_1.Contains("$text"))
                {
                    string_1 = string_1.Replace("$text", spinText());
                }

                while (string_1.Contains("$smile"))
                {
                    string_1 = smile(string_1, "$smile", getIconSmile());
                }
                int num = string_1.IndexOf('{');
                int num2 = string_1.IndexOf('}');
                string result;
                if (num == -1 && num2 == -1)
                {
                    result = string_1;
                }
                else if (num == -1 || num2 < num)
                {
                    result = string_1;
                }
                else
                {
                    if (num2 == -1)
                    {
                        throw new ArgumentException("Unbalanced brace.");
                    }
                    string text = detechString(string_1.Substring(num + 1, string_1.Length - (num + 1)));
                    num2 = text.IndexOf('}');
                    if (num2 == -1)
                    {
                        throw new ArgumentException("Unbalanced brace.");
                    }
                    string[] array = text.Substring(0, num2).Split(new char[]
                    {
                '|'
                    });
                    string str = array[rd.Next(0, array.Length)];
                    result = string_1.Substring(0, num) + str + detechString(text.Substring(num2 + 1, text.Length - (num2 + 1)));
                    return result;
                }
            }
            catch { return string_1; }
            return string_1;
        }
        private static string spinText()
        {
            string text = randomText(rd.Next(3, 5));
            string text2 = randomText(rd.Next(3, 5));
            string text3 = randomText(rd.Next(3, 5));
            string text4 = randomText(rd.Next(3, 5));
            string text5 = randomText(rd.Next(3, 5));
            string text6 = randomText(rd.Next(3, 5));
            string text7 = randomText(rd.Next(3, 5));
            string text8 = randomText(rd.Next(3, 5));
            string text9 = randomText(rd.Next(3, 5));
            string text10 = randomText(rd.Next(3, 5));
            return string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9}", new object[]
            {
            text,
            text2,
            text3,
            text4,
            text5,
            text6,
            text7,
            text8,
            text9,
            text10
            });
        }
        private static string randomText(int int_0)
        {
            char[] array = new char[int_0];
            for (int i = 0; i < int_0; i++)
            {
                array[i] = "abcdefghiklmnopqrstxyzv"[rd.Next(23)];
            }
            return new string(array);
        }
        public static string smile(string string_1, string string_2, string string_3)
        {
            Regex regex = new Regex(Regex.Escape(string_2));
            return regex.Replace(string_1, string_3, 1);
        }
        public static int getSecondDataTimeNow()
        {
            return (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }
        private static string getIconSmile()
        {
            string[] array = "\ud83d\ude37|\ud83d\ude37|\ud83d\ude13|\ud83d\ude30|\ud83d\ude25|\ud83d\ude2a|\ud83d\ude28|\ud83d\ude31|\ud83d\ude35|\ud83d\ude2d|\ud83d\ude20|\ud83d\ude33|\ud83d\ude32|\ud83d\ude24|\ud83d\udebd|\ud83d\udec0|\ud83d\udc59|\ud83d\udc84|\ud83d\udc55|\ud83d\udc58|\ud83d\udc57|\ud83d\udc62|\ud83d\udc60|\ud83d\udc61|\ud83d\udcbc|\ud83d\udc5c|\ud83d\udc54|\ud83c\udfa9|\ud83d\udc52|\ud83d\udc51|\ud83d\udc8d|\ud83d\udead|\ud83c\udfc8|\ud83c\udfc0|\ud83c\udfbe|\ud83c\udfb1|\ud83c\udfaf|\ud83c\udfbf|\ud83c\udf8c|\ud83c\udfc1|\ud83c\udfc6|\ud83d\udc4c|\ud83d\udc4e|\ud83d\ude4c|\ud83d\udcaa|\ud83d\udc4a|\ud83d\udc4f|\ud83d\udc46|\ud83d\udc49|\ud83d\udc48|\ud83d\udc47|\ud83d\udc94|\ud83d\udc99|\ud83d\udc9a|\ud83d\udc9b|\ud83d\udc9c|\ud83d\udc97|\ud83d\udc98|\ud83d\udc93|\ud83d\udc9d|\ud83d\udc96|\ud83d\udc9e|\ud83d\udc9f|\ud83d\udc8c|\ud83d\udc91|\ud83d\udc8b|\ud83d\udc44|\ud83d\ude0d|\ud83d\ude18|\ud83d\ude1a|\ud83d\ude0a|\ud83d\ude0f|\ud83d\ude0c|\ud83d\ude03|\ud83d\ude04|\ud83d\ude1e|\ud83d\ude22|\ud83d\ude1c|\ud83d\ude1d|\ud83d\ude09|\ud83d\ude14|\ud83d\ude12|\ud83d\ude02|\ud83d\ude21|\ud83d\udc7f|\ud83d\udc7d|\ud83d\udc7e|\ud83d\udc7b|\ud83d\udc7c|\ud83d\udc6f|\ud83d\udc82|\ud83d\udc73|\ud83c\udf85|\ud83d\udc6e|\ud83d\udc77|\ud83d\udc78|\ud83d\udc74|\ud83d\udc75|\ud83d\udc68|\ud83d\udc69|\ud83d\udc66|\ud83d\udc67|\ud83d\udc76|\ud83d\udc71|\ud83d\udc6b|\ud83c\udf8e|\ud83d\udc83|\ud83d\udc42|\ud83d\udc43|\ud83d\udc40|\ud83c\udf1f|\ud83c\udf19|\ud83c\udfb5|\ud83c\udfb6|\ud83d\udca4|\ud83d\udd25|\ud83c\ude50|\ud83c\udf80|\ud83c\udf02|\ud83d\udca7|\ud83d\udd28|\ud83d\udcba|\ud83d\udd31|\ud83d\udd30|\ud83c\udc04|\ud83d\udc8e|\ud83d\udca0|\ud83d\udd37|\ud83d\udd36".Split(new char[]
            {
            '|'
            });
            return array[rd.Next(0, array.Length)];
        }
        public static string getFromIndex(string string_0, int int_0, string string_1)
        {
            try
            {
                string str = "";
                for (int i = int_0; i < string_0.Length; i++)
                {
                    char ch = string_0[i];
                    if (!(ch.ToString() != string_1))
                    {
                        break;
                    }
                    str = str + string_0[i];
                }
                return str;
            }
            catch (Exception ex)
            {
                Log4NetSyncController.LogException(ex, "");
                return string.Empty;
            }
        }
        public static string ReplaceUnsupportedIcons(string input)
        {
            StringBuilder result = new StringBuilder();
            try
            {
                string pattern = @"[\uD800-\uDBFF][\uDC00-\uDFFF]";
                MatchCollection matches = Regex.Matches(input, pattern);
                foreach (Match match in matches)
                {
                    string icon = match.Value;
                    if (!SupportedIcons.Contains(icon))
                    {
                        input = input.Replace(icon, HelperCore.GetRandomItemFromListNotRemove(SupportedIcons, new Random()));
                    }
                }
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return input.ToString();
        }
        public static string RemoveFromIndex(string string_0, int int_0, int int_1)
        {
            try
            {
                string str = "";
                str = string_0.Remove(int_0, int_1 - int_0);
                return str;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        public static string getSubstringBetween(string input, string start, string end)
        {
            try
            {
                int startIndex = input.IndexOf(start);
                if (startIndex >= 0)
                {
                    int endIndex = input.IndexOf(end, startIndex + 1);
                    if (endIndex >= 0)
                    {
                        string text = input.Substring(startIndex, endIndex - startIndex);
                        return input.Substring(startIndex, endIndex - startIndex);
                    }
                }

                return input;
            }
            catch (Exception ex) { Log4NetSyncController.LogException(ex, ""); }
            return string.Empty;
        }
        public static string CreateRandomStringNumber(int lengText, Random rd = null)
        {
            string text = "";
            if (rd == null)
            {
                rd = new Random();
            }
            string text2 = "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            for (int i = 0; i < lengText; i++)
            {
                text += text2[rd.Next(0, text2.Length)];
            }
            return text;
        }
        public static string RemoveSpecialCharacters(string input)
        {
            // Chỉ giữ lại các ký tự chữ và số
            return Regex.Replace(input, @"[^a-zA-Z0-9\s\u00C0-\u00FF\u0102-\u0103\u0110\u0111\u0128-\u0129\u0168-\u0169\u01A0-\u01A1\u01AF-\u01B0\u1EA0-\u1EF9]", "");
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
    }
}
