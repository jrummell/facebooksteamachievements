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
using System.Web;

namespace SteamAchievements.Services
{
    public interface IAutoUpdateLogger
    {
        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Log(string message);

        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        void Log(Exception exception);

        /// <summary>
        /// Flushes the log.
        /// </summary>
        void Flush();

        /// <summary>
        /// Writes the log to the response.
        /// </summary>
        /// <param name="response">The response.</param>
        void Write(HttpResponse response);

        /// <summary>
        /// Deletes all log files older than the given date.
        /// </summary>
        /// <param name="date">The oldest date to keep.</param>
        void Delete(DateTime date);
    }
}