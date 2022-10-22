using ErogeDiary.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ErogeDiary.Views.Converters
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

            return ts.ToPlayTimeString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
