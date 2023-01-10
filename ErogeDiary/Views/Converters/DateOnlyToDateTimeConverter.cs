using System;
using System.Globalization;
using System.Windows.Data;

namespace ErogeDiary.Views.Converters;

public class DateOnlyToDateTimeConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var input = value as DateOnly?;
        return input?.ToDateTime(TimeOnly.MinValue);
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var input = value as DateTime?;
        return input == null ? null : DateOnly.FromDateTime((DateTime)input);
    }
}
