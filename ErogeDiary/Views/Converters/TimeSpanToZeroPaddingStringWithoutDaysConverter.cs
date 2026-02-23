using ErogeDiary.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ErogeDiary.Views.Converters
{
    public class TimeSpanToZeroPaddingStringWithoutDaysConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan timeSpan)
            {
                return timeSpan.ToZeroPaddingStringWithoutDays();
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
