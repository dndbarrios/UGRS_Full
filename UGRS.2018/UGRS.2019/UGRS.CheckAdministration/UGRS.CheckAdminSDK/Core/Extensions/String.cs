using System.Collections.Generic;
using System.Linq;

namespace UGRS.Core.Extensions {
    public static class StringExtension {

        public static string Inject(this string currentString, string placeholder, string injectionString) => currentString.Replace(placeholder, injectionString);
        public static bool IsOneOf<T>(this T self, params T[] values) => values.Contains(self);
        public static string Inject(this string currentString, Dictionary<string, string> injectStrings) {
            foreach (var @string in injectStrings) {
                currentString = currentString.Replace(@string.Key, @string.Value);
            }
            return currentString;
        }
    }
}