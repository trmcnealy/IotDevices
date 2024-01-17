// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLine.CommandLine
{
    //[System.Runtime.Versioning.NonVersionable]
    public static class AppliedOptionExtensions
    {
        public static IEnumerable<OptionError> ValidateAll(this AppliedOption option)
        {
            return new[] {option.Validate()}.Concat(option.AppliedOptions.SelectMany(ValidateAll)).Where(o => o != null);
        }

        internal static IEnumerable<AppliedOption> FlattenBreadthFirst(this IEnumerable<AppliedOption> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            foreach (AppliedOption item in options.FlattenBreadthFirst(o => o.AppliedOptions))
            {
                yield return item;
            }
        }

        public static bool HasOption(this AppliedOption option,
                                     string             alias)
        {
            if (option == null)
            {
                throw new ArgumentNullException(nameof(option));
            }

            return option.AppliedOptions.Contains(alias);
        }

        public static IReadOnlyCollection<string> IfOptionGetArguments(this AppliedOption option, string alias)
        {
            if (option == null)
            {
                throw new ArgumentNullException(nameof(option));
            }

            if(option.HasOption(alias))
            {
                return option[alias].Arguments;
            }

            return Array.Empty<string>();
        }

        public static T IfOptionGetValue<T>(this AppliedOption option, string alias)
        {
            if (option == null)
            {
                throw new ArgumentNullException(nameof(option));
            }

            if(option.HasOption(alias))
            {
                return option[alias].Value<T>();
            }

            return default;
        }
    }
}