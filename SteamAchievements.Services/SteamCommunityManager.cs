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
    public class SteamCommunityManager : IDisposable
    {
        private readonly WebClient _webClient = new WebClient();
        private readonly AchievementXmlParser _parser = new AchievementXmlParser();

        /// <summary>
        /// Gets the closed achievements from http://steamcommunity.com/id/[customurl]/stats/[game]/?xml=1.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <param name="games">The games.</param>
        /// <returns></returns>
        public IEnumerable<Achievement> GetAchievements(string steamUserId, IEnumerable<Game> games)
        {
            if (steamUserId == null)
            {
                throw new ArgumentNullException("steamUserId");
            }

            if (games == null)
            {
                throw new ArgumentNullException("games");
            }

            List<Achievement> achievements = new List<Achievement>();

            foreach (Game game in games)
            {
                int gameId = game.Id;
                string statsUrl = GetStatsUrl(steamUserId, game.Abbreviation);

                string xml = _webClient.DownloadString(statsUrl);

                IEnumerable<Achievement> gameAchievements = _parser.Parse(xml, gameId, true);
                achievements.AddRange(gameAchievements);
            }

            return achievements;
        }

        /// <summary>
        /// Gets all achievements from http://steamcommunity.com/id/[customurl]/stats/[game]/?xml=1.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <param name="game">The game.</param>
        /// <returns></returns>
        public IEnumerable<Achievement> GetAchievements(string steamUserId, Game game)
        {
            if (steamUserId == null)
            {
                throw new ArgumentNullException("steamUserId");
            }

            if (game == null)
            {
                throw new ArgumentException("game");
            }

            string statsUrl = GetStatsUrl(steamUserId, game.Abbreviation);

            string xml = _webClient.DownloadString(statsUrl);

            return _parser.Parse(xml, game.Id, false);
        }

        private static string GetStatsUrl(string steamUserId, string gameAbbreviation)
        {
            return String.Format("http://steamcommunity.com/id/{0}/stats/{1}/?xml=1",
                                                steamUserId, gameAbbreviation);
        }

        #region IDisposable Members

        public void Dispose()
        {
            _webClient.Dispose();
        }

        #endregion
    }
}