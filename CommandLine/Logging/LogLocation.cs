namespace CommandLine.CommandLine.Logging
{

    /// <summary>
    ///     Source code location of a logging message.
    /// </summary>
    //[System.Runtime.Versioning.NonVersionable]
    public class LogLocation
    {
        /// <summary>
        ///     Gets the file location.
        /// </summary>
        /// <value>The file location.</value>
        public string File { get; }

        /// <summary>
        ///     Gets the line inside the file.
        /// </summary>
        /// <value>The line.</value>
        public int Line { get; }

        /// <summary>
        ///     Gets the column inside the line of the file.
        /// </summary>
        /// <value>The column.</value>
        public int Column { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogLocation" /> class.
        /// </summary>
        /// <param name="filePath">The file location.</param>
        public LogLocation(string filePath)
            : this(filePath, 1) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogLocation" /> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="line">The line.</param>
        public LogLocation(string filePath, int line)
            : this(filePath, line, 1) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogLocation" /> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="line">The line.</param>
        /// <param name="column">The column.</param>
        public LogLocation(string filePath, int line, int column)
        {
            File   = filePath;
            Line   = line;
            Column = column;
        }
    }

}
