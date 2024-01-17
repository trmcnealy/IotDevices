using System;

namespace CommandLine.CommandLine
{
    //[System.Runtime.Versioning.NonVersionable]
    public class ParseException : Exception
    {
        public ParseException(string message)
            : base(message)
        {
        }

        public ParseException(string    message,
                              Exception innerException)
            : base(message, innerException)
        {
        }
    }
}