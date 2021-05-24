using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ErogeDaily.Views.Converters
{
    public class PlayTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not TimeSpan)
            {
                return null;
            }

            var ts = (TimeSpan)value;

            if (parameter != null && ts.TotalSeconds == 0)
            {
                return parameter;
            }

            var res = "";
            if (((int)ts.TotalHours) > 0)
            {
                res += $"{(int)ts.TotalHours}時間";
            }
            if (ts.Minutes > 0)
            {
                res += $"{ts.Minutes}分";
            }
            if (ts.Seconds > 0)
            {
                res += $"{ts.Seconds}秒";
            }
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
