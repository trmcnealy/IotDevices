// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CommandLine.CommandLine
{
    //[System.Runtime.Versioning.NonVersionable]
    public class Parser
    {
        private readonly ParserConfiguration configuration;

        public OptionSet DefinedOptions
        {
            get { return configuration.DefinedOptions; }
        }

        public Parser(params Option[] options)
            : this(new ParserConfiguration(options))
        {
        }

        public Parser(char[]          delimiters,
                      params Option[] options)
            : this(new ParserConfiguration(options, delimiters))
        {
        }

        public Parser(ParserConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public ParseResult Parse(string[] args)
        {
            return Parse(args, false);
        }

        internal ParseResult Parse(IReadOnlyCollection<string> rawArgs,
                                   bool                        isProgressive)
        {
            Queue<Token>        unparsedTokens     = new Queue<Token>(NormalizeRootCommand(rawArgs).Lex(configuration));
            AppliedOptionSet    rootAppliedOptions = new AppliedOptionSet();
            List<AppliedOption> allAppliedOptions  = new List<AppliedOption>();
            List<OptionError>   errors             = new List<OptionError>();
            List<string>        unmatchedTokens    = new List<string>();

            while (unparsedTokens.Any())
            {
                Token token = unparsedTokens.Dequeue();

                if (token.Type == TokenType.EndOfArguments)
                {
                    // stop parsing further tokens
                    break;
                }

                if (token.Type != TokenType.Argument)
                {
                    Option definedOption = DefinedOptions.SingleOrDefault(o => o.HasAlias(token.Value));

                    if (definedOption != null)
                    {
                        AppliedOption appliedOption = allAppliedOptions.LastOrDefault(o => o.HasAlias(token.Value));

                        if (appliedOption == null)
                        {
                            appliedOption = new AppliedOption(definedOption, token.Value);
                            rootAppliedOptions.Add(appliedOption);
                        }

                        allAppliedOptions.Add(appliedOption);

                        continue;
                    }
                }

                bool added = false;

                foreach (AppliedOption appliedOption in Enumerable.Reverse(allAppliedOptions))
                {
                    AppliedOption option = appliedOption.TryTakeToken(token);

                    if (option != null)
                    {
                        allAppliedOptions.Add(option);
                        added = true;
                        break;
                    }

                    if (token.Type == TokenType.Argument &&
                        appliedOption.Option.IsCommand)
                    {
                        break;
                    }
                }

                if (!added)
                {
                    unmatchedTokens.Add(token.Value);
                }
            }

            if (rootAppliedOptions.Command()?.TreatUnmatchedTokensAsErrors == true)
            {
                errors.AddRange(unmatchedTokens.Select(UnrecognizedArg));
            }

            if (configuration.RootCommandIsImplicit)
            {
                rawArgs = rawArgs.Skip(1).ToArray();
                AppliedOption[] appliedOptions = rootAppliedOptions.SelectMany(o => o.AppliedOptions).ToArray();
                rootAppliedOptions = new AppliedOptionSet(appliedOptions);
            }

            return new ParseResult(rawArgs, rootAppliedOptions, isProgressive, configuration, unparsedTokens.Select(t => t.Value).ToArray(), unmatchedTokens, errors);
        }

        internal IReadOnlyCollection<string> NormalizeRootCommand(IReadOnlyCollection<string> args)
        {
            if (configuration.RootCommandIsImplicit)
            {
                args = new[] {configuration.RootCommand.Name}.Concat(args).ToArray();
            }

            string firstArg = args.FirstOrDefault();

            if (DefinedOptions.Count != 1)
            {
                return args;
            }

            string commandName = DefinedOptions.SingleOrDefault(o => o.IsCommand)?.Name;

            if (commandName == null ||
                string.Equals(firstArg, commandName, StringComparison.OrdinalIgnoreCase))
            {
                return args;
            }

            if (firstArg != null                               &&
                firstArg.Contains(Path.DirectorySeparatorChar) &&
                (firstArg.EndsWith(commandName, StringComparison.OrdinalIgnoreCase) || firstArg.EndsWith($"{commandName}.exe", StringComparison.OrdinalIgnoreCase)))
            {
                args = new[] {commandName}.Concat(args.Skip(1)).ToArray();
            }
            else
            {
                args = new[] {commandName}.Concat(args).ToArray();
            }

            return args;
        }

        private static OptionError UnrecognizedArg(string arg)
        {
            return new OptionError(ValidationMessages.UnrecognizedCommandOrArgument(arg), arg);
        }
    }
}