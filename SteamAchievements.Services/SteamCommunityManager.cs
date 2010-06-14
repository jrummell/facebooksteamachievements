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
using System.Net;
using SteamAchievements.Data;
using System.Xml;

namespace SteamAchievements.Services
{
    public class SteamCommunityManager : IDisposable
    {
        private readonly WebClient _webClient = new WebClient();
        private readonly AchievementXmlParser _achievementParser = new AchievementXmlParser();
        private readonly GameXmlParser _gamesParser = new GameXmlParser();

        /// <summary>
        /// Gets the closed achievements from http://steamcommunity.com/id/[customurl]/statsfeed/[game]/?xml=1.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <param name="games">The games.</param>
        /// <returns></returns>
        public IEnumerable<Achievement> GetAchievements(string steamUserId)
        {
            if (steamUserId == null)
            {
                throw new ArgumentNullException("steamUserId");
            }

            List<Achievement> achievements = new List<Achievement>();

            IEnumerable<Game> games = GetGames(steamUserId);
            foreach (Game game in games)
            {
                string statsUrl = game.StatsUrl.ToString();
                int lastIndexOfSlash = statsUrl.LastIndexOf("/");
                string gameAbbreviation = statsUrl.Substring(lastIndexOfSlash + 1);
                string xmlStatsUrl = statsUrl + "/?xml=1";

                string xml = _webClient.DownloadString(xmlStatsUrl);

                IEnumerable<Achievement> gameAchievements;
                try
                {
                    gameAchievements = _achievementParser.ParseClosed(xml);
                }
                catch (XmlException ex)
                {
                    //throw new InvalidOperationException("Invalid xml for " + game.Name + " stats: " + statsUrl, ex);
                    continue;
                }

                if (gameAchievements.Any())
                {
                    List<Achievement> achievementList = gameAchievements.ToList();
                    achievementList.ForEach(a => a.Game = game);

                    achievements.AddRange(achievementList);
                }
            }

            return achievements;
        }

        public IEnumerable<Game> GetGames(string steamUserId)
        {
            if (steamUserId == null)
            {
                throw new ArgumentNullException("steamUserId");
            }

            string gamesUrl = GetGamesUrl(steamUserId);

            string xml = _webClient.DownloadString(gamesUrl);

            return _gamesParser.Parse(xml);
        }

        private static string GetGamesUrl(string steamUserId)
        {
            return String.Format("http://steamcommunity.com/id/{0}/games/?xml=1", steamUserId);
        }

        #region IDisposable Members

        public void Dispose()
        {
            _webClient.Dispose();
        }

        #endregion
    }
}