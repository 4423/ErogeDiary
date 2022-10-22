using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace ErogeDiary.Views.Converters;

public class BooleanConverter<T> : IValueConverter
{
    public BooleanConverter(T trueValue, T falseValue, BooleanConverterDirection direction)
    {
        this.True = trueValue;
        this.False = falseValue;
        this.Direction = direction;
    }

    public T True { get; set; }
    public T False { get; set; }
    public BooleanConverterDirection Direction { get; set; }

    public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        switch (Direction)
        {
            case BooleanConverterDirection.ToBoolean:
                return ConvertToBoolean(value);
            case BooleanConverterDirection.FromBoolean:
                return ConvertFromBoolean(value);
        }
        throw new NotImplementedException();
    }

    public object? ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        switch (Direction)
        {
            case BooleanConverterDirection.ToBoolean:
                return ConvertFromBoolean(value);
            case BooleanConverterDirection.FromBoolean:
                return ConvertToBoolean(value);
        }
        throw new NotImplementedException();
    }

    private object? ConvertToBoolean(object? value)
        => value is T && EqualityComparer<T>.Default.Equals((T)value, True);

    private object? ConvertFromBoolean(object? value)
        => value is bool && ((bool)value) ? True : False;
}

public enum BooleanConverterDirection
{
    ToBoolean,
    FromBoolean,
}