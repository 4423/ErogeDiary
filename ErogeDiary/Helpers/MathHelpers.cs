using System;

namespace ErogeDiary.Helpers;

public static class MathHelpers
{
    public static double CeilingWithDecimalPlaces(double value, int decimalPlaces)
    {
        var offset = (int)Math.Pow(10, decimalPlaces);
        return Math.Ceiling(value * offset) / offset;
    }
}
