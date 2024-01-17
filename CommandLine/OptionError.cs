// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace CommandLine.CommandLine
{
    //[System.Runtime.Versioning.NonVersionable]
    public class OptionError
    {
        public string Message { get; }

        public AppliedOption Option { get; }

        public string Token { get; }

        public OptionError(string        message,
                           string        token,
                           AppliedOption option = null)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(message));
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(token));
            }

            Message = message;
            Option  = option;
            Token   = token;
        }

        public override string ToString()
        {
            return Message;
        }
    }
}