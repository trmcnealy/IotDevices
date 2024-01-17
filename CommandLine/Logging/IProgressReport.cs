namespace CommandLine.CommandLine.Logging
{

    /// <summary>
    ///     Interface to notify progress.
    /// </summary>
    public interface IProgressReport
    {
        void FatalExit(string message);

        /// <summary>
        ///     Notify progress.
        /// </summary>
        /// <param name="level">The level 0-100.</param>
        /// <param name="message">The message to notify.</param>
        /// <returns>true if the process is aborted; false otherwise</returns>
        bool ProgressStatus(int level, string message);
    }

}
