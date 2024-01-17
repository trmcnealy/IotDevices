﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CommandLine.CommandLine.Logging
{

    /// <summary>
    ///     Root Logger class.
    /// </summary>
    //[System.Runtime.Versioning.NonVersionable]
    public class Logger
    {
        private static readonly string DefaultFilePathError = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().
                                                                                                        Location);

        private static          int                _errorCount;
        private static readonly List<string>       ContextStack      = new List<string>();
        private static readonly Stack<LogLocation> FileLocationStack = new Stack<LogLocation>();

        /// <summary>
        ///     Gets the context as a string.
        /// </summary>
        /// <value>The context.</value>
        private static string ContextAsText
        {
            get { return HasContext ? string.Join("/", ContextStack) : null; }
        }

        /// <summary>
        ///     Gets or sets the logger output.
        /// </summary>
        /// <value>The logger output.</value>
        public static ILogger LoggerOutput { get; set; }

        /// <summary>
        ///     Gets a value indicating whether this instance has context.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has context; otherwise, <c>false</c>.
        /// </value>
        private static bool HasContext
        {
            get { return ContextStack.Count > 0; }
        }

        /// <summary>
        ///     Gets a value indicating whether this instance has errors.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </value>
        public static bool HasErrors
        {
            get { return _errorCount > 0; }
        }

        /// <summary>
        ///     Gets or sets the progress report.
        /// </summary>
        /// <value>The progress report.</value>
        public static IProgressReport ProgressReport { get; set; }

        /// <summary>
        ///     Initializes the <see cref="Logger" /> class.
        /// </summary>
        static Logger()
        {
            PushLocation(DefaultFilePathError);
            LoggerOutput = new ConsoleLogger();
        }

        /// <summary>
        ///     Logs the specified error.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="parameters">The parameters.</param>
        public static void Error(string message, Exception ex, params object[] parameters)
        {
            LogRawMessage(LogLevel.Error, message, ex, parameters);
            _errorCount++;
        }

        /// <summary>
        ///     Logs the specified error.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Error(string message)
        {
            Error("{0}", message);
        }

        /// <summary>
        ///     Logs the specified error.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="parameters">The parameters.</param>
        public static void Error(string message, params object[] parameters)
        {
            Error(message, null, parameters);
        }

        /// <summary>
        ///     Exits the process.
        /// </summary>
        /// <param name="reason">The reason.</param>
        /// <param name="parameters">The parameters.</param>
        public static void Exit(string reason, params object[] parameters)
        {
            string message = string.Format(reason, parameters);

            if(ProgressReport != null)
            {
                ProgressReport.FatalExit(message);
            }

            if(LoggerOutput != null)
            {
                LoggerOutput.Exit(message, 1);
            }
        }

        /// <summary>
        ///     Logs the specified fatal error.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The exception.</param>
        /// <param name="parameters">The parameters.</param>
        public static void Fatal(string message, Exception ex, params object[] parameters)
        {
            LogRawMessage(LogLevel.Fatal, message, ex, parameters);
            _errorCount++;
            //Exit("A fatal error occured");
        }

        /// <summary>
        ///     Logs the specified fatal error.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Fatal(string message)
        {
            Fatal("{0}", message);
        }

        /// <summary>
        ///     Logs the specified fatal error.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="parameters">The parameters.</param>
        public static void Fatal(string message, params object[] parameters)
        {
            Fatal(message, null, parameters);
        }

        /// <summary>
        ///     Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Message(string message)
        {
            Message("{0}", message);
        }

        /// <summary>
        ///     Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="parameters">The parameters.</param>
        public static void Message(string message, params object[] parameters)
        {
            LogRawMessage(LogLevel.Info, message, null, parameters);
        }

        /// <summary>
        ///     Pops the context.
        /// </summary>
        public static void PopContext()
        {
            if(ContextStack.Count > 0)
            {
                ContextStack.RemoveAt(ContextStack.Count - 1);
            }
        }

        /// <summary>
        ///     Pops the context location.
        /// </summary>
        public static void PopLocation()
        {
            FileLocationStack.Pop();
        }

        /// <summary>
        ///     Logs the specified progress level and message.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="message">The message.</param>
        /// <param name="parameters">The parameters.</param>
        public static void Progress(int level, string message, params object[] parameters)
        {
            Message(message, parameters);

            if(ProgressReport != null)
            {
                if(ProgressReport.ProgressStatus(level, string.Format(message, parameters)))
                {
                    Exit("Process aborted manually");
                }
            }
        }

        /// <summary>
        ///     Pushes a context string.
        /// </summary>
        /// <param name="context">The context.</param>
        public static void PushContext(string context)
        {
            ContextStack.Add(context);
        }

        /// <summary>
        ///     Pushes a context formatted string.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="parameters">The parameters.</param>
        public static void PushContext(string context, params object[] parameters)
        {
            ContextStack.Add(string.Format(context, parameters));
        }

        /// <summary>
        ///     Pushes a context location.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="line">The line.</param>
        /// <param name="column">The column.</param>
        public static void PushLocation(string fileName, int line = 1, int column = 1)
        {
            FileLocationStack.Push(new LogLocation(fileName, line, column));
        }

        /// <summary>
        ///     Runs a delegate in the specified log context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="method">The method.</param>
        public static void RunInContext(string context, MethodLogInContext method)
        {
            try
            {
                PushContext(context);
                method();
            }
            finally
            {
                PopContext();
            }
        }

        /// <summary>
        ///     Logs the specified warning.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Warning(string message)
        {
            Warning("{0}", message);
        }

        /// <summary>
        ///     Logs the specified warning.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="parameters">The parameters.</param>
        public static void Warning(string message, params object[] parameters)
        {
            LogRawMessage(LogLevel.Warning, message, null, parameters);
        }

        /// <summary>
        ///     Logs the raw message to the LoggerOutput.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="parameters">The parameters.</param>
        private static void LogRawMessage(LogLevel type, string message, Exception exception, params object[] parameters)
        {
            LogLocation logLocation = FileLocationStack.Count > 0 ? FileLocationStack.Peek() : null;

            if(LoggerOutput == null)
            {
                Console.WriteLine("Warning, unable to log error. No LoggerOutput configured");
            }
            else
            {
                LoggerOutput.Log(type, logLocation, ContextAsText, message, exception, parameters);
            }
        }

        #region Delegates

        /// <summary>
        ///     Public delegate for an execution inside a log context.
        /// </summary>
        public delegate void MethodLogInContext();

        #endregion

    }

}
