using System;
using System.Collections.Generic;

namespace spnode.utils
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> ForEachThen<T>(
            this IEnumerable<T> source,
            Action<T> action)
        {
            foreach (var element in source)
            {
                action(element);
                yield return element;
            }
        }

        public static void ForEach<T>(
            this IEnumerable<T> source,
            Action<T> action)
        {
            foreach (var element in source)
            {
                action(element);
            }
        }
    }
}
