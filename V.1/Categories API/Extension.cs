// Extension Methods for LINQ
using System;
using System.Collections.Generic;
using System.Linq;

public static class LinqExtensions
{
    public static int Sum<T>(this IEnumerable<T> source, Func<T, int> selector)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (selector == null) throw new ArgumentNullException(nameof(selector));

        int sum = 0;
        foreach (T item in source)
        {
            sum += selector(item);
        }
        return sum;
    }

    public static T FirstOrDefault<T>(this IEnumerable<T> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        foreach (T item in source)
        {
            return item;
        }
        return default(T);
    }

    public static int Count<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        int count = 0;
        foreach (T item in source)
        {
            if (predicate(item))
                count++;
        }
        return count;
    }

    public static IEnumerable<T> OrderByDescending<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        where TKey : IComparable<TKey>
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

        return source.OrderBy(keySelector).Reverse();
    }
}
