using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace ZestPost.Base.Controller
{
    public static class StringExtension
    {
        /// <summary>
        /// Mảng chứa các ký tự chữ cái và chữ số (chữ hoa và chữ thường, các chữ số từ 0 đến 9)
        /// </summary>
        public static readonly char[] AlphNums = new char[54]
        {
    '0', '2', '3', '4', '5', '6', '8', '9', 'a', 'b',
    'c', 'd', 'e', 'f', 'g', 'h', 'j', 'k', 'm', 'n',
    'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y',
    'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J',
    'K', 'L', 'M', 'N', 'P', 'R', 'S', 'T', 'U', 'V',
    'W', 'X', 'Y', 'Z'
        };

        /// <summary>
        /// Mảng chứa các ký tự chữ cái (chữ hoa và chữ thường)
        /// </summary>
        public static readonly char[] Letters = new char[46]
        {
    'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'j', 'k',
    'm', 'n', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w',
    'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G',
    'H', 'J', 'K', 'L', 'M', 'N', 'P', 'R', 'S', 'T',
    'U', 'V', 'W', 'X', 'Y', 'Z'
        };

        /// <summary>
        /// Mảng chứa các ký tự chữ cái thường
        /// </summary>
        public static readonly char[] LowerLetters = new char[23]
        {
    'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'j', 'k',
    'm', 'n', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w',
    'x', 'y', 'z'
        };

        /// <summary>
        /// Mảng chứa các ký tự chữ cái hoa
        /// </summary>
        public static readonly char[] UpperLetters = new char[23]
        {
    'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K',
    'L', 'M', 'N', 'P', 'R', 'S', 'T', 'U', 'V', 'W',
    'X', 'Y', 'Z'
        };

        /// <summary>
        /// Mảng chứa các ký tự chữ số
        /// </summary>
        public static readonly char[] Numbers = new char[8] { '0', '2', '3', '4', '5', '6', '8', '9' };

        /// <summary>
        /// Mảng chứa các ký tự đặc biệt, cần trùng với các ký tự đặc biệt trong PATTERN_XXX
        /// </summary>
        public static readonly char[] SpecialLetters = new char[11]
        {
    '!', '@', '#', '$', '%', '^', '&', '*', '(', ')',
    '_'
        };


        /// <summary>
        /// Kiểm tra chuỗi có rỗng hoặc null không.
        /// </summary>
        public static bool IsEmpty(this string data)
        {
            return string.IsNullOrEmpty(data);
        }

        public static string RemoveSpecialCharacters(this string input)
        {
            if (input.IsEmpty()) return string.Empty;

            return Regex.Replace(input, @"[^\w\-.]", "");
        }

        /// <summary>
        /// Kiểm tra chuỗi có không rỗng.
        /// </summary>
        public static bool IsNotEmpty(this string data)
        {
            return !data.IsEmpty();
        }

        /// <summary>
        /// Nếu chuỗi rỗng, trả về giá trị mặc định, nếu không trả về chuỗi gốc.
        /// </summary>
        public static string IfEmpty(this string value, string defaultValue)
        {
            if (!value.IsEmpty())
            {
                return value;
            }

            return defaultValue;
        }

        public static bool Contains(this string source, IEnumerable<string> values, bool ignoreCase = true)
        {
            if (string.IsNullOrEmpty(source) || values == null || !values.Any())
                return false;

            return values.Any(value => source.IndexOf(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) >= 0);
        }

        public static bool NotContains(this string source, IEnumerable<string> values, bool ignoreCase = true)
        {
            return !source.Contains(values, ignoreCase);
        }

        /// <summary>
        /// Trả về chuỗi mới nều chuỗi bằng giá trị truyền vào
        /// </summary>
        public static string RepaceIf(this string value, string invalidValue, string newValue)
        {
            if (value.Equals(invalidValue)) return newValue;
            return value;
        }

        /// <summary>
        /// Ví dụ " a      b    " => "a b"
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string TrimToEmpty(this string input)
        {
            if (input.IsEmpty())
            {
                return input;
            }

            return Regex.Replace(input, @"\s+", " ").Trim();
        }

        /// <summary>
        /// Extension method để lấy chuỗi từ đầu đến index chỉ định
        /// </summary>
        /// <param name="input"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string PadLeft(this string input, int index)
        {
            if (input.IsEmpty())
            {
                return string.Empty;
            }

            if (input == null || index < 0 || index > input.Length)
            {
                return input;
            }

            return input[..index];
        }

        /// <summary>
        /// Extension method để lấy chuỗi từ index chỉ định đến hết text
        /// </summary>
        /// <param name="input"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string PadRight(this string input, int index)
        {
            if (input == null || index < 0 || index > input.Length)
            {
                return string.Empty;
            }

            return input[index..];
        }

        /// <summary>
        /// Lấy tên miền của địa chỉ email.
        /// </summary>
        public static string GetEmailDomain(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return "";
            }

            int num = email.IndexOf('@');
            if (num <= 0)
            {
                return "";
            }

            return email.Substring(num + 1);
        }

        /// <summary>
        /// Định dạng lại chuỗi XML theo định dạng dễ đọc hơn.
        /// </summary>
        public static string FormatXml(this string xml)
        {
            try
            {
                return XDocument.Parse(xml).ToString();
            }
            catch (Exception)
            {
                return xml;
            }
        }

        /// <summary>
        /// Chuyển đổi chuỗi thành định dạng UTF-8.
        /// </summary>
        /// <param name="input">Chuỗi cần chuyển đổi.</param>
        /// <returns>Chuỗi đã được mã hóa theo định dạng UTF-8.</returns>
        public static string ToUtf8(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            byte[] utf8Bytes = Encoding.UTF8.GetBytes(input);
            return Encoding.UTF8.GetString(utf8Bytes);
        }

        /// <summary>
        /// Chuyển chuỗi thành kiểu PascalCase.
        /// </summary>
        public static string ToPascalCase(this string value)
        {
            value = char.ToUpper(value[0]) + ((value.Length > 1) ? value.Remove(0, 1).ToLower() : "");
            return value;
        }

        /// <summary>
        /// Chuyển chuỗi từ dạng có dấu cách thành PascalCase (Ví dụ: batch_code --> BatchCode).
        /// </summary>
        public static string SpaceToPascalCase(this string value)
        {
            string[] array = Split(value, "_");
            string text = "";
            int num = 0;
            string[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                string text2 = array2[i].ToPascalCase();
                text += text2;
                num++;
            }

            return text;
        }

        /// <summary>
        /// Tách chuỗi thành mảng các chuỗi con dựa trên dấu phân cách (token) được cung cấp.
        /// </summary>
        /// <param name="value">Chuỗi cần tách.</param>
        /// <param name="token">Dấu phân cách để tách chuỗi. Mặc định là dấu phẩy (",").</param>
        /// <param name="option">Tùy chọn để xử lý các phần tử trống trong mảng kết quả. Mặc định là <see cref="StringSplitOptions.RemoveEmptyEntries"/>.</param>
        /// <returns>Mảng các chuỗi con sau khi tách, hoặc null nếu chuỗi ban đầu là null.</returns>
        public static string[] Split(this string value, string token = ",", StringSplitOptions option = StringSplitOptions.RemoveEmptyEntries)
        {
            return value?.Split(new string[1] { token }, option);
        }

        /// <summary>
        /// Chuyển chuỗi từ dạng có dấu cách thành CamelCase (Ví dụ: batch_code --> batchCode).
        /// </summary>
        public static string SpaceToCamelCase(this string value)
        {
            string[] array = Split(value, "_");
            string text = "";
            int num = 0;
            string[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                string text2 = array2[i].ToPascalCase();
                if (num == 0)
                {
                    text2 = text2.ToLower();
                }

                text += text2;
                num++;
            }

            return text;
        }

        /// <summary>
        /// Chuyển chuỗi PascalCase thành snake_case.
        /// Ví dụ: MyVariable -> my_variable.
        /// </summary>
        public static string Pascal2Lower(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty; // Trả về chuỗi rỗng nếu chuỗi đầu vào là null hoặc rỗng
            }

            string text = "";
            int length = value.Length;

            for (int i = 0; i < length; i++)
            {
                char c = value[i];
                if (char.IsLower(c))
                {
                    text += c; // Nếu là chữ cái thường, thêm vào kết quả
                    continue;
                }

                // Nếu là chữ cái hoa, chuyển thành chữ thường và thêm vào kết quả
                string text2 = char.ToLower(c).ToString();
                text += ((i == 0) ? text2 : ("_" + text2)); // Thêm dấu gạch dưới nếu không phải là ký tự đầu tiên
            }

            return text;
        }

        /// <summary>
        /// Lấy tên lớp từ tên đầy đủ của lớp (bao gồm namespace).
        /// Ví dụ: "Namespace.ClassName" -> "ClassName".
        /// </summary>
        public static string GetNameOfClass(this string fullName)
        {
            int num = fullName.LastIndexOf(".");
            if (num < 0)
            {
                return fullName; // Nếu không có dấu chấm, trả về nguyên chuỗi
            }

            return fullName.Substring(num + 1); // Lấy phần sau dấu chấm (tên lớp)
        }

        /// <summary>
        /// Lấy namespace từ tên đầy đủ của lớp.
        /// Ví dụ: "Namespace.ClassName" -> "Namespace".
        /// </summary>
        public static string GetNamespaceOfClass(this string fullName)
        {
            int num = fullName.LastIndexOf(".");
            if (num < 1)
            {
                return string.Empty; // Nếu không có dấu chấm, trả về chuỗi rỗng
            }

            return fullName.Substring(0, num); // Lấy phần trước dấu chấm (namespace)
        }

        /// <summary>
        /// Cắt chuỗi nếu độ dài chuỗi vượt quá độ dài tối đa.
        /// Nếu chuỗi có độ dài nhỏ hơn hoặc bằng maxLength, trả về chuỗi ban đầu.
        /// </summary>
        /// <param name="str">Chuỗi cần cắt.</param>
        /// <param name="maxLength">Độ dài tối đa của chuỗi.</param>
        /// <returns>Chuỗi cắt theo độ dài tối đa.</returns>
        public static string Truncate(this string str, int maxLength)
        {
            if (str == null)
            {
                return null; // Nếu chuỗi là null, trả về null
            }

            if (str.Length <= maxLength)
            {
                return str; // Nếu chuỗi có độ dài nhỏ hơn hoặc bằng maxLength, không cần cắt
            }

            return str.Left(maxLength); // Cắt chuỗi từ đầu
        }

        /// <summary>
        /// Lấy một phần của chuỗi từ cuối chuỗi.
        /// </summary>
        /// <param name="str">Chuỗi cần lấy phần cuối.</param>
        /// <param name="len">Độ dài của phần chuỗi cần lấy từ cuối.</param>
        /// <returns>Phần chuỗi lấy từ cuối.</returns>
        public static string Right(this string str, int len)
        {
            if (str.IsEmpty())
            {
                return string.Empty; // Nếu chuỗi rỗng, trả về chuỗi rỗng
            }

            if (str.Length < len)
            {
                return str; // Nếu độ dài chuỗi nhỏ hơn độ dài yêu cầu, trả về chuỗi nguyên vẹn
            }

            return str.Substring(str.Length - len, len); // Lấy phần cuối của chuỗi
        }

        /// <summary>
        /// Lấy một phần của chuỗi từ đầu chuỗi.
        /// </summary>
        /// <param name="str">Chuỗi cần lấy phần đầu.</param>
        /// <param name="len">Độ dài của phần chuỗi cần lấy từ đầu.</param>
        /// <returns>Phần chuỗi lấy từ đầu.</returns>
        public static string Left(this string str, int len)
        {
            if (str == null)
            {
                return string.Empty; // Nếu chuỗi là null, trả về chuỗi rỗng
            }

            if (str.Length < len)
            {
                return str; // Nếu độ dài chuỗi nhỏ hơn độ dài yêu cầu, trả về chuỗi nguyên vẹn
            }

            return str.Substring(0, len); // Lấy phần đầu của chuỗi
        }

        /// <summary>
        /// Xác định xem chuỗi có chứa một khối văn bản giữa chuỗi mở và chuỗi đóng đã cho hay không.
        /// </summary>
        /// <param name="data">Chuỗi cần tìm kiếm.</param>
        /// <param name="open">Chuỗi mở của khối văn bản.</param>
        /// <param name="close">Chuỗi đóng của khối văn bản.</param>
        /// <param name="startIndex">Chỉ mục bắt đầu tìm kiếm. Mặc định là 0.</param>
        /// <returns>Trả về true nếu tìm thấy một khối văn bản, ngược lại trả về false.</returns>
        public static bool HasBlock(this string data, string open, string close, int startIndex = 0)
        {
            int num = data.IndexOf(open, startIndex);
            if (num >= 0)
            {
                return data.IndexOf(close, num + 1) >= 0;
            }

            return false;
        }

        /// <summary>
        /// Tạo chuỗi đảo ngược từ chuỗi ban đầu.
        /// </summary>
        /// <param name="str">Chuỗi cần đảo ngược.</param>
        /// <returns>Chuỗi đã được đảo ngược.</returns>
        public static string Reverse(this string str)
        {
            if (str == null || str == "" || str.Length == 1)
            {
                return str;
            }

            StringBuilder stringBuilder = new StringBuilder(str.Length);
            for (int num = str.Length - 1; num >= 0; num--)
            {
                stringBuilder.Append(str[num]);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Kiểm tra xem chuỗi có độ dài trong khoảng từ min đến max không.
        /// </summary>
        /// <param name="str">Chuỗi cần kiểm tra.</param>
        /// <param name="min">Độ dài tối thiểu.</param>
        /// <param name="max">Độ dài tối đa.</param>
        /// <param name="trim">Chỉ định xem có loại bỏ khoảng trắng đầu và cuối trước khi kiểm tra không. Mặc định là false.</param>
        /// <returns>True nếu độ dài của chuỗi trong phạm vi cho phép, ngược lại trả về false.</returns>
        public static bool HasLengthInRange(this string str, int min, int max, bool trim = false)
        {
            if (str == null)
            {
                return false;
            }

            if (trim)
            {
                str = str.Trim();
            }

            int length = str.Length;
            if (length >= min)
            {
                return length <= max;
            }

            return false;
        }

        /// <summary>
        /// Kiểm tra xem chuỗi có độ dài tối thiểu không.
        /// </summary>
        /// <param name="str">Chuỗi cần kiểm tra.</param>
        /// <param name="min">Độ dài tối thiểu.</param>
        /// <param name="trim">Chỉ định xem có loại bỏ khoảng trắng đầu và cuối trước khi kiểm tra không. Mặc định là false.</param>
        /// <returns>True nếu độ dài chuỗi lớn hơn hoặc bằng min, ngược lại trả về false.</returns>
        public static bool HasMinLength(this string str, int min, bool trim = false)
        {
            if (str == null)
            {
                return false;
            }

            if (trim)
            {
                str = str.Trim();
            }

            return str.Length >= min;
        }

        /// <summary>
        /// Kiểm tra xem chuỗi có độ dài chính xác không.
        /// </summary>
        /// <param name="str">Chuỗi cần kiểm tra.</param>
        /// <param name="len">Độ dài chính xác.</param>
        /// <param name="trim">Chỉ định xem có loại bỏ khoảng trắng đầu và cuối trước khi kiểm tra không. Mặc định là false.</param>
        /// <returns>True nếu độ dài chuỗi bằng len, ngược lại trả về false.</returns>
        public static bool HasLength(this string str, int len, bool trim = false)
        {
            if (str == null)
            {
                return false;
            }

            if (trim)
            {
                str = str.Trim();
            }

            return str.Length == len;
        }

        /// <summary>
        /// Kiểm tra xem chuỗi có khớp với biểu thức chính quy hay không.
        /// </summary>
        /// <param name="value">Chuỗi cần kiểm tra.</param>
        /// <param name="expression">Biểu thức chính quy để kiểm tra.</param>
        /// <returns>Trả về true nếu chuỗi khớp với biểu thức chính quy, ngược lại trả về false.</returns>
        public static bool IsMatch(this string value, string expression)
        {
            return Regex.IsMatch(value, expression, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Kiểm tra xem chuỗi có khớp với mẫu lọc giống như File Filter (ví dụ: abc*abc).
        /// </summary>
        /// <param name="value">Chuỗi cần kiểm tra.</param>
        /// <param name="pattern">Mẫu lọc (File Filter) cần so khớp.</param>
        /// <param name="token">Ký tự thay thế (mặc định là "*").</param>
        /// <returns>Trả về true nếu chuỗi khớp với mẫu lọc, ngược lại trả về false.</returns>
        public static bool IsMatchFilter(this string value, string pattern, string token = "*")
        {
            if (pattern.IsEmpty() || pattern.Equals(token))
            {
                return true;
            }

            value = value.ToLower();
            pattern = pattern.ToLower();
            string[] array = Split(pattern, token);
            bool flag = !pattern.StartsWith(token);
            bool flag2 = !pattern.EndsWith(token);
            int startIndex = 0;
            for (int i = 0; i < array.Length; i++)
            {
                string text = array[i];
                bool flag3 = false;
                if (i == 0 && flag)
                {
                    flag3 = value.StartsWith(text);
                    if (flag3)
                    {
                        startIndex = text.Length;
                    }
                }
                else if (i == array.Length - 1 && flag2)
                {
                    flag3 = value.EndsWith(text);
                    if (flag3)
                    {
                        startIndex = value.Length;
                    }
                }
                else
                {
                    int num = value.IndexOf(text, startIndex);
                    flag3 = num >= 0;
                    if (flag3)
                    {
                        startIndex = num + text.Length;
                    }
                }

                if (!flag3)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Mã hóa chuỗi thành dạng URL-encoded.
        /// </summary>
        /// <param name="value">Chuỗi cần mã hóa.</param>
        /// <returns>Chuỗi đã được mã hóa theo định dạng URL.</returns>
        public static string UrlEncode(this string value)
        {
            return WebUtility.UrlEncode(value);
        }

        /// <summary>
        /// Giải mã chuỗi từ định dạng URL-encoded.
        /// </summary>
        /// <param name="value">Chuỗi cần giải mã.</param>
        /// <returns>Chuỗi đã được giải mã từ định dạng URL.</returns>
        public static string UrlDecode(this string value)
        {
            return WebUtility.UrlDecode(value);
        }

        /// <summary>
        /// Mã hóa chuỗi thành dạng HTML-encoded.
        /// </summary>
        /// <param name="value">Chuỗi cần mã hóa.</param>
        /// <returns>Chuỗi đã được mã hóa theo định dạng HTML.</returns>
        public static string HtmlEncode(this string value)
        {
            return WebUtility.HtmlEncode(value);
        }

        /// <summary>
        /// Giải mã chuỗi từ định dạng HTML-encoded.
        /// </summary>
        /// <param name="value">Chuỗi cần giải mã.</param>
        /// <returns>Chuỗi đã được giải mã từ định dạng HTML.</returns>
        public static string HtmlDecode(this string value)
        {
            return WebUtility.HtmlDecode(value);
        }

        public static string EncodeBase64(this string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            byte[] bytes = Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(bytes);
        }

        // Extension method to decode a Base64 string
        public static string DecodeBase64(this string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            byte[] bytes = Convert.FromBase64String(str);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
