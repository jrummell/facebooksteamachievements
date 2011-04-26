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
using System.ServiceModel;

namespace SteamAchievements.Services
{
    [ServiceContract(Namespace = "www.jrummell.com/SteamAchievements")]
    public interface IUserService : IDisposable
    {
        /// <summary>
        /// Gets the auto update users.
        /// </summary>
        [OperationContract]
        ICollection<string> GetAutoUpdateUsers();

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="facebookUserId">The facebook user id.</param>
        [OperationContract]
        User GetUser(long facebookUserId);

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        [OperationContract]
        User GetUser(string steamUserId);

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="user">The user.</param>
        [OperationContract]
        void UpdateUser(User user);

        /// <summary>
        /// Deauthorizes the user.
        /// </summary>
        /// <param name="facebookUserId">The facebook user id.</param>
        [OperationContract]
        void DeauthorizeUser(long facebookUserId);
    }
}