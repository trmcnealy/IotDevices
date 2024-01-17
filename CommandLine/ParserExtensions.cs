// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;

namespace CommandLine.CommandLine
{
    //[System.Runtime.Versioning.NonVersionable]
    public static class ParserExtensions
    {
        public static ParseResult Parse(this Parser parser,
                                        string      s)
        {
            return parser.Parse(s.Tokenize().ToArray(), !s.EndsWith(" "));
        }
    }
}