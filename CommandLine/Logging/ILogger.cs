using System;

namespace CommandLine.CommandLine.Logging
{

    /// <summary>
    ///     Logging interface for backend output.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        ///     Exits the process with the specified reason.
        /// </summary>
        /// <param name="reason">The reason.</param>
        /// <param name="exitCode">The exit code</param>
        void Exit(string reason, int exitCode);

        /// <summary>
        ///     Logs the specified log message.
        /// </summary>
        /// <param name="logLevel">The log level</param>
        /// <param name="logLocation">The log location.</param>
        /// <param name="context">The context.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="parameters">The parameters.</param>
        void Log(LogLevel logLevel, LogLocation logLocation, string context, string message, Exception exception, params object[] parameters);
    }

}
