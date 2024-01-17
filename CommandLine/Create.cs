// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Reflection;

namespace CommandLine.CommandLine
{
    //[System.Runtime.Versioning.NonVersionable]
    public static class Create
    {
        private static readonly Lazy<string> executableName = new Lazy<string>(() => Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location));

        public static Option Option(string        aliases,
                                    string        help,
                                    ArgumentsRule arguments = null)
        {
            return new Option(aliases.Split(new[]
                                            {
                                                '|',
                                                ' '
                                            }, StringSplitOptions.RemoveEmptyEntries), help, arguments);
        }

        [Obsolete("Do not use this overload. It will be removed. materialize argument is unused.", true)]
        public static Option Option(string                      aliases,
                                    string                      help,
                                    ArgumentsRule               arguments,
                                    Func<AppliedOption, object> materialize)
        {
            return Option(aliases, help, arguments);
        }

        public static Command Command(string name,
                                      string help)
        {
            return new Command(name, help);
        }

        public static Command Command(string          name,
                                      string          help,
                                      params Option[] options)
        {
            return new Command(name, help, options);
        }

        public static Command Command(string          name,
                                      string          help,
                                      bool            treatUnmatchedTokensAsErrors,
                                      params Option[] options)
        {
            return new Command(name, help, options, treatUnmatchedTokensAsErrors: treatUnmatchedTokensAsErrors);
        }

        public static Command Command(string          name,
                                      string          help,
                                      ArgumentsRule   arguments,
                                      params Option[] options)
        {
            return new Command(name, help, options, arguments);
        }

        public static Command Command(string          name,
                                      string          help,
                                      ArgumentsRule   arguments,
                                      bool            treatUnmatchedTokensAsErrors,
                                      params Option[] options)
        {
            return new Command(name, help, options, arguments, treatUnmatchedTokensAsErrors);
        }

        public static Command Command(string           name,
                                      string           help,
                                      params Command[] commands)
        {
            return new Command(name, help, commands);
        }

        public static Command RootCommand(params Option[] options)
        {
            return Command(executableName.Value, "", Accept.NoArguments(), options);
        }
    }
}