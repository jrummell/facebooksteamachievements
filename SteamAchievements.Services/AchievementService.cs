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
        /// <returns>
        /// All <see cref="Achievement"/>s for the given user and game.
        /// </returns>
        public List<Achievement> GetAchievements(SteamUserIdGameIdParameter json)
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
        /// <returns>All <see cref="Game"/>s.</returns>
        public List<Game> GetGames()
        {
            return _service.GetGames().ToList();
        }

        /// <summary>
        /// Updates the achievements.
        /// </summary>
        /// <param name="json">The json object.</param>
        /// <returns>true if successful, else false.</returns>
        /// <remarks>jQuery/WCF requires a return value in order for jQuery to execute $.ajax.success.</remarks>
        public bool UpdateAchievements(SteamUserIdParameter json)
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
        /// <returns>true if successful, else false.</returns>
        /// <remarks>jQuery/WCF requires a return value in order for jQuery to execute $.ajax.success.</remarks>
        public bool UpdateSteamUserId(SteamUserIdFacebookUserIdParameter json)
        {
            if (json == null)
            {
                throw new ArgumentNullException("json");
            }

            _service.UpdateSteamUserId(json.FacebookUserId, json.SteamUserId);

            return true;
        }

        /// <summary>
        /// Publishes the last 5 achievements added within the last 5 minutes for the given user.
        /// </summary>
        /// <param name="json">The json object.</param>
        /// <returns>true if successful, else false.</returns>
        public bool PublishLatestAchievements(SteamUserIdFacebookUserIdParameter json)
        {
            if (json == null)
            {
                throw new ArgumentNullException("json");
            }

            IEnumerable<Achievement> achievements = _service.GetLatestAchievements(json.SteamUserId);

            if (achievements.Any())
            {
                AchievementsPublisher publisher = new AchievementsPublisher();
                publisher.Publish(achievements, json.SteamUserId, json.FacebookUserId);
            }

            return true;
        }

        #endregion
    }
}