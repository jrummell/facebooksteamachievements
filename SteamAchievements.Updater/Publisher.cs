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
using SteamAchievements.Services;

namespace SteamAchievements.Updater
{
    public class Publisher : IDisposable
    {
        private readonly IAutoUpdateManager _autoUpdateManager;

        public Publisher(IAutoUpdateManager autoUpdateManager)
        {
            _autoUpdateManager = autoUpdateManager;
        }

        #region IDisposable Members

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        #endregion

        public void Publish()
        {
            ICollection<string> users = _autoUpdateManager.GetAutoUpdateUsers();

            foreach (string user in users)
            {
                try
                {
                    _autoUpdateManager.PublishUserAchievements(user);
                }
                catch (Exception ex)
                {
                    _autoUpdateManager.Logger.Log(ex);
                    continue;
                }
            }

            _autoUpdateManager.Logger.Log("All users published.");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _autoUpdateManager.Dispose();
            }
        }
    }
}