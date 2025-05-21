using Microsoft.Extensions.Logging;
using System.Text;

namespace ecommerce_api.Helpers
{
    public class Logger
    {
        private readonly ILogger<Logger> _logger;
        private readonly Logs _myLogger;
        private readonly StringBuilder _logs;

        public Logger(ILogger<Logger> logger, Logs customLogger)
        {
            _logger = logger;
            _myLogger = customLogger;
            _logs = new StringBuilder();
        }

        public void LogMessage(string message)
        {
            _logs.AppendLine(message);
        }

        public void LogException(string context, Exception ex, string relatedData = "")
        {
            if (!string.IsNullOrEmpty(relatedData))
                _logs.AppendLine($"Exception in {context}: Data - {relatedData}, {ex.Message}");
            else
                _logs.AppendLine($"Exception in {context}: Message - {ex.Message}");
        }

        public void LogFailure(string context, string message, string relatedData = "")
        {
            if (!string.IsNullOrEmpty(relatedData))
                _logs.AppendLine($"Failure in {context}: Data - {relatedData}, {message}");
            else
                _logs.AppendLine($"Failure in {context}: Message - {message}");
        }

        public void LogData(string context, string data)
        {
            _logs.AppendLine($"Data in {context}: {data}");
        }

        public void FlushLogs()
        {
            if (_logs.Length > 0)
            {
                _myLogger.Log(_logs);
                _logs.Clear();
            }
        }
    }

    public class Logs
    {
        private readonly IWebHostEnvironment _env;

        public Logs(IWebHostEnvironment env)
        {
            _env = env;
        }

        public bool Log(StringBuilder contents)
        {
            try
            {
                string currentDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
                string createdTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

                // Use IWebHostEnvironment to get the file path
                string filePath = Path.Combine(_env.ContentRootPath, "Logs");

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                // Log file name - single file for appending logs
                filePath = Path.Combine(filePath, "AppLog.txt");

                // Check if the file exists and if the last modification date is before today
                if (File.Exists(filePath))
                {
                    var lastWriteTime = File.GetLastWriteTime(filePath);

                    // If the file is from a previous day, overwrite it
                    if (lastWriteTime.Date < DateTime.UtcNow.Date)
                    {
                        File.WriteAllText(filePath, $"Log for {currentDate}\n");  // Overwrite with new log for today
                    }
                }
                else
                {
                    // If file doesn't exist, create a new one
                    File.WriteAllText(filePath, $"Log for {currentDate}\n");  // Start the new file with today's log
                }

                // Append log content
                File.AppendAllText(filePath, $"{createdTime}:\t{contents.ToString()}\n");

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
