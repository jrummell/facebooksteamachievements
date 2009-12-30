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
using System.Text.RegularExpressions;
using SteamAchievements.Data;
using SteamAchievements.Services.Properties;

namespace SteamAchievements.Services
{
    public class SteamCommunityManager
    {
        private const RegexOptions _options = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline;

        private readonly Regex _endRegex = new Regex(Settings.Default.AchievementSeparatorRegex, _options);
        private readonly Regex _imageRegex = new Regex(Settings.Default.AchievementImageRegex, _options);
        private readonly AchievementManager _service = new AchievementManager();
        private readonly Regex _textRegex = new Regex(Settings.Default.AchievementTextRegex, _options);

        public IEnumerable<Achievement> GetAchievements(string steamUserId)
        {
            List<Achievement> achievements = new List<Achievement>();

            IEnumerable<Game> games = _service.GetGames();
            foreach (Game game in games)
            {
                string statsUrl = String.Format("http://steamcommunity.com/id/{0}/stats/{1}?tab=achievements",
                                                steamUserId, game.Abbreviation);
                string html = GetStatsHtml(statsUrl);
                if (html == null)
                {
                    continue;
                }

                // achievements the player hasn't earned yet come after the separator expression
                Match endMatch = _endRegex.Match(html);
                if (endMatch.Success)
                {
                    html = html.Substring(0, endMatch.Index);
                }
                else
                {
                    // there are no earned achievements for this game
                    continue;
                }

                MatchCollection textMatches = _textRegex.Matches(html);
                MatchCollection imagesMatches = _imageRegex.Matches(html);

                for (int i = 0; i < textMatches.Count; i++)
                {
                    string name = textMatches[i].Groups["name"].Value;
                    string description = textMatches[i].Groups["description"].Value;
                    string image = imagesMatches[i].Groups["image"].Value;

                    Achievement achievement = new Achievement
                                                  {
                                                      Name = name,
                                                      Description = description,
                                                      ImageUrl = image,
                                                      GameId = game.Id
                                                  };
                    achievements.Add(achievement);
                }
            }

            return achievements;
        }

        /// <summary>
        /// Gets the stats HTML.
        /// </summary>
        /// <param name="statsUrl">The stats URL.</param>
        /// <returns></returns>
        private static string GetStatsHtml(string statsUrl)
        {
            string html;
            using (WebClient client = new WebClient())
            {
                try
                {
                    html = client.DownloadString(statsUrl);
                }
                catch (WebException)
                {
                    return null;
                }

                // remove new lines so that Regex parsing is easier
                html = html.Replace("\r\n", "").Replace("\n", "");
            }
            return html;
        }
    }
}