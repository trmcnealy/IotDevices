// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLine.CommandLine
{
    //[System.Runtime.Versioning.NonVersionable]
    public class ArgumentsRule
    {
        private readonly Func<AppliedOption, string>            validate;
        private readonly Func<ParseResult, IEnumerable<string>> suggest;
        private readonly Func<string>                           defaultValue;

        public IReadOnlyCollection<string> AllowedValues { get; }

        internal Func<string> GetDefaultValue
        {
            get { return () => defaultValue(); }
        }

        public string Description { get; }

        public string Name { get; }

        internal Func<AppliedOption, object> Materializer { get; }

        public ArgumentsRule(Func<AppliedOption, string> validate)
            : this(validate, null)
        {
        }

        internal ArgumentsRule(Func<AppliedOption, string>            validate,
                               IReadOnlyCollection<string>            allowedValues = null,
                               Func<string>                           defaultValue  = null,
                               string                                 description   = null,
                               string                                 name          = null,
                               Func<ParseResult, IEnumerable<string>> suggest       = null,
                               Func<AppliedOption, object>            materialize   = null)
        {
            this.validate = validate ?? throw new ArgumentNullException(nameof(validate));

            this.defaultValue = defaultValue ?? (() => null);
            Description       = description;
            Name              = name;

            if (suggest == null)
            {
                this.suggest = result => AllowedValues.FindSuggestions(result);
            }
            else
            {
                this.suggest = result => suggest(result).ToArray().FindSuggestions(result.TextToMatch());
            }

            AllowedValues = allowedValues ?? Array.Empty<string>();

            Materializer = materialize;
        }

        public string Validate(AppliedOption option)
        {
            return validate(option);
        }

        internal IEnumerable<string> Suggest(ParseResult parseResult)
        {
            return suggest(parseResult);
        }

        internal object Materialize(AppliedOption appliedOption)
        {
            return Materializer?.Invoke(appliedOption);
        }
    }
}