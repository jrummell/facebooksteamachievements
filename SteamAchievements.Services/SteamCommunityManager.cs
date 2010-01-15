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
using System.Net;
using SteamAchievements.Data;

namespace SteamAchievements.Services
{
    public class SteamCommunityManager
    {
        private readonly IAchievementManager _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="SteamCommunityManager"/> class.
        /// </summary>
        /// <param name="achievementManager">The achievement manager.</param>
        public SteamCommunityManager(IAchievementManager achievementManager)
        {
            if (achievementManager == null)
            {
                throw new ArgumentNullException("achievementManager");
            }
            _service = achievementManager;
        }

        /// <summary>
        /// Gets the achievements from http://steamcommunity.com/id/[customurl]/stats/[game]/?xml=1.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <returns></returns>
        public IEnumerable<Achievement> GetAchievements(string steamUserId)
        {
            AchievementXmlParser parser = new AchievementXmlParser();
            List<Achievement> achievements = new List<Achievement>();

            IEnumerable<Game> games = _service.GetGames();
            foreach (Game game in games)
            {
                int gameId = game.Id;
                string statsUrl = String.Format("http://steamcommunity.com/id/{0}/stats/{1}/?xml=1",
                                                steamUserId, game.Abbreviation);
                string xml;
                using (WebClient client = new WebClient())
                {
                    xml = client.DownloadString(statsUrl);
                }

                IEnumerable<Achievement> gameAchievements = parser.Parse(xml, gameId);
                achievements.AddRange(gameAchievements);
            }

            return achievements;
        }
    }
}