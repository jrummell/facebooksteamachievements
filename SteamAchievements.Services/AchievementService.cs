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
    public class AchievementService : IAchievementService
    {
        private readonly SteamCommunityManager _communityService = new SteamCommunityManager();
        private readonly AchievementManager _service = new AchievementManager();

        #region IAchievementService Members

        /// <summary>
        /// Gets the achievements.
        /// </summary>
        /// <param name="json">The json object.</param>
        /// <returns></returns>
        public List<Achievement> GetAchievements(GetAchievementsParameter json)
        {
            if (json == null)
            {
                throw new ArgumentNullException("json");
            }

            return _service.GetAchievements(json.SteamUserId, json.GameId).ToList();
        }

        /// <summary>
        /// Gets the games.
        /// </summary>
        /// <returns></returns>
        public List<Game> GetGames()
        {
            return _service.GetGames().ToList();
        }

        /// <summary>
        /// Updates the achievements.
        /// </summary>
        /// <param name="json">The json object.</param>
        /// <returns></returns>
        public bool UpdateAchievements(UpdateAchievementsParameter json)
        {
            if (json == null)
            {
                throw new ArgumentNullException("json");
            }

            IEnumerable<Achievement> achievements = _communityService.GetAchievements(json.SteamUserId);

            _service.UpdateAchievements(json.SteamUserId, achievements);

            return true;
        }

        /// <summary>
        /// Updates the steam user id.
        /// </summary>
        /// <param name="json">The json object.</param>
        /// <returns></returns>
        public bool UpdateSteamUserId(UpdateSteamUserIdParameter json)
        {
            if (json == null)
            {
                throw new ArgumentNullException("json");
            }

            _service.UpdateSteamUserId(json.FacebookUserId, json.SteamUserId);

            return true;
        }

        #endregion
    }
}