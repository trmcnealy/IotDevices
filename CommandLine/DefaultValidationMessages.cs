// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;

namespace CommandLine.CommandLine
{
    //[System.Runtime.Versioning.NonVersionable]
    internal class DefaultValidationMessages : IValidationMessages
    {
        public string CommandAcceptsOnlyOneArgument(string command,
                                                    int    argumentCount)
        {
            return $"Command '{command}' only accepts a single argument but {argumentCount} were provided.";
        }

        public string CommandAcceptsOnlyOneSubcommand(string command,
                                                      string subcommandsSpecified)
        {
            return $"Command '{command}' only accepts a single subcommand but multiple were provided: {subcommandsSpecified}";
        }

        public string FileDoesNotExist(string filePath)
        {
            return $"File does not exist: {filePath}";
        }

        public string NoArgumentsAllowed(string option)
        {
            return $"Arguments not allowed for option: {option}";
        }

        public string OptionAcceptsOnlyOneArgument(string option,
                                                   int    argumentCount)
        {
            return $"Option '{option}' only accepts a single argument but {argumentCount} were provided.";
        }

        public string RequiredArgumentMissingForCommand(string command)
        {
            return $"Required argument missing for command: {command}";
        }

        public string RequiredArgumentMissingForOption(string option)
        {
            return $"Required argument missing for option: {option}";
        }

        public string RequiredCommandWasNotProvided()
        {
            return "Required command was not provided.";
        }

        public string UnrecognizedArgument(string   unrecognizedArg,
                                           string[] allowedValues)
        {
            return $"Argument '{unrecognizedArg}' not recognized. Must be one of:\n\t{string.Join("\n\t", allowedValues.Select(v => $"'{v}'"))}";
        }

        public string UnrecognizedCommandOrArgument(string arg)
        {
            return $"Unrecognized command or argument '{arg}'";
        }

        public string UnrecognizedOption(string   unrecognizedOption,
                                         string[] allowedValues)
        {
            return $"Option '{unrecognizedOption}' not recognized. Must be one of:\n\t{string.Join("\n\t", allowedValues.Select(v => $"'{v}'"))}";
        }
    }
}