using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CommandLine.CommandLine.Logging
{

    /// <summary>
    ///     <see cref="ILogger" /> base implementation.
    /// </summary>
    //[System.Runtime.Versioning.NonVersionable]
    public abstract class LoggerBase : ILogger
    {
        private static readonly Regex regex = new Regex(@"^\s*at\s+([^\)]+)\)\s+in\s+(.*):line\s+(\d+)");

        /// <summary>
        ///     Formats the message.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="logLocation">The log location.</param>
        /// <param name="context">The context.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static string FormatMessage(LogLevel        logLevel,
                                           LogLocation     logLocation,
                                           string          context,
                                           string          message,
                                           Exception       exception,
                                           params object[] parameters)
        {
            StringBuilder lineMessage = new StringBuilder();

            if(logLocation != null)
            {
                lineMessage.AppendFormat("{0}({1},{2}): ", logLocation.File, logLocation.Line, logLocation.Column);
            }

            // Write log parsable by Visual Studio
            string levelName = Enum.GetName(typeof(LogLevel), logLevel).
                                    ToLower();

            lineMessage.AppendFormat("{0}:{1}", levelName == "fatal" ? "error:fatal" : levelName, FormatMessage(context, message, parameters));
            return lineMessage.ToString();
        }

        /// <summary>
        ///     Formats the message.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="message">The message.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static string FormatMessage(string context, string message, params object[] parameters)
        {
            StringBuilder lineMessage = new StringBuilder();

            // Write log parsable by Visual Studio
            lineMessage.AppendFormat("{0}{1}", context != null ? " in " + context + " " : "", message != null ? string.Format(message, parameters) : "");
            return lineMessage.ToString();
        }

        /// <summary>
        ///     Logs an exception.
        /// </summary>
        /// <param name="logLocation">The log location.</param>
        /// <param name="ex">The exception to log.</param>
        protected void LogException(LogLocation logLocation, Exception ex)
        {
            // Print friendly error parsable by Visual Studio in order to display them in the Error List
            StringReader reader = new StringReader(ex.ToString());

            // And write the exception parsable by Visual Studio
            string line;

            while((line = reader.ReadLine()) != null)
            {
                Match match = regex.Match(line);

                if(match.Success)
                {
                    string methodLocation = match.Groups[1].
                                                  Value;

                    string fileName = match.Groups[2].
                                            Value;

                    int lineNumber;
                    int.TryParse(match.Groups[3].
                                       Value,
                                 out lineNumber);

                    Log(LogLevel.Error, new LogLocation(fileName, lineNumber, 1), methodLocation, "Exception", null);
                }
                else
                {
                    // Escape a line
                    Log(LogLevel.Error,
                        logLocation,
                        null,
                        line.Replace("{", "{{").
                             Replace("}", "}}"),
                        null);
                }
            }
        }

        /// <summary>
        ///     Exits the process with the specified reason.
        /// </summary>
        /// <param name="reason">The reason.</param>
        /// <param name="exitCode">The exit code</param>
        public abstract void Exit(string reason, int exitCode);

        /// <summary>
        ///     Logs the specified log message.
        /// </summary>
        /// <param name="logLevel">The log level</param>
        /// <param name="logLocation">The log location.</param>
        /// <param name="context">The context.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="parameters">The parameters.</param>
        public abstract void Log(LogLevel logLevel, LogLocation logLocation, string context, string message, Exception exception, params object[] parameters);
    }

}
