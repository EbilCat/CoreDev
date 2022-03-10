using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace CoreDev.Extensions
{
    public static class StringExtensions
    {
        private static TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

        public static string ToTitleCase(this string str)
        {
            string titleCaseString = textInfo.ToTitleCase(str.ToLower());
            return titleCaseString;
        }

        public static string RemoveWhiteSpace(this string str)
        {
            return new string(str.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());
        }

        public static string AddWhiteSpace(this string str)
        {
            return Regex.Replace(str, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 "); ;
        }

        public static string ToMD5Hash(this string str)
        {
            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(str.ToStream());
            string hashString = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            return hashString;
        }

        public static Stream ToStream(this string str)
        {
            return str.ToStream(Encoding.UTF8);
        }

        public static Stream ToStream(this string str, Encoding encoding)
        {
            return new MemoryStream(encoding.GetBytes(str ?? ""));
        }
    }
}