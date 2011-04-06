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
using System.Web;
using System.Xml;
using Elmah;

namespace SteamAchievements.Services
{
    public class SteamCommunityManager : ISteamCommunityManager
    {
        private readonly AchievementXmlParser _achievementParser = new AchievementXmlParser();
        private readonly SteamProfileXmlParser _profileParser = new SteamProfileXmlParser();
        private readonly GameXmlParser _gamesParser = new GameXmlParser();
        private readonly WebClient _webClient = new WebClient();

        #region ISteamCommunityManager Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _webClient.Dispose();
        }

        /// <summary>
        /// Gets the profile from http://steamcommunity.com/id/[customurl]/?xml=1.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <returns></returns>
        public SteamProfile GetProfile(string steamUserId)
        {
            if (steamUserId == null)
            {
                throw new ArgumentNullException("steamUserId");
            }

            string xml = _webClient.DownloadString(GetProfileUrl(steamUserId, true));

            return _profileParser.Parse(xml).SingleOrDefault();
        }

        /// <summary>
        /// Gets the closed achievements from http://steamcommunity.com/id/[customurl]/[game]/?xml=1.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <returns></returns>
        public ICollection<UserAchievement> GetAchievements(string steamUserId)
        {
            if (steamUserId == null)
            {
                throw new ArgumentNullException("steamUserId");
            }

            List<UserAchievement> achievements = new List<UserAchievement>();

            IEnumerable<Game> games = GetGames(steamUserId);
            foreach (Game game in games.Where(g => g.PlayedRecently))
            {
                string xmlStatsUrl = game.StatsUrl + "/?xml=1";

                string xml = _webClient.DownloadString(xmlStatsUrl);

                IEnumerable<UserAchievement> gameAchievements;
                try
                {
                    gameAchievements = _achievementParser.ParseClosed(xml);
                }
                catch (XmlException ex)
                {
                    // log and ignore invalid achievement xml
                    HttpContext context = HttpContext.Current;
                    if (context != null)
                    {
                        ErrorSignal signal = ErrorSignal.FromContext(context);
                        if (signal != null)
                        {
                            string message = "Invalid xml for " + game.Name + " stats: " + game.StatsUrl;
                            Exception exception = new InvalidOperationException(message, ex);
                            signal.Raise(exception);
                        }
                    }

                    continue;
                }

                if (!gameAchievements.Any())
                {
                    continue;
                }

                List<UserAchievement> achievementList = gameAchievements.ToList();
                Game game1 = game;
                achievementList.ForEach(a => a.Game = game1);

                achievements.AddRange(achievementList);
            }

            return achievements;
        }

        /// <summary>
        /// Gets the games from http://steamcommunity.com/id/[customurl]/games/?xml=1.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <returns></returns>
        public ICollection<Game> GetGames(string steamUserId)
        {
            if (steamUserId == null)
            {
                throw new ArgumentNullException("steamUserId");
            }

            Uri gamesUrl = GetGamesUrl(steamUserId, true);

            string xml = _webClient.DownloadString(gamesUrl);

            return _gamesParser.Parse(xml);
        }

        #endregion

        /// <summary>
        /// Gets the profile URL.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <param name="xml">if set to <c>true</c> gets the xml url.</param>
        /// <returns></returns>
        public static Uri GetProfileUrl(string steamUserId, bool xml)
        {
            string url = string.Format("http://steamcommunity.com/id/{0}{1}",
                                       steamUserId, GetXmlParameter(xml));
            return new Uri(url, UriKind.Absolute);
        }

        /// <summary>
        /// Gets the games URL.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <param name="xml">if set to <c>true</c> gets the xml url.</param>
        /// <returns></returns>
        private static Uri GetGamesUrl(string steamUserId, bool xml)
        {
            string url = String.Format("http://steamcommunity.com/id/{0}/games/{1}",
                                       steamUserId, GetXmlParameter(xml));
            return new Uri(url, UriKind.Absolute);
        }

        private static string GetXmlParameter(bool xml)
        {
            return xml ? "?xml=1" : String.Empty;
        }
    }
}