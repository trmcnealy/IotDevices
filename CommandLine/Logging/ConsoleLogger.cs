using System;
using System.IO;

namespace CommandLine.CommandLine.Logging
{

    /// <summary>
    ///     Default logger to Console.Out.
    /// </summary>
    //[System.Runtime.Versioning.NonVersionable]
    public class ConsoleLogger : LoggerBase
    {
        /// <summary>
        ///     Gets or sets the output <see cref="TextWriter" />. Default is set to <see cref="Console.Out" />.
        /// </summary>
        /// <value>The output <see cref="TextWriter" />.</value>
        public TextWriter Output { get; set; }

        public ConsoleLogger()
        {
            Output = Console.Out;
        }

        /// <summary>
        ///     Exits the process with the specified reason.
        /// </summary>
        /// <param name="reason">The reason.</param>
        /// <param name="exitCode">The exit code</param>
        public override void Exit(string reason, int exitCode)
        {
            if(Output == null)
            {
                return;
            }

            Logger.Error("Process stopped. " + reason);
            Environment.Exit(exitCode);
        }

        /// <summary>
        ///     Logs the specified log message.
        /// </summary>
        /// <param name="logLevel">The log level</param>
        /// <param name="logLocation">The log location.</param>
        /// <param name="context">The context.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="parameters">The parameters.</param>
        public override void Log(LogLevel logLevel, LogLocation logLocation, string context, string message, Exception exception, params object[] parameters)
        {
            lock(this)
            {
                if(Output == null)
                {
                    return;
                }

                switch(logLevel)
                {
                    case LogLevel.Info:
                        LogInfo(logLocation, context, message, exception, parameters);
                        break;
                    case LogLevel.Warning:
                        LogWarning(logLocation, context, message, exception, parameters);
                        break;
                    case LogLevel.Error:
                        LogError(logLocation, context, message, exception, parameters);
                        break;
                    case LogLevel.Fatal:
                        LogFatal(logLocation, context, message, exception, parameters);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
                }

                if(exception != null)
                {
                    LogException(logLocation, exception);
                }
            }
        }

        public void LogInfo(LogLocation logLocation, string context, string message, Exception exception, params object[] parameters)
        {
            string lineMessage = FormatMessage(LogLevel.Info, logLocation, context, message, exception, parameters);
            Output.WriteLine(ConsoleColor.Gray, lineMessage);
            Output.Flush();
        }

        public void LogWarning(LogLocation logLocation, string context, string message, Exception exception, params object[] parameters)
        {
            string lineMessage = FormatMessage(LogLevel.Warning, logLocation, context, message, exception, parameters);
            Output.WriteLine(ConsoleColor.Yellow, lineMessage);
            Output.Flush();
        }

        public void LogError(LogLocation logLocation, string context, string message, Exception exception, params object[] parameters)
        {
            string lineMessage = FormatMessage(LogLevel.Error, logLocation, context, message, exception, parameters);
            Output.WriteLine(ConsoleColor.Red, lineMessage);
            Output.Flush();
        }

        public void LogFatal(LogLocation logLocation, string context, string message, Exception exception, params object[] parameters)
        {
            string lineMessage = FormatMessage(LogLevel.Fatal, logLocation, context, message, exception, parameters);
            Output.WriteLine(ConsoleColor.DarkRed, lineMessage);
            Output.Flush();
        }
    }

}
