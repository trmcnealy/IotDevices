// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLine.CommandLine
{
    //[System.Runtime.Versioning.NonVersionable]
    internal static class EnumerableExtensions
    {
        internal static IEnumerable<T> Do<T>(this IEnumerable<T> source,
                                             Action<T>           action)
        {
            return source.Select(x =>
                                 {
                                     action(x);
                                     return x;
                                 });
        }

        internal static IEnumerable<T> FlattenBreadthFirst<T>(this IEnumerable<T>     source,
                                                              Func<T, IEnumerable<T>> children)
        {
            Queue<T> queue = new Queue<T>();

            foreach (T option in source)
            {
                queue.Enqueue(option);
            }

            while (queue.Count > 0)
            {
                T current = queue.Dequeue();

                foreach (T option in children(current))
                {
                    queue.Enqueue(option);
                }

                yield return current;
            }
        }

        internal static IEnumerable<T> RecurseWhileNotNull<T>(this T     source,
                                                              Func<T, T> next)
            where T : class
        {
            yield return source;

            while ((source = next(source)) != null)
            {
                yield return source;
            }
        }
    }
}