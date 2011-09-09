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
using System.Diagnostics;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Xml;
using Elmah;
using SteamAchievements.Data;

namespace SteamAchievements.Services
{
    public class SteamCommunityManager : Disposable, ISteamCommunityManager
    {
        private static readonly object _achievementCacheLock = new object();
        private readonly IAchievementXmlParser _achievementParser;
        private readonly IGameXmlParser _gamesParser;
        private readonly ISteamProfileXmlParser _profileParser;
        private readonly IWebClientWrapper _webClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="SteamCommunityManager"/> class.
        /// </summary>
        /// <param name="webClient">The web client.</param>
        /// <param name="profileParser">The profile parser.</param>
        /// <param name="gamesParser">The games parser.</param>
        /// <param name="achievementParser">The achievement parser.</param>
        public SteamCommunityManager(IWebClientWrapper webClient, ISteamProfileXmlParser profileParser,
                                     IGameXmlParser gamesParser, IAchievementXmlParser achievementParser)
        {
            _webClient = webClient;
            _achievementParser = achievementParser;
            _gamesParser = gamesParser;
            _profileParser = profileParser;
        }

        private static IDictionary<int, Achievement> AchievementCache
        {
            get
            {
                lock (_achievementCacheLock)
                {
                    string key = typeof (SteamCommunityManager) + ".AchievementCache";
                    MemoryCache cache = MemoryCache.Default;
                    IDictionary<int, Achievement> dictionary =
                        cache.Get(key) as IDictionary<int, Achievement>;

                    if (dictionary == null)
                    {
                        dictionary = new Dictionary<int, Achievement>();
                        cache.Add(key, dictionary, DateTime.Now.AddDays(1));
                    }

                    return dictionary;
                }
            }
        }

        #region ISteamCommunityManager Members

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

            if (xml == null)
            {
                return null;
            }

            return _profileParser.Parse(xml).SingleOrDefault();
        }

        /// <summary>
        /// Gets the closed achievements from http://steamcommunity.com/id/[customurl]/[game]/?xml=1.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <returns></returns>
        public ICollection<UserAchievement> GetClosedAchievements(string steamUserId)
        {
            return GetAchievements(steamUserId, true);
        }

        /// <summary>
        /// Gets the achievements from http://steamcommunity.com/id/[customurl]/[game]/?xml=1.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <returns></returns>
        public ICollection<UserAchievement> GetAchievements(string steamUserId)
        {
            return GetAchievements(steamUserId, false);
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
            Debug.WriteLine(gamesUrl);

            string xml = _webClient.DownloadString(gamesUrl);

            if (xml == null)
            {
                return new Game[0];
            }

            try
            {
                return _gamesParser.Parse(xml);
            }
            catch (XmlException ex)
            {
                string message = String.Format("Invalid games URL for {0}.", steamUserId);
                throw new InvalidGamesXmlException(message, ex);
            }
        }

        /// <summary>
        /// Sets Name, Description, and ImageUrl from the achievement cache.
        /// </summary>
        /// <param name="achievements">The achievements.</param>
        public void FillAchievements(IEnumerable<Achievement> achievements)
        {
            foreach (Achievement achievement in achievements)
            {
                int hashCode = achievement.GetHashCode();
                if (AchievementCache.ContainsKey(hashCode))
                {
                    Achievement cachedAchievement = AchievementCache[hashCode];
                    achievement.Name = cachedAchievement.Name;
                    achievement.Description = cachedAchievement.Description;
                    achievement.ImageUrl = cachedAchievement.ImageUrl;
                }
            }
        }

        #endregion

        /// <summary>
        /// Gets the achievements from http://steamcommunity.com/id/[customurl]/[game]/?xml=1.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <param name="closedOnly">if set to <c>true</c> [closed only].</param>
        /// <returns></returns>
        private ICollection<UserAchievement> GetAchievements(string steamUserId, bool closedOnly)
        {
            if (steamUserId == null)
            {
                throw new ArgumentNullException("steamUserId");
            }

            List<UserAchievement> achievements = new List<UserAchievement>();

            IEnumerable<Game> games = GetGames(steamUserId);
            if (closedOnly)
            {
                games = games.Where(g => g.PlayedRecently);
            }
            foreach (Game game in games)
            {
                Uri xmlStatsUrl = new Uri(game.StatsUrl + "/?xml=1", UriKind.Absolute);
                Debug.WriteLine(xmlStatsUrl);

                string xml = _webClient.DownloadString(xmlStatsUrl);

                if (xml == null)
                {
                    continue;
                }

                IEnumerable<UserAchievement> gameAchievements;
                try
                {
                    if (closedOnly)
                    {
                        gameAchievements = _achievementParser.ParseClosed(xml);
                    }
                    else
                    {
                        gameAchievements = _achievementParser.Parse(xml);
                    }
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
                            string message = String.Format("Invalid xml for {0} stats: {1}.", game.Name, game.StatsUrl);
                            Exception exception = new InvalidStatsXmlException(message, ex);
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
                achievementList.ForEach(a => a.Achievement.Game = game1);

                achievements.AddRange(achievementList);
            }

            lock (_achievementCacheLock)
            {
                // cache the achievements
                foreach (Achievement achievement in achievements.Select(a => a.Achievement))
                {
                    int hashCode = achievement.GetHashCode();
                    if (!AchievementCache.ContainsKey(hashCode))
                    {
                        AchievementCache.Add(hashCode, achievement);
                    }
                }
            }

            return achievements;
        }

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

        /// <summary>
        /// Gets the XML parameter.
        /// </summary>
        /// <param name="xml">if set to <c>true</c> [XML].</param>
        /// <returns></returns>
        private static string GetXmlParameter(bool xml)
        {
            return xml ? "?xml=1" : String.Empty;
        }

        protected override void DisposeManaged()
        {
            _webClient.Dispose();
        }
    }
}