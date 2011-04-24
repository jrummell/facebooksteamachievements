#region License

// Copyright 2010 John Rummell
// 
// This file is part of SteamAchievements.
// 
//     SteamAchievements is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     SteamAchievements is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with SteamAchievements.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.IO;
using System.Text;
using System.Web;
using Microsoft.Practices.Unity;

namespace SteamAchievements.Services
{
    public class AutoUpdateLogger : IAutoUpdateLogger
    {
        private static readonly object _logLock = new object();
        private readonly StringBuilder _log = new StringBuilder();
        private readonly string _logPath;
        private string _logFilePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoUpdateLogger"/> class.
        /// </summary>
        [InjectionConstructor]
        public AutoUpdateLogger()
        {
            _logPath = HttpContext.Current.Server.MapPath("~/App_Data/AutoUpdate");

            Init();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoUpdateLogger"/> class.
        /// </summary>
        /// <param name="logPath">The log path.</param>
        public AutoUpdateLogger(string logPath)
        {
            _logPath = logPath;

            Init();
        }

        #region IAutoUpdateLogger Members

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
        /// <param name="exception">The exception.</param>
        public void Log(Exception exception)
        {
            Log("Exception: " + exception.GetType());
            Log(exception.Message);
            Log(exception.StackTrace);

            if (exception.InnerException != null)
            {
                Log("Inner Exception: " + exception.InnerException.GetType());
                Log(exception.InnerException.Message);
                Log(exception.InnerException.StackTrace);
            }
        }

        /// <summary>
        /// Flushes the log.
        /// </summary>
        public void Flush()
        {
            lock (_logLock)
            {
                File.AppendAllText(_logFilePath, _log.ToString());
                _log.Clear();
            }
        }

        /// <summary>
        /// Deletes all log files older than the given date.
        /// </summary>
        /// <param name="date">The oldest date to keep.</param>
        public void Delete(DateTime date)
        {
            lock (_logLock)
            {
                string[] files = Directory.GetFiles(_logPath, "*.log");

                foreach (string file in files)
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    string[] dateParts = fileName.Split('-');
                    DateTime fileDate = new DateTime(Convert.ToInt32(dateParts[0]), Convert.ToInt32(dateParts[1]),
                                                     Convert.ToInt32(dateParts[2]));

                    if (fileDate < date)
                    {
                        File.Delete(file);
                    }
                }
            }
        }

        #endregion

        private void Init()
        {
            DateTime now = DateTime.UtcNow;
            string logFileName = String.Format("{0}-{1}-{2}.log", now.Year, now.Month, now.Day);
            _logFilePath = Path.Combine(_logPath, logFileName);
        }
    }
}