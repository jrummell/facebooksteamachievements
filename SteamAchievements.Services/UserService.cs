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
using System.Linq;
using SteamAchievements.Data;

namespace SteamAchievements.Services
{
    [ServiceErrorBehaviour(typeof (HttpErrorHandler))]
    public class UserService : IUserService
    {
        private readonly IAchievementManager _manager;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="manager">The manager.</param>
        public UserService(IAchievementManager manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException("manager");
            }

            _manager = manager;
        }

        #region IUserService Members

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="facebookUserId">The facebook user id.</param>
        /// <returns></returns>
        public User GetUser(long facebookUserId)
        {
            Data.User user = _manager.GetUser(facebookUserId);

            return Map(user);
        }

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <returns></returns>
        public User GetUser(string steamUserId)
        {
            Data.User user = _manager.GetUser(steamUserId);

            return Map(user);
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="user">The user.</param>
        public void UpdateUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            bool duplicate = _manager.IsDuplicate(user.SteamUserId, user.FacebookUserId);
            if (duplicate)
            {
                throw new DuplicateSteamUserException();
            }

            _manager.UpdateUser(Map(user));
        }

        /// <summary>
        /// Deauthorizes the user.
        /// </summary>
        /// <param name="facebookUserId">The facebook user id.</param>
        public void DeauthorizeUser(long facebookUserId)
        {
            _manager.DeauthorizeUser(facebookUserId);
        }

        /// <summary>
        /// Gets the auto update users.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetAutoUpdateUsers()
        {
            return _manager.GetAutoUpdateUsers().Select(user => user.SteamUserId);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        #endregion

        /// <summary>
        /// Maps the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        private static User Map(Data.User user)
        {
            if (user == null)
            {
                return null;
            }

            return new User
                       {
                           AccessToken = user.AccessToken,
                           AutoUpdate = user.AutoUpdate,
                           FacebookUserId = user.FacebookUserId,
                           SteamUserId = user.SteamUserId,
                           PublishDescription = user.PublishDescription
                       };
        }

        /// <summary>
        /// Maps the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        private static Data.User Map(User user)
        {
            if (user == null)
            {
                return null;
            }

            return new Data.User
                       {
                           AccessToken = user.AccessToken,
                           AutoUpdate = user.AutoUpdate,
                           FacebookUserId = user.FacebookUserId,
                           SteamUserId = user.SteamUserId,
                           PublishDescription = user.PublishDescription
                       };
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            lock (this)
            {
                if (disposing)
                {
                    _manager.Dispose();
                }
            }
        }
    }
}