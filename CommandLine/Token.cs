// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace CommandLine.CommandLine
{
    //[System.Runtime.Versioning.NonVersionable]
    public class Token
    {
        public string Value { get; }

        public TokenType Type { get; }

        public Token(string    value,
                     TokenType type)
        {
            Value = value ?? "";
            Type  = type;
        }

        public override string ToString()
        {
            return $"{Type}: {Value}";
        }
    }
}