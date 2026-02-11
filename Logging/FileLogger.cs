using System;
using System.IO;

namespace ecommerce_api.Logging
{
    /// <summary>
    /// File-based logger implementation that writes error logs to daily text files
    /// </summary>
    public class FileLogger : IFileLogger
    {
        private readonly string _logDirectory;
        private readonly object _lock = new object();

        public FileLogger()
        {
            // Set log directory to 'logs' folder in the application root
            _logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
            
            // Create logs directory if it doesn't exist
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
        }

        /// <summary>
        /// Logs an error with timestamp, level, message, and stack trace to a daily text file
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="exception">Exception object containing stack trace</param>
        public void LogError(string message, Exception exception)
        {
            // Thread-safe logging
            lock (_lock)
            {
                try
                {
                    var logFileName = GetDailyLogFileName();
                    var logFilePath = Path.Combine(_logDirectory, logFileName);
                    
                    var logEntry = FormatLogEntry(message, exception);
                    
                    // Append log entry to file
                    File.AppendAllText(logFilePath, logEntry);
                }
                catch (Exception ex)
                {
                    // If logging fails, write to console as fallback
                    Console.Error.WriteLine($"Failed to write to log file: {ex.Message}");
                    Console.Error.WriteLine($"Original error: {message}");
                }
            }
        }

        /// <summary>
        /// Generates the log file name based on current date (UTC)
        /// </summary>
        /// <returns>Log file name in format: error-log-YYYY-MM-DD.txt</returns>
        private string GetDailyLogFileName()
        {
            var date = DateTime.UtcNow.ToString("yyyy-MM-dd");
            return $"error-log-{date}.txt";
        }

        /// <summary>
        /// Formats the log entry with timestamp (UTC), level, message, and stack trace
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="exception">Exception object</param>
        /// <returns>Formatted log entry string</returns>
        private string FormatLogEntry(string message, Exception exception)
        {
            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var separator = new string('-', 80);
            
            var logEntry = $"{separator}\n";
            logEntry += $"Timestamp (UTC): {timestamp}\n";
            logEntry += $"Level: Error\n";
            logEntry += $"Message: {message}\n";
            
            if (exception != null)
            {
                logEntry += $"Exception Type: {exception.GetType().FullName}\n";
                logEntry += $"Exception Message: {exception.Message}\n";
                logEntry += $"Stack Trace:\n{exception.StackTrace}\n";
                
                // Log inner exceptions if present
                if (exception.InnerException != null)
                {
                    logEntry += $"\nInner Exception: {exception.InnerException.GetType().FullName}\n";
                    logEntry += $"Inner Exception Message: {exception.InnerException.Message}\n";
                    logEntry += $"Inner Stack Trace:\n{exception.InnerException.StackTrace}\n";
                }
            }
            
            logEntry += $"{separator}\n\n";
            
            return logEntry;
        }
    }
}
