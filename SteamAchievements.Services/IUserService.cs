﻿#region License

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
using SteamAchievements.Services.Models;

namespace SteamAchievements.Services
{
    public interface IUserService : IDisposable
    {
        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        UserModel GetUser(string userId);

        /// <summary>
        /// Gets the user by facebook user identifier.
        /// </summary>
        /// <param name="facebookUserId">The facebook user identifier.</param>
        /// <returns></returns>
        UserModel GetByFacebookUserId(long facebookUserId);

        /// <summary>
        /// Changes the steam user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="steamUserId">The steam user identifier.</param>
        void ChangeSteamUserId(string userId, string steamUserId);

        /// <summary>
        /// Deauthorizes the user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        void DeauthorizeUser(string userId);
    }
}