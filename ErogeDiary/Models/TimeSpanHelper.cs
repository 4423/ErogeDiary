using System;
using ErogeDiary.Properties;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ErogeDiary.Models
{
    public static class TimeSpanHelper
    {
        public static string ToPlayTimeString(this TimeSpan timeSpan)
        {
            var res = "";
            if (((int)timeSpan.TotalHours) > 0)
            {
                res += string.Format(CultureInfo.CurrentCulture, Strings.PlayTime_HoursFormat, (int)timeSpan.TotalHours);
            }
            if (timeSpan.Minutes > 0)
            {
                res += string.Format(CultureInfo.CurrentCulture, Strings.PlayTime_MinutesFormat, timeSpan.Minutes);
            }
            if (timeSpan.Seconds > 0)
            {
                res += string.Format(CultureInfo.CurrentCulture, Strings.PlayTime_SecondsFormat, timeSpan.Seconds);
            }
            return res;
        }

        public static string ToZeroPaddingStringWithoutDays(this TimeSpan timeSpan)
        {
            int hours = (int)timeSpan.TotalHours;
            int minutes = timeSpan.Minutes;
            int seconds = timeSpan.Seconds;
            return $"{hours:00}:{minutes:00}:{seconds:00}";
        }

        private static readonly Regex timeSpanRegex = new Regex(
            @"^(?<hours>\d+):(?<minutes>[0-5][0-9]):(?<seconds>[0-5][0-9])$",
            RegexOptions.Compiled
        );

        public static TimeSpan ParseWithoutDays(this string s)
        {
            var match = timeSpanRegex.Match(s);
            if (!match.Success)
            {
                throw new FormatException(s);
            }

            var hours = int.Parse(match.Groups["hours"].Value);
            var minutes = int.Parse(match.Groups["minutes"].Value);
            var seconds = int.Parse(match.Groups["seconds"].Value);
            return new TimeSpan(hours, minutes, seconds);
        }

        public static bool TryParseWithoutDays(string s, out TimeSpan? result)
        {
            try
            {
                result = s.ParseWithoutDays();
                return true;
            }
            catch (Exception)
            {
                result = null;
                return false;
            }
        }
    }
}
