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
using System.Collections.Generic;
using System.IO;
using SteamAchievements.Data;

namespace SteamAchievements.Updater
{
    public class AutoUpdateLogger : Disposable, IAutoUpdateLogger
    {
        private static readonly object _logLock = new object();
        private readonly string _logPath;
        private readonly List<TextWriter> _textWriters = new List<TextWriter>();
        private string _logFilePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoUpdateLogger"/> class.
        /// </summary>
        /// <param name="logPath">The log path.</param>
        public AutoUpdateLogger(string logPath)
        {
            _logPath = logPath;

            InitDefaultWriter();
        }

        #region IAutoUpdateLogger Members

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Log(string message)
        {
            string formattedMessage = String.Format("{0} {1}", DateTime.UtcNow, message);

            foreach (TextWriter writer in _textWriters)
            {
                writer.WriteLine(formattedMessage);
            }
        }

        /// <summary>
        /// Logs the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void Log(string format, params object[] args)
        {
            Log(String.Format(format, args));
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
                foreach (TextWriter writer in _textWriters)
                {
                    writer.Flush();
                }
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

        /// <summary>
        /// Attaches the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void Attach(TextWriter writer)
        {
            _textWriters.Add(writer);
        }

        #endregion

        /// <summary>
        /// Inits the default writer.
        /// </summary>
        private void InitDefaultWriter()
        {
            DirectoryInfo logDirectory = new DirectoryInfo(_logPath);
            if (!logDirectory.Exists)
            {
                logDirectory.Create();
            }

            DateTime now = DateTime.UtcNow;
            string logFileName = String.Format("{0}-{1}-{2}.log", now.Year, now.Month, now.Day);
            _logFilePath = Path.Combine(_logPath, logFileName);

            StreamWriter writer = new StreamWriter(_logFilePath, true);
            Attach(writer);
        }

        /// <summary>
        /// Disposes the managed resources.
        /// </summary>
        protected override void DisposeManaged()
        {
            foreach (TextWriter writer in _textWriters)
            {
                writer.Flush();
                writer.Dispose();
            }
        }
    }
}