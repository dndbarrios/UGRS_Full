using System.Collections.Generic;
using System.Linq;

namespace UGRS.Core.Extensions {
    public static class StringExtension {

        public static string Inject(this string self, string placeholder, string injectionString) => self.Replace(placeholder, injectionString);
        public static bool IsOneOf(this string self, params string[] values) => values.Contains(self);
        public static string RemoveAll(this string self, params string[] values) {
            foreach (var value in values) {
                self = self.Replace(value, "");
            }
            return self;
        }

        public static string Inject(this string self, Dictionary<string, string> injectStrings) {
            foreach (var @string in injectStrings) {
                self = self.Replace(@string.Key, @string.Value);
            }
            return self;
        }
    }
}