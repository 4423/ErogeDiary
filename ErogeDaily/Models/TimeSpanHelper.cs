using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErogeDaily.Models
{
    public static class TimeSpanHelper
    {
        public static string ToPlayTimeString(this TimeSpan timeSpan)
        {
            var res = "";
            if (((int)timeSpan.TotalHours) > 0)
            {
                res += $"{(int)timeSpan.TotalHours}時間";
            }
            if (timeSpan.Minutes > 0)
            {
                res += $"{timeSpan.Minutes}分";
            }
            if (timeSpan.Seconds > 0)
            {
                res += $"{timeSpan.Seconds}秒";
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

        public static TimeSpan ParseWithoutDays(this string s)
        {
            var d = s.Split(":");
            if (d.Length != 3)
            {
                throw new FormatException(s);
            }

            var hours = int.Parse(d[0]);
            var minutes = int.Parse(d[1]);
            var seconds = int.Parse(d[2]);
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
