using System;
using System.IO;
using System.Text;
using System.Web;

namespace SteamAchievements.Services
{
    public class AutoUpdateLogger
    {
        private readonly StringBuilder _log = new StringBuilder();
        private readonly string _logFilePath;
        private static readonly object _logLock = new object();

        public AutoUpdateLogger(string logPath)
        {
            DateTime now = DateTime.UtcNow;
            string logFileName = String.Format("{0}-{1}-{2}.log", now.Year, now.Month, now.Day);
            _logFilePath = Path.Combine(_logFilePath, logFileName);
        }

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Log(string message)
        {
            _log.AppendFormat("{0} {1}{2}", DateTime.UtcNow, message, Environment.NewLine);
        }

        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="ex">The ex.</param>
        public void Log(Exception ex)
        {
            Log("Exception: " + ex.GetType());
            Log(ex.Message);
            Log(ex.StackTrace);

            if (ex.InnerException != null)
            {
                Log("Inner Exception: " + ex.InnerException.GetType());
                Log(ex.InnerException.Message);
                Log(ex.InnerException.StackTrace);
            }
        }

        /// <summary>
        /// Flushes the log.
        /// </summary>
        public void FlushLog()
        {
            lock (_logLock)
            {
                File.AppendAllText(_logFilePath, _log.ToString());
                _log.Clear();
            }
        }

        /// <summary>
        /// Writes the log to the response.
        /// </summary>
        /// <param name="response"></param>
        public void WriteLog(HttpResponse response)
        {
            response.Write(_log.ToString());
        }
    }
}
