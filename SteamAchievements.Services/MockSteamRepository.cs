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
using System.Data.Linq;
using System.Linq;

namespace SteamAchievements.Services
{
    public class MockSteamRepository : Data.ISteamRepository
    {
        private readonly Dictionary<int, Data.Achievement> _achievements;
        private readonly Dictionary<int, Data.UserAchievement> _userAchievements;
        private readonly Dictionary<long, Data.User> _users;
        private readonly Dictionary<int, Data.AchievementName> _achievementNames;

        public MockSteamRepository()
        {
            _achievementNames = new Dictionary<int, Data.AchievementName>
                                    {
                                        {1, new Data.AchievementName
                                                                {
                                                                    Id = 1,
                                                                    AchievementId = 1,
                                                                    Name = "FRIED PIPER",
                                                                    Description =
                                                                        "Use a Molotov to burn a Clown leading at least 10 Common Infected."
                                                                }}
                                    };
            _achievements = new Dictionary<int, Data.Achievement>
                                {
                                    {
                                        1,
                                        new Data.Achievement
                                            {
                                                Id = 1,
                                                ApiName = "ach_incendiary_clown_posse",
                                                GameId = 550,
                                                ImageUrl =
                                                    "http://media.steampowered.com/steamcommunity/public/images/apps/550/8a1dbb0d78c8e288ed5ce990a20454073d01ba9b.jpg",
                                                AchievementNames =
                                                    new EntitySet<Data.AchievementName>
                                                        {
                                                            new Data.AchievementName
                                                                {
                                                                    Id = 1,
                                                                    AchievementId = 1,
                                                                    Name = "FRIED PIPER",
                                                                    Description =
                                                                        "Use a Molotov to burn a Clown leading at least 10 Common Infected."
                                                                }
                                                        }
                                            }
                                        }
                                };
            _users = new Dictionary<long, Data.User>
                         {
                             {
                                 1234567890,
                                 new Data.User
                                     {
                                         AccessToken = "",
                                         AutoUpdate = true,
                                         FacebookUserId = 1234567890,
                                         SteamUserId = "NullReference",
                                         PublishDescription = true
                                     }
                                 }
                         };
            _userAchievements = new Dictionary<int, Data.UserAchievement>
                                    {
                                        {
                                            1,
                                            new Data.UserAchievement
                                                {
                                                    Achievement = _achievements.Values.First(),
                                                    AchievementId = _achievements.Keys.First(),
                                                    FacebookUserId = _users.Keys.First(),
                                                    Date = DateTime.Now.AddDays(-1),
                                                    Id = 1,
                                                    User = _users.Values.First(),
                                                    Hidden = false,
                                                    Published = false
                                                }
                                            }
                                    };
        }

        #region ISteamRepository Members

        public void Dispose()
        {
        }

        public IQueryable<Data.Achievement> Achievements
        {
            get { return _achievements.Values.AsQueryable(); }
        }

        public IQueryable<Data.UserAchievement> UserAchievements
        {
            get { return _userAchievements.Values.AsQueryable(); }
        }

        public IQueryable<Data.User> Users
        {
            get { return _users.Values.AsQueryable(); }
        }

        public IQueryable<Data.AchievementName> AchievementNames
        {
            get { return _achievementNames.Values.AsQueryable(); }
        }

        public void InsertOnSubmit(Data.User user)
        {
            _users.Add(user.FacebookUserId, user);
        }

        public void DeleteOnSubmit(Data.User user)
        {
            _users.Remove(user.FacebookUserId);
        }

        public void InsertOnSubmit(Data.Achievement achievement)
        {
            achievement.Id = _achievements.Keys.Max() + 1;
            _achievements.Add(achievement.Id, achievement);
        }

        public void InsertAllOnSubmit(IEnumerable<Data.UserAchievement> achievements)
        {
            foreach (Data.UserAchievement userAchievement in achievements)
            {
            	userAchievement.Id = _userAchievements.Any() ? (_userAchievements.Keys.Max() + 1) : 1;
                if (userAchievement.Achievement == null && _achievements.ContainsKey(userAchievement.AchievementId))
                {
                    userAchievement.Achievement = _achievements[userAchievement.AchievementId];
                }

                _userAchievements.Add(userAchievement.Id, userAchievement);
            }
        }

        public void DeleteAllOnSubmit(IEnumerable<Data.UserAchievement> achievements)
        {
            foreach (Data.UserAchievement userAchievement in achievements.ToArray())
            {
                _userAchievements.Remove(userAchievement.Id);
            }
        }

        public void InsertOnSubmit(Data.AchievementName achievementName)
        {
            achievementName.Id = _achievementNames.Keys.Max() + 1;
            _achievementNames.Add(achievementName.Id, achievementName);
        }

        public void SubmitChanges()
        {
        }

        #endregion
    }
}