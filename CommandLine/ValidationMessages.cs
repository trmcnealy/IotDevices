// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace CommandLine.CommandLine
{
    //[System.Runtime.Versioning.NonVersionable]
    public static class ValidationMessages
    {
        private static          IValidationMessages current  = new DefaultValidationMessages();
        private static readonly IValidationMessages @default = current;

        public static IValidationMessages Current
        {
            get { return current; }
            set { current = value ?? new DefaultValidationMessages(); }
        }

        internal static string NoArgumentsAllowed(string option)
        {
            return current.NoArgumentsAllowed(option).NotWhitespace() ?? @default.NoArgumentsAllowed(option);
        }

        internal static string CommandAcceptsOnlyOneArgument(string command,
                                                             int    argumentCount)
        {
            return current.CommandAcceptsOnlyOneArgument(command, argumentCount).NotWhitespace() ?? @default.CommandAcceptsOnlyOneArgument(command, argumentCount);
        }

        internal static string CommandAcceptsOnlyOneSubcommand(string command,
                                                               string subcommandsSpecified)
        {
            return current.CommandAcceptsOnlyOneSubcommand(command, subcommandsSpecified).NotWhitespace() ?? @default.CommandAcceptsOnlyOneSubcommand(command, subcommandsSpecified);
        }

        internal static string FileDoesNotExist(string filePath)
        {
            return current.FileDoesNotExist(filePath).NotWhitespace() ?? @default.FileDoesNotExist(filePath);
        }

        internal static string OptionAcceptsOnlyOneArgument(string option,
                                                            int    argumentCount)
        {
            return current.OptionAcceptsOnlyOneArgument(option, argumentCount).NotWhitespace() ?? @default.OptionAcceptsOnlyOneArgument(option, argumentCount);
        }

        internal static string RequiredArgumentMissingForCommand(string command)
        {
            return current.RequiredArgumentMissingForCommand(command).NotWhitespace() ?? @default.RequiredArgumentMissingForCommand(command);
        }

        internal static string RequiredArgumentMissingForOption(string option)
        {
            return current.RequiredArgumentMissingForOption(option).NotWhitespace() ?? @default.RequiredArgumentMissingForOption(option);
        }

        internal static string RequiredCommandWasNotProvided()
        {
            return current.RequiredCommandWasNotProvided().NotWhitespace() ?? @default.RequiredCommandWasNotProvided();
        }

        internal static string UnrecognizedArgument(string   unrecognizedArg,
                                                    string[] allowedValues)
        {
            return current.UnrecognizedArgument(unrecognizedArg, allowedValues).NotWhitespace() ?? @default.UnrecognizedArgument(unrecognizedArg, allowedValues);
        }

        internal static string UnrecognizedCommandOrArgument(string arg)
        {
            return current.UnrecognizedCommandOrArgument(arg).NotWhitespace() ?? @default.UnrecognizedCommandOrArgument(arg);
        }

        internal static string UnrecognizedOption(string   unrecognizedOption,
                                                  string[] allowedValues)
        {
            return current.UnrecognizedOption(unrecognizedOption, allowedValues).NotWhitespace() ?? @default.UnrecognizedOption(unrecognizedOption, allowedValues);
        }
    }
}