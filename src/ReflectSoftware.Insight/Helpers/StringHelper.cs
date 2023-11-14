
using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace RI.Utils.Strings
{
    public static class StringHelper
    {
        private static readonly string[,] URLSpecialCharacters = new string[12, 2]
        {
            { "%25", "%" },
            { "%2b", "+" },
            { "%24", "$" },
            { "%26", "&" },
            { "%2c", "," },
            { "%2f", "/" },
            { "%3a", ":" },
            { "%3b", ";" },
            { "%3d", "=" },
            { "%3f", "?" },
            { "%40", "@" },
            { "+", " " }
        };

        public static bool IsNullOrEmpty(string str)
        {
            if (str != null)
            {
                return string.Compare(str.Trim(), string.Empty, ignoreCase: false, CultureInfo.InvariantCulture) == 0;
            }

            return true;
        }

        public static string IfNullUseDefault(string str, string defaultValue)
        {
            return str ?? defaultValue;
        }

        public static string IfNullOrEmptyUseDefault(string str, string defaultValue)
        {
            if (!IsNullOrEmpty(str))
            {
                return str;
            }

            return defaultValue;
        }

        public static string FullTrim(string str)
        {
            return str?.Replace(" ", string.Empty);
        }

        public static bool IsUrlFormatValid(string url)
        {
            url = url.Trim().ToLowerInvariant();
            if (url.IndexOf("http://", StringComparison.InvariantCulture) > 0 || url.IndexOf("https://", StringComparison.InvariantCulture) > 0 || url.IndexOf("ftp://", StringComparison.InvariantCulture) > 0)
            {
                return false;
            }

            if (url.IndexOf("http://", StringComparison.InvariantCulture) == -1 && url.IndexOf("https://", StringComparison.InvariantCulture) == -1 && url.IndexOf("ftp://", StringComparison.InvariantCulture) == -1)
            {
                url = string.Format(CultureInfo.InvariantCulture, "http://{0}", new object[1] { url });
            }

            return new Regex("(ftp|http|https):\\/\\/(\\w+:{0,1}\\w*@)?(\\S+)(:[0-9]+)?(\\/|\\/([\\w#!:.?+=&%@!\\-\\/]))?").IsMatch(url);
        }

        public static bool IsValidIPAddress(string address)
        {
            return new Regex("\\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\b").IsMatch(address);
        }

        public static bool IsStrongPassword(string password, bool bSpecialChars, int minSize, int maxSize)
        {
            if (password.IndexOf(" ", StringComparison.InvariantCulture) > 0)
            {
                return false;
            }

            string format = "^ (?=.*\\d)(?=.*[a-z])(?=.*[A-Z]){0}.{{{1},{2}}}$";
            string text = ((!bSpecialChars) ? string.Empty : "(?=.*[\\-\\+\\?\\*\\$\\[\\]\\^\\.\\(\\)\\|`~!@#%&_ ={}:;  ',/])");
            string pattern = string.Format(CultureInfo.InvariantCulture, format, new object[3] { text, minSize, maxSize });
            Regex regex = new Regex(pattern, RegexOptions.IgnorePatternWhitespace);
            Match match = regex.Match(password);
            return match.Success;
        }

        public static string UrlDecoder(string sValue)
        {
            return HttpUtility.UrlDecode(sValue);
        }

        public static string UrlDecoder(string sValue, Encoding ec)
        {
            return HttpUtility.UrlDecode(sValue, ec);
        }

        public static string UrlEncoder(string sValue)
        {
            return HttpUtility.UrlEncode(sValue);
        }

        public static string UrlEncoder(string sValue, Encoding ec)
        {
            return HttpUtility.UrlEncode(sValue, ec);
        }

        public static string UrlEncoderEx(string url)
        {
            StringBuilder stringBuilder = new StringBuilder(url);
            for (int i = 0; i < URLSpecialCharacters.Length / 2; i++)
            {
                stringBuilder.Replace(URLSpecialCharacters[i, 1], URLSpecialCharacters[i, 0]);
            }

            return stringBuilder.ToString();
        }

        public static string UrlDecoderEx(string url)
        {
            StringBuilder stringBuilder = new StringBuilder(url);
            for (int num = URLSpecialCharacters.Length / 2 - 1; num >= 0; num--)
            {
                stringBuilder.Replace(URLSpecialCharacters[num, 0], URLSpecialCharacters[num, 0].ToLower(CultureInfo.InvariantCulture));
                stringBuilder.Replace(URLSpecialCharacters[num, 0], URLSpecialCharacters[num, 1]);
            }

            return stringBuilder.ToString();
        }
    }


    public static class StringHash
    {
        public static long RSHash(string str)
        {
            int num = 63689;
            long num2 = 0L;
            for (int i = 0; i < str.Length; i++)
            {
                num2 = num2 * num + str[i];
                num *= 378551;
            }

            return num2;
        }

        public static long JSHash(string str)
        {
            long num = 1315423911L;
            for (int i = 0; i < str.Length; i++)
            {
                num ^= (num << 5) + str[i] + (num >> 2);
            }

            return num;
        }

        public static long ELFHash(string str)
        {
            long num = 0L;
            long num2 = 0L;
            for (int i = 0; i < str.Length; i++)
            {
                num = (num << 4) + str[i];
                if ((num2 = num & 0xF0000000u) != 0)
                {
                    num ^= num2 >> 24;
                }

                num &= ~num2;
            }

            return num;
        }

        public static long BKDRHash(string str)
        {
            long num = 0L;
            for (int i = 0; i < str.Length; i++)
            {
                num = num * 131 + str[i];
            }

            return num;
        }

        public static long SDBMHash(string str)
        {
            long num = 0L;
            for (int i = 0; i < str.Length; i++)
            {
                num = str[i] + (num << 6) + (num << 16) - num;
            }

            return num;
        }

        public static long DJBHash(string str)
        {
            long num = 5381L;
            for (int i = 0; i < str.Length; i++)
            {
                num = (num << 5) + num + str[i];
            }

            return num;
        }

        public static long DEKHash(string str)
        {
            long num = str.Length;
            for (int i = 0; i < str.Length; i++)
            {
                num = (num << 5) ^ (num >> 27) ^ str[i];
            }

            return num;
        }

        public static long BPHash(string str)
        {
            long num = 0L;
            for (int i = 0; i < str.Length; i++)
            {
                num = (num << 7) ^ str[i];
            }

            return num;
        }

        public static long FNVHash(string str)
        {
            long num = 2166136261L;
            long num2 = 0L;
            for (int i = 0; i < str.Length; i++)
            {
                num2 *= num;
                num2 ^= str[i];
            }

            return num2;
        }

        public static long APHash(string str)
        {
            long num = 2863311530L;
            for (int i = 0; i < str.Length; i++)
            {
                num = ((((uint)i & (true ? 1u : 0u)) != 0) ? (num ^ ~(((num << 11) + str[i]) ^ (num >> 5))) : (num ^ ((num << 7) ^ (str[i] * (num >> 3)))));
            }

            return num;
        }
    }
}