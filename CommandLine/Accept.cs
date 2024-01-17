// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CommandLine.CommandLine
{
    //[System.Runtime.Versioning.NonVersionable]
    public static class Accept
    {
        public static ArgumentsRule AnyOneOf(params string[] values)
        {
            return ExactlyOneArgument().
                And(new ArgumentsRule(o =>
                                      {
                                          string arg = o.Arguments.Single();

                                          return !values.Contains(arg, StringComparer.OrdinalIgnoreCase) ? ValidationMessages.UnrecognizedArgument(arg, values) : "";
                                      }, values));
        }

        public static ArgumentsRule AnyOneOf(Func<IEnumerable<string>> getValues)
        {
            return ExactlyOneArgument().
                   And(new ArgumentsRule(o =>
                                         {
                                             string[] values = getValues().ToArray();

                                             string arg = o.Arguments.Single();

                                             return !values.Contains(arg, StringComparer.OrdinalIgnoreCase) ? ValidationMessages.UnrecognizedArgument(arg, values) : "";
                                         })).
                   WithSuggestionsFrom(_ => getValues());
        }

        public static ArgumentsRule ExactlyOneArgument(Func<AppliedOption, string> errorMessage = null)
        {
            return new ArgumentsRule(o =>
                                     {
                                         int argumentCount = o.Arguments.Count;

                                         if (argumentCount == 0)
                                         {
                                             if (errorMessage == null)
                                             {
                                                 return o.Option.IsCommand ? ValidationMessages.RequiredArgumentMissingForCommand(o.Option.ToString()) :
                                                            ValidationMessages.RequiredArgumentMissingForOption(o.Option.ToString());
                                             }

                                             return errorMessage(o);
                                         }

                                         if (argumentCount > 1)
                                         {
                                             if (errorMessage == null)
                                             {
                                                 return o.Option.IsCommand ? ValidationMessages.CommandAcceptsOnlyOneArgument(o.Option.ToString(), argumentCount) :
                                                            ValidationMessages.OptionAcceptsOnlyOneArgument(o.Option.ToString(), argumentCount);
                                             }

                                             return errorMessage(o);
                                         }

                                         return null;
                                     }, materialize: o => o.Arguments.SingleOrDefault());
        }

        public static ArgumentsRule ExistingFilesOnly(this ArgumentsRule rule)
        {
            return rule.And(new ArgumentsRule(o => o.Arguments.Where(filePath => !File.Exists(filePath) && !Directory.Exists(filePath)).Select(ValidationMessages.FileDoesNotExist).FirstOrDefault()));
        }

        public static ArgumentsRule LegalFilePathsOnly(this ArgumentsRule rule)
        {
            return rule.And(new ArgumentsRule(o =>
                                              {
                                                  foreach (string arg in o.Arguments)
                                                  {
                                                      try
                                                      {
                                                          FileInfo fileInfo = new FileInfo(arg);
                                                      }
                                                      catch (NotSupportedException ex)
                                                      {
                                                          return ex.Message;
                                                      }
                                                      catch (ArgumentException ex)
                                                      {
                                                          return ex.Message;
                                                      }
                                                  }

                                                  return null;
                                              }));
        }

        public static ArgumentsRule WithSuggestionsFrom(params string[] values)
        {
            return new ArgumentsRule(_ => null, suggest: parseResult => values.FindSuggestions(parseResult.TextToMatch()));
        }

        public static ArgumentsRule WithSuggestionsFrom(Func<string, IEnumerable<string>> suggest)
        {
            return new ArgumentsRule(_ => null, suggest: parseResult => suggest(parseResult.TextToMatch()));
        }

        public static ArgumentsRule WithSuggestionsFrom(this ArgumentsRule                rule,
                                                        Func<string, IEnumerable<string>> suggest)
        {
            return rule.And(WithSuggestionsFrom(suggest));
        }

        public static ArgumentsRule WithSuggestionsFrom(this   ArgumentsRule rule,
                                                        params string[]      values)
        {
            return rule.And(WithSuggestionsFrom(values));
        }

        public static ArgumentsRule ZeroOrOneArgument()
        {
            return new ArgumentsRule(o =>
                                     {
                                         if (o.Arguments.Count > 1)
                                         {
                                             return o.Option.IsCommand ? ValidationMessages.CommandAcceptsOnlyOneArgument(o.Option.ToString(), o.Arguments.Count) :
                                                        ValidationMessages.OptionAcceptsOnlyOneArgument(o.Option.ToString(), o.Arguments.Count);
                                         }

                                         return null;
                                     }, materialize: o => o.Arguments.SingleOrDefault());
        }

        internal static ArgumentsRule ExactlyOneCommandRequired(Func<AppliedOption, string> errorMessage = null)
        {
            return new ArgumentsRule(o =>
                                     {
                                         int optionCount = o.AppliedOptions.Count;

                                         if (optionCount == 0)
                                         {
                                             if (errorMessage == null)
                                             {
                                                 return ValidationMessages.RequiredArgumentMissingForCommand(o.Option.ToString());
                                             }

                                             return errorMessage(o);
                                         }

                                         if (optionCount > 1)
                                         {
                                             if (errorMessage == null)
                                             {
                                                 return ValidationMessages.CommandAcceptsOnlyOneSubcommand(o.Option.ToString(), string.Join(", ", o.AppliedOptions.Select(a => a.Option)));
                                             }

                                             return errorMessage(o);
                                         }

                                         return null;
                                     });
        }

        public static ArgumentsRule NoArguments(Func<AppliedOption, string> errorMessage = null)
        {
            return new ArgumentsRule(o =>
                                     {
                                         if (!o.Arguments.Any())
                                         {
                                             return null;
                                         }

                                         if (errorMessage == null)
                                         {
                                             return ValidationMessages.NoArgumentsAllowed(o.Option.ToString());
                                         }

                                         return errorMessage(o);
                                     }, materialize: _ => true);
        }

        public static ArgumentsRule OneOrMoreArguments(Func<AppliedOption, string> errorMessage = null)
        {
            return new ArgumentsRule(o =>
                                     {
                                         int optionCount = o.Arguments.Count;

                                         if (optionCount == 0)
                                         {
                                             if (errorMessage == null)
                                             {
                                                 return o.Option.IsCommand ? ValidationMessages.RequiredArgumentMissingForCommand(o.Option.ToString()) :
                                                            ValidationMessages.RequiredArgumentMissingForOption(o.Option.ToString());
                                             }

                                             return errorMessage(o);
                                         }

                                         return null;
                                     }, materialize: o => o.Arguments);
        }

        internal static ArgumentsRule ZeroOrMoreOf(params Option[] options)
        {
            string[] values = options.SelectMany(o => o.RawAliases).ToArray();

            string[] completionValues = options.Where(o => !o.IsHidden()).SelectMany(o => o.RawAliases).ToArray();

            return new ArgumentsRule(o =>
                                     {
                                         string unrecognized = values.FirstOrDefault(v => !o.Option.DefinedOptions.Any(oo => oo.HasAlias(v)));

                                         if (unrecognized != null)
                                         {
                                             return ValidationMessages.UnrecognizedOption(unrecognized, values);
                                         }

                                         return null;
                                     }, completionValues);
        }

        public static ArgumentsRule ZeroOrMoreArguments()
        {
            return new ArgumentsRule(_ => null, materialize: o => o.Arguments);
        }
    }
}