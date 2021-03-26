using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ErogeDaily.Views.Converters
{
    public class UriStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Uri input = value as Uri;
            return input?.ToString() ?? String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string input = value as string;
            return String.IsNullOrEmpty(input) ? null : new Uri(input, UriKind.Absolute);
        }
    }
}
