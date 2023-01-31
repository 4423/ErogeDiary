using System;
using System.Collections.Generic;

namespace ErogeDiary.Helpers;

public static class ListExtensions
{
    public static T Random<T>(this List<T> values)
    {
        var randomIndex = new Random().Next(values.Count - 1);
        return values[randomIndex];
    }
}
