using System.Text.RegularExpressions;

namespace Core.Extension
{
    public static class StringExtension
    {
        public static bool IsNumber(this string pStrText)
        {
            Regex regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
            return !regex.IsMatch(pStrText);
        }
    }
}
