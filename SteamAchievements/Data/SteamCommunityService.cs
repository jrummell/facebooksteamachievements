using System;
using System.Net;
using System.Text.RegularExpressions;
using SteamAchievements.Properties;

namespace SteamAchievements.Data
{
    public class SteamCommunityService
    {
        public AchievementCollection GetAchievements(string steamUserId)
        {
            SteamDataContext context = new SteamDataContext();
            AchievementCollection achievements = new AchievementCollection();

            foreach (Game game in context.Games)
            {
                string statsUrl = String.Format("http://steamcommunity.com/id/{0}/stats/{1}?tab=achievements", steamUserId,
                                                game);
                string html = GetStatsHtml(statsUrl);

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