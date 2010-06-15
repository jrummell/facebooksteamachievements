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
using System.Xml;
using Elmah;

namespace SteamAchievements.Services
{
    public class SteamCommunityManager : IDisposable
    {
        private readonly AchievementXmlParser _achievementParser = new AchievementXmlParser();
        private readonly GameXmlParser _gamesParser = new GameXmlParser();
        private readonly WebClient _webClient = new WebClient();

        #region IDisposable Members

        public void Dispose()
        {
            _webClient.Dispose();
        }

        #endregion

        /// <summary>
        /// Gets the closed achievements from http://steamcommunity.com/id/[customurl]/statsfeed/[game]/?xml=1.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
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
                string xmlStatsUrl = statsUrl + "/?xml=1";

                string xml = _webClient.DownloadString(xmlStatsUrl);

                IEnumerable<Achievement> gameAchievements;
                try
                {
                    gameAchievements = _achievementParser.ParseClosed(xml);
                }
                catch (XmlException ex)
                {
                    ErrorSignal signal = ErrorSignal.FromCurrentContext();
                    if (signal != null)
                    {
                        string message = "Invalid xml for " + game.Name + " stats: " + statsUrl;
                        Exception exception = new InvalidOperationException(message, ex);
                        signal.Raise(exception);
                    }

                    continue;
                }

                if (gameAchievements.Any())
                {
                    List<Achievement> achievementList = gameAchievements.ToList();
                    Game game1 = game;
                    achievementList.ForEach(a => a.Game = game1);

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
    }
}