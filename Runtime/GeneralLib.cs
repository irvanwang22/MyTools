using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MyTools
{
    public static class GeneralLib
    {
        public static string ToReadableString<TKey, TValue>(this Dictionary<TKey, TValue> dict)
        {
            if (dict == null || dict.Count == 0)
                return "{}";

            StringBuilder sb = new StringBuilder();
            sb.Append("{ ");

            foreach (var kvp in dict)
            {
                sb.Append($"{kvp.Key}: {kvp.Value}, ");
            }

            // Remove the trailing comma and space
            sb.Length -= 2;
            sb.Append(" }");

            return sb.ToString();
        }

        public static string GenerateRandomString(int length, char[] chars)
        {
            string rst = "";
            for (int i = 0; i < length; i++)
            {
                rst += chars[Random.Range(0, chars.Length)];
            }
            return rst;
        }
    }
}