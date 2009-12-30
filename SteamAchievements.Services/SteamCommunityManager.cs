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
        private readonly AchievementManager _service = new AchievementManager();

        public IEnumerable<Achievement> GetAchievements(string steamUserId)
        {
            List<Achievement> achievements = new List<Achievement>();

            foreach (Game game in _service.GetGames())
            {
                string statsUrl = String.Format("http://steamcommunity.com/id/{0}/stats/{1}?tab=achievements",
                                                steamUserId,
                                                game.Abbreviation);
                string html = GetStatsHtml(statsUrl);

                // achievements the player hasn't earned yet come after 3 br tags
                int index = html.IndexOf("<br /><br /><br />");
                if (index > 0)
                {
                    html = html.Substring(0, index);
                }

                const RegexOptions options = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline;
                Regex textRegex = new Regex(Settings.Default.AchievementTextRegex, options);
                Regex imageRegex = new Regex(Settings.Default.AchievementImageRegex, options);

                MatchCollection textMatches = textRegex.Matches(html);
                MatchCollection imagesMatches = imageRegex.Matches(html);

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

        private static string GetStatsHtml(string statsUrl)
        {
            string html;
            using (WebClient client = new WebClient())
            {
                html = client.DownloadString(statsUrl);
                html = html.Replace("\r\n", "").Replace("\n", "");
            }
            return html;
        }
    }
}