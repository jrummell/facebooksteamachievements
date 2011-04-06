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
using System.Collections.Generic;

namespace SteamAchievements.Services
{
    public interface ISteamCommunityManager : IDisposable
    {
        /// <summary>
        /// Gets the closed achievements from http://steamcommunity.com/id/[customurl]/statsfeed/[game]/?xml=1.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <returns></returns>
        ICollection<UserAchievement> GetAchievements(string steamUserId);

        /// <summary>
        /// Gets the games from http://steamcommunity.com/id/[customurl]/games/?xml=1.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <returns></returns>
        ICollection<Game> GetGames(string steamUserId);

        /// <summary>
        /// Gets the profile.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <returns></returns>
        SteamProfile GetProfile(string steamUserId);
    }
}