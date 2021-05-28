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
    }
}
