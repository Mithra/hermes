using System;

namespace Hermes.WebAPI.WebAPI.Helpers
{
    public static class StringHelpers
    {
        public static string TakeSubset(this string str, int maxChars)
        {
            if (String.IsNullOrEmpty(str) || str.Length <= maxChars)
                return str;

            return str.Substring(0, maxChars);
        }
    }
}
