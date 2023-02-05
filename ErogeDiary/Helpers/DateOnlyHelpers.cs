using System;

namespace ErogeDiary.Helpers;

public static class DateOnlyHelpers
{
    public static DateOnly Min(DateOnly first, DateOnly second)
        => first < second ? first : second;
}
