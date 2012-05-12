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

using SteamAchievements.Services.Models;
using System.Collections.Generic;

namespace SteamAchievements.Services
{
    public interface IFacebookClientService
    {
        /// <summary>
        /// Get's the facebook user id.
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        long GetUserId(string accessToken);

        /// <summary>
        /// Publishes a post to the user's profile.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="parameters">The post parameters.</param>
        void Publish(User user, IDictionary<string, object> parameters);

        /// <summary>
        /// Parses the signed request.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        object ParseSignedRequest(string signedRequest);
    }
}