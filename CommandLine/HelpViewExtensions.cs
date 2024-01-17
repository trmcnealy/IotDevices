// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.CommandLine
{
    //[System.Runtime.Versioning.NonVersionable]
    public static class HelpViewExtensions
    {
        private static readonly int columnGutterWidth = 3;

        public static string HelpView(this Command command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            StringBuilder helpView = new StringBuilder();

            WriteSynopsis(command, helpView);

            WriteArgumentsSection(command, helpView);

            WriteOptionsSection(command, helpView);

            WriteSubcommandsSection(command, helpView);

            WriteAdditionalArgumentsSection(command, helpView);

            return helpView.ToString();
        }

        private static void WriteAdditionalArgumentsSection(Command       command,
                                                            StringBuilder helpView)
        {
            if (command.TreatUnmatchedTokensAsErrors)
            {
                return;
            }

            helpView.Append(DefaultHelpViewText.AdditionalArgumentsSection);
        }

        private static void WriteArgumentsSection(Command       command,
                                                  StringBuilder helpView)
        {
            string argName        = command.ArgumentsRule.Name;
            string argDescription = command.ArgumentsRule.Description;

            bool shouldWriteCommandArguments = !string.IsNullOrWhiteSpace(argName) && !string.IsNullOrWhiteSpace(argDescription);

            Command? parentCommand = command.Parent as Command;

            string? parentArgName        = parentCommand?.ArgumentsRule?.Name;
            string? parentArgDescription = parentCommand?.ArgumentsRule?.Description;

            bool shouldWriteParentCommandArguments = !string.IsNullOrWhiteSpace(parentArgName) && !string.IsNullOrWhiteSpace(parentArgDescription);

            if (shouldWriteCommandArguments || shouldWriteParentCommandArguments)
            {
                helpView.AppendLine();
                helpView.AppendLine(DefaultHelpViewText.ArgumentsSection.Title);
            }
            else
            {
                return;
            }

            string indent                  = "  ";
            string argLeftColumnText       = $"{indent}<{argName}>";
            string parentArgLeftColumnText = $"{indent}<{parentArgName}>";
            int  leftColumnWidth         = Math.Max(argLeftColumnText.Length, parentArgLeftColumnText.Length) + columnGutterWidth;

            if (shouldWriteParentCommandArguments)
            {
                WriteColumnizedSummary(parentArgLeftColumnText, parentArgDescription, leftColumnWidth, helpView);
            }

            if (shouldWriteCommandArguments)
            {
                WriteColumnizedSummary(argLeftColumnText, argDescription, leftColumnWidth, helpView);
            }
        }

        private static void WriteOptionsSection(Command       command,
                                                StringBuilder helpView)
        {
            Option[] options = command.DefinedOptions.Where(o => !o.IsCommand).Where(o => !o.IsHidden()).ToArray();

            if (!options.Any())
            {
                return;
            }

            helpView.AppendLine();
            helpView.AppendLine(DefaultHelpViewText.OptionsSection.Title);

            WriteOptionsList(options, helpView);
        }

        private static void WriteSubcommandsSection(Command       command,
                                                    StringBuilder helpView)
        {
            Command[] subcommands = command.DefinedOptions.Where(o => !o.IsHidden()).OfType<Command>().ToArray();

            if (!subcommands.Any())
            {
                return;
            }

            helpView.AppendLine();
            helpView.AppendLine(DefaultHelpViewText.CommandsSection.Title);

            WriteOptionsList(subcommands, helpView);
        }

        private static void WriteOptionsList(Option[]      options,
                                             StringBuilder helpView)
        {
            Dictionary<Option, string> leftColumnTextFor = options.ToDictionary(o => o, LeftColumnText);

            int leftColumnWidth = leftColumnTextFor.Values.Select(s => s.Length).OrderBy(length => length).Last() + columnGutterWidth;

            foreach (Option option in options)
            {
                WriteColumnizedSummary(leftColumnTextFor[option], option.HelpText, leftColumnWidth, helpView);
            }
        }

        private static string LeftColumnText(Option option)
        {
            string leftColumnText = "  " + string.Join(", ", option.RawAliases.OrderBy(a => a.Length).
                                                                    Select(a =>
                                                                           {
                                                                               if (option.IsCommand)
                                                                               {
                                                                                   return a.TrimStart('-');
                                                                               }

                                                                               return a;
                                                                           }));

            string argumentName = option.ArgumentsRule.Name;

            if (!string.IsNullOrWhiteSpace(argumentName))
            {
                leftColumnText += $" <{argumentName}>";
            }

            return leftColumnText;
        }

        private static void WriteColumnizedSummary(string        leftColumnText,
                                                   string        rightColumnText,
                                                   int           width,
                                                   StringBuilder helpView)
        {
            helpView.Append(leftColumnText);

            if (leftColumnText.Length <= width - 2)
            {
                helpView.Append(new string(' ', width - leftColumnText.Length));
            }
            else
            {
                helpView.AppendLine();
                helpView.Append(new string(' ', width));
            }

            string descriptionWithLineWraps = string.Join(Environment.NewLine + new string(' ', width), rightColumnText.Split(new[]
                                                                                                                              {
                                                                                                                                  '\r',
                                                                                                                                  '\n'
                                                                                                                              }, StringSplitOptions.RemoveEmptyEntries).
                                                                                                                        Select(s => s.Trim()));

            helpView.AppendLine(descriptionWithLineWraps);
        }

        private static void WriteSynopsis(Command       command,
                                          StringBuilder helpView)
        {
            helpView.Append(DefaultHelpViewText.Synopsis.Title);

            foreach (Command subcommand in command.RecurseWhileNotNull(c => c.Parent as Command).Reverse())
            {
                helpView.Append($" {subcommand.Name}");

                string argsName = subcommand.ArgumentsRule.Name;
                if (subcommand != command &&
                    !string.IsNullOrWhiteSpace(argsName))
                {
                    helpView.Append($" <{argsName}>");
                }
            }

            if (command.DefinedOptions.Any(o => !o.IsCommand && !o.IsHidden()))
            {
                helpView.Append(DefaultHelpViewText.Synopsis.Options);
            }

            string argumentsName = command.ArgumentsRule.Name;
            if (!string.IsNullOrWhiteSpace(argumentsName))
            {
                helpView.Append($" <{argumentsName}>");
            }

            if (command.DefinedOptions.OfType<Command>().Any())
            {
                helpView.Append(DefaultHelpViewText.Synopsis.Command);
            }

            if (!command.TreatUnmatchedTokensAsErrors)
            {
                helpView.Append(DefaultHelpViewText.Synopsis.AdditionalArguments);
            }

            helpView.AppendLine();
        }
    }
}