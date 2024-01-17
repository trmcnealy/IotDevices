using System;
using System.IO;

namespace CommandLine.CommandLine
{
    //[System.Runtime.Versioning.NonVersionable]
    public static class ConsoleMethods
    {
        public static void WriteLine(this TextWriter textWriter, ConsoleColor textColor,
                                     string       msg)
        {
            Console.ForegroundColor = textColor;
            textWriter.WriteLine(msg);
            Console.ResetColor();
        }

        public static void WriteLine(this TextWriter textWriter, ConsoleColor textColor,
                                     string       format,
                                     object       arg0)
        {
            Console.ForegroundColor = textColor;
            textWriter.WriteLine(string.Format(format, arg0));
            Console.ResetColor();
        }

        public static void WriteLine<T>(this TextWriter textWriter, ConsoleColor textColor,
                                        string       format,
                                        params T[]   arg)
        {
            Console.ForegroundColor = textColor;
            textWriter.WriteLine(string.Format(format, arg));
            Console.ResetColor();
        }

        public static void WriteLine(this TextWriter textWriter, ConsoleColor textColor,
                                     string       description,
                                     string       title)
        {
            Console.ForegroundColor = textColor;
            textWriter.WriteLine(description, title, null, null);
            Console.ResetColor();
        }

        public static void WriteLine(this TextWriter textWriter, ConsoleColor textColor,
                                     string       description,
                                     string       file,
                                     int          line,
                                     int          column)
        {
            Console.ForegroundColor = textColor;
            textWriter.WriteLine(description, file, line, column);
            Console.ResetColor();
        }
    }
}
