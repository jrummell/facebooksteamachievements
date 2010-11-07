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
using Facebook.Rest;
using Facebook.Schema;
using SteamAchievements.Services.Properties;

namespace SteamAchievements.Services
{
    public class AchievementsPublisher
    {
        private readonly Api _api;
        private readonly bool _testMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="AchievementsPublisher"/> class.
        /// </summary>
        public AchievementsPublisher()
        {
            _testMode = Settings.Default.TestMode;

            List<Enums.ExtendedPermissions> permissions =
                new List<Enums.ExtendedPermissions> {Enums.ExtendedPermissions.publish_stream};
            _api = FacebookApiFactory.CreateInstance(permissions);
        }

        /// <summary>
        /// Publishes the specified achievements.
        /// </summary>
        /// <param name="achievements">The achievements.</param>
        /// <param name="steamUserId">The steam user id.</param>
        /// <param name="facebookUserId">The facebook user id.</param>
        public void Publish(IEnumerable<Achievement> achievements, string steamUserId, long facebookUserId)
        {
            if (steamUserId == null)
            {
                throw new ArgumentNullException("steamUserId");
            }

            if (_testMode)
            {
                return;
            }

            foreach (Achievement achievement in achievements)
            {
                string description = String.Format("earned the {0} achievement in {1}.", achievement.Name,
                                                   achievement.Game.Name);

                string gameStatsLink = achievement.Game.StatsUrl.ToString();
                string gameAchievementsLink = gameStatsLink + "?tab=achievements";

                attachment attachment = new attachment
                                            {
                                                caption = achievement.Description,
                                                name = achievement.Name,
                                                href = gameAchievementsLink,
                                                media = new List<attachment_media>
                                                            {
                                                                new attachment_media_image
                                                                    {
                                                                        src = achievement.ImageUrl.ToString(),
                                                                        href = gameAchievementsLink
                                                                    }
                                                            }
                                            };

                List<action_link> links = new List<action_link>
                                              {
                                                  new action_link
                                                      {
                                                          text = achievement.Game.Name + " stats",
                                                          href = gameStatsLink
                                                      }
                                              };

                _api.Stream.Publish(description, attachment, links, null, facebookUserId);
            }
        }
    }
}