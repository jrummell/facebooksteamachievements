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
using System.Data.SqlClient;
using SteamAchievements.Data;
using SteamAchievements.Services;

namespace SteamAchievements.Updater
{
    public class Publisher : Disposable
    {
        private readonly IAutoUpdateManager _autoUpdateManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="Publisher"/> class.
        /// </summary>
        /// <param name="autoUpdateManager">The auto update manager.</param>
        public Publisher(IAutoUpdateManager autoUpdateManager)
        {
            _autoUpdateManager = autoUpdateManager;
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        public IAutoUpdateLogger Logger
        {
            get { return _autoUpdateManager.Logger; }
        }

        /// <summary>
        /// Publishes this instance.
        /// </summary>
        public void Publish()
        {
            IEnumerable<string> users = _autoUpdateManager.GetAutoUpdateUsers();

            foreach (string user in users)
            {
                try
                {
                    _autoUpdateManager.PublishUserAchievements(user);
                }
                catch (SqlException ex)
                {
                    // a sql exception usually insn't recoverable, so return
                    _autoUpdateManager.Logger.Log(ex);
                    return;
                }
                catch (Exception ex)
                {
                    // log any other errors and continue to the next user
                    _autoUpdateManager.Logger.Log(ex);
                    continue;
                }
            }

            _autoUpdateManager.Logger.Log("All users published.");
        }

        /// <summary>
        /// Disposes the managed resources.
        /// </summary>
        protected override void DisposeManaged()
        {
            _autoUpdateManager.Dispose();
        }
    }
}