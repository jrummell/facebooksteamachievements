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
using AutoMapper;
using SteamAchievements.Data;

namespace SteamAchievements.Services
{
    public class UserService : Disposable, IUserService
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
        public Models.User GetUser(long facebookUserId)
        {
            Data.User user = _manager.GetUser(facebookUserId);

            return Map(user);
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="user">The user.</param>
        public void UpdateUser(Models.User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
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
        public ICollection<Models.User> GetAutoUpdateUsers()
        {
            return _manager.GetAutoUpdateUsers().Select(Map).ToArray();
        }

        #endregion

        protected override void DisposeManaged()
        {
            _manager.Dispose();
        }

        /// <summary>
        /// Maps the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        private static Models.User Map(Data.User user)
        {
            if (user == null)
            {
                return null;
            }

            return Mapper.Map<Models.User>(user);
        }

        /// <summary>
        /// Maps the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        private static Data.User Map(Models.User user)
        {
            if (user == null)
            {
                return null;
            }

            return Mapper.Map<Data.User>(user);
        }
    }
    

}