// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;

namespace CommandLine.CommandLine
{
    //[System.Runtime.Versioning.NonVersionable]
    public class Command : Option
    {
        internal override bool IsCommand
        {
            get { return true; }
        }

        public bool TreatUnmatchedTokensAsErrors { get; } = true;

        public Command(string        name,
                       string        help,
                       Option[]      options                      = null,
                       ArgumentsRule arguments                    = null,
                       bool          treatUnmatchedTokensAsErrors = true)
            : base(new[] {name}, help, arguments, options)
        {
            TreatUnmatchedTokensAsErrors = treatUnmatchedTokensAsErrors;
        }

        public Command(string    name,
                       string    help,
                       Command[] subcommands)
            : base(new[] {name}, help, options: subcommands)
        {
            string[] commandNames = subcommands.SelectMany(o => o.Aliases).ToArray();

            ArgumentsRule = Accept.ExactlyOneCommandRequired().WithSuggestionsFrom(commandNames).And(ArgumentsRule);
        }
    }
}