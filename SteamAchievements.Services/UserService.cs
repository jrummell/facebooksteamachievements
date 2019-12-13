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
using System.Linq;
using AutoMapper;
using SteamAchievements.Data;
using SteamAchievements.Services.Models;

namespace SteamAchievements.Services
{
    public class UserService : Disposable, IUserService
    {
        private readonly IAchievementManager _manager;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService" /> class.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="mapper">The mapper.</param>
        /// <exception cref="ArgumentNullException">manager</exception>
        public UserService(IAchievementManager manager, IMapper mapper)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            _manager = manager;
            _mapper = mapper;
        }

        #region IUserService Members

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public UserModel GetUser(string userId)
        {
            Data.User user = _manager.GetUser(userId);

            return Map(user);
        }

        public UserModel GetByFacebookUserId(long facebookUserId)
        {
            var user = _manager.GetByFacebookUserId(facebookUserId);

            return Map(user);
        }

        public void CreateUser(UserModel user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            _manager.CreateUser(Map(user));
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="user">The user.</param>
        public void UpdateUser(Models.UserModel user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            _manager.UpdateUser(Map(user));
        }

        /// <summary>
        /// Deauthorizes the user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        public void DeauthorizeUser(string userId)
        {
            _manager.DeauthorizeUser(userId);
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
        private Models.UserModel Map(Data.User user)
        {
            if (user == null)
            {
                return null;
            }

            return _mapper.Map<Models.UserModel>(user);
        }

        /// <summary>
        /// Maps the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        private Data.User Map(Models.UserModel user)
        {
            if (user == null)
            {
                return null;
            }

            return _mapper.Map<Data.User>(user);
        }
    }
    

}