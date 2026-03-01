using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ErogeDiary.Views.Converters
{
    public class UriStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var input = value as Uri;
            return input?.ToString() ?? String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var input = value as string;
            if (String.IsNullOrWhiteSpace(input))
            {
                return Binding.DoNothing;
            }

            return Uri.TryCreate(input, UriKind.Absolute, out var uri) ? uri : DependencyProperty.UnsetValue;
        }
    }
}
