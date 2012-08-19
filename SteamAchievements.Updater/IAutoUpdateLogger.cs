#region License

//  Copyright 2011 John Rummell
//  
//  This file is part of SteamAchievements.
//  
//      SteamAchievements is free software: you can redistribute it and/or modify
//      it under the terms of the GNU General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      SteamAchievements is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU General Public License for more details.
//  
//      You should have received a copy of the GNU General Public License
//      along with SteamAchievements.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.IO;
using SteamAchievements.Services;

namespace SteamAchievements.Updater
{
    public interface IAutoUpdateLogger : IErrorLogger, IDisposable
    {
        /// <summary>
        ///   Logs the specified format.
        /// </summary>
        /// <param name="format"> The format. </param>
        /// <param name="args"> The args. </param>
        void Log(string format, params object[] args);

        /// <summary>
        ///   Flushes the log.
        /// </summary>
        void Flush();

        /// <summary>
        ///   Deletes all log files older than the given date.
        /// </summary>
        /// <param name="date"> The oldest date to keep. </param>
        void Delete(DateTime date);

        /// <summary>
        ///   Attaches the specified writer.
        /// </summary>
        /// <param name="writer"> The writer. </param>
        void Attach(TextWriter writer);
    }
}