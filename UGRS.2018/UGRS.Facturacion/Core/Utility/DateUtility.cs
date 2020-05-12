using System;
using System.Globalization;

namespace Core.Utility
{
    public static class DateUtility
    {
        public static DateTime GetTime(int pIntTime)
        {
            return DateTime.ParseExact(GetTimeFormat(pIntTime), "HHmm", CultureInfo.InvariantCulture);
        }

        public static string GetTimeFormat(int pIntTime)
        {
            return string.Concat((pIntTime / 100).ToString("00"), (pIntTime % 100).ToString("00"));
        }
    }
}
