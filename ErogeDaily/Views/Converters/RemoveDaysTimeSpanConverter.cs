using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ErogeDaily.Views.Converters
{
    public class RemoveDaysTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan)
            {
                var ts = (TimeSpan)value;
                int hours = ts.Hours + 24 * ts.Days;
                return $"{hours}:{ts.Minutes}:{ts.Seconds}";
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = value as string;
            if (s == null)
            {
                return null;
            }

            var d = s.Split(":");
            if (d.Length != 3)
            {
                return null;
            }

            try
            {
                var hours = int.Parse(d[0]);
                var minutes = int.Parse(d[1]);
                var seconds = int.Parse(d[2]);
                return new TimeSpan(hours, minutes, seconds);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
