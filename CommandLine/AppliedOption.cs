// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLine.CommandLine
{
    //[System.Runtime.Versioning.NonVersionable]
    public class AppliedOption
    {
        private readonly List<string> arguments = new List<string>();
        private readonly Lazy<string> defaultValue;
        private readonly Func<object> materialize;
        private          bool         considerAcceptingAnotherArgument = true;

        public AppliedOptionSet AppliedOptions { get; } = new AppliedOptionSet();

        public IReadOnlyCollection<string> Arguments
        {
            get
            {
                if (arguments.Any() ||
                    defaultValue.Value == null)
                {
                    return arguments.ToArray();
                }

                return new[] {defaultValue.Value};
            }
        }

        public string Name
        {
            get { return Option.Name; }
        }

        public Option Option { get; }

        public string Token { get; }

        public AppliedOption this[string alias]
        {
            get { return AppliedOptions[alias]; }
        }

        public IReadOnlyCollection<string> Aliases
        {
            get { return Option.Aliases; }
        }

        public AppliedOption(Option option,
                             string token = null)
        {
            if (option == null)
            {
                throw new ArgumentNullException(nameof(option));
            }

            Option = option;

            defaultValue = new Lazy<string>(option.ArgumentsRule.GetDefaultValue);

            Token = token ?? option.ToString();

            materialize = () => option.ArgumentsRule.Materialize(this);
        }

        public AppliedOption TryTakeToken(Token token)
        {
            AppliedOption option = TryTakeArgument(token) ?? TryTakeOptionOrCommand(token);
            considerAcceptingAnotherArgument = false;
            return option;
        }

        private AppliedOption TryTakeArgument(Token token)
        {
            if (token.Type != TokenType.Argument)
            {
                return null;
            }

            if (!considerAcceptingAnotherArgument &&
                !Option.IsCommand)
            {
                // Options must be respecified in order to accept additional arguments. This is 
                // not the case for commands.
                return null;
            }

            foreach (AppliedOption appliedOption in AppliedOptions)
            {
                AppliedOption a = appliedOption.TryTakeToken(token);
                if (a != null)
                {
                    return a;
                }
            }

            arguments.Add(token.Value);

            if (Validate() == null)
            {
                considerAcceptingAnotherArgument = false;
                return this;
            }

            arguments.RemoveAt(arguments.Count - 1);
            return null;
        }

        private AppliedOption TryTakeOptionOrCommand(Token token)
        {
            AppliedOption childOption = AppliedOptions.SingleOrDefault(o => o.Option.DefinedOptions.Any(oo => oo.RawAliases.Contains(token.Value)));

            if (childOption != null)
            {
                return childOption.TryTakeToken(token);
            }

            if (token.Type == TokenType.Command &&
                AppliedOptions.Any(o => o.Option.IsCommand && !o.HasAlias(token.Value)))
            {
                // if a subcommand has already been applied, don't accept this one
                return null;
            }

            AppliedOption applied = AppliedOptions.SingleOrDefault(o => o.Option.HasRawAlias(token.Value));

            if (applied != null)
            {
                applied.OptionWasRespecified();
                return applied;
            }

            applied = Option.DefinedOptions.Where(o => o.RawAliases.Contains(token.Value)).Select(o => new AppliedOption(o, token.Value)).SingleOrDefault();

            if (applied != null)
            {
                AppliedOptions.Add(applied);
            }

            return applied;
        }

        internal void OptionWasRespecified()
        {
            considerAcceptingAnotherArgument = true;
        }

        internal OptionError Validate()
        {
            string error = Option.ArgumentsRule.Validate(this);
            return string.IsNullOrWhiteSpace(error) ? null : new OptionError(error, Token, this);
        }

        public bool HasAlias(string alias)
        {
            return Option.HasAlias(alias);
        }

        public T Value<T>()
        {
            return (T) Value();
        }

        public object Value()
        {
            try
            {
                return materialize();
            }
            catch (Exception exception)
            {
                string argumentsDescription = Arguments.Any() ? string.Join(", ", Arguments) : " (none)";
                throw new ParseException($"An exception occurred while getting the value for option '{Option.Name}' based on argument(s): {argumentsDescription}.", exception);
            }
        }

        public override string ToString()
        {
            return this.Diagram();
        }
    }
}