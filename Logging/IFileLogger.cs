namespace ecommerce_api.Logging
{
    /// <summary>
    /// Interface for file-based error logging
    /// </summary>
    public interface IFileLogger
    {
        /// <summary>
        /// Logs an error with exception details to a daily text file
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="exception">Exception object containing stack trace</param>
        void LogError(string message, Exception exception);
    }
}
