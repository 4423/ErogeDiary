using System.Collections.Generic;
using System.Linq;

namespace ErogeDiary.Helpers;

public static class NullableExtensions
{
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source)
        => source.Where(x => x != null).Select(x => x!);
}
