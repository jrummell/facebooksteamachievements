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
using SteamAchievements.Data;

namespace SteamAchievements.Services
{
    public class MockSteamRepository : ISteamRepository
    {
        private readonly Dictionary<int, Achievement> _achievements;
        private readonly Dictionary<int, Data.UserAchievement> _userAchievements;
        private readonly Dictionary<long, Data.User> _users;

        public MockSteamRepository()
        {
            _achievements = new Dictionary<int, Achievement>
                                {
                                    {
                                        1,
                                        new Achievement
                                            {
                                                Id = 1,
                                                Description = "Achievement 1 Description",
                                                GameId = 220,
                                                Name = "Achievement 1 Name",
                                                ImageUrl =
                                                    "http://media.steampowered.com/steamcommunity/public/images/apps/220/hl2_escape_apartmentraid.jpg"
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

        public IQueryable<Achievement> Achievements
        {
            get { return _achievements.Values.AsQueryable(); }
            set { throw new NotSupportedException(); }
        }

        public IQueryable<Data.UserAchievement> UserAchievements
        {
            get { return _userAchievements.Values.AsQueryable(); }
            set { throw new NotSupportedException(); }
        }

        public IQueryable<Data.User> Users
        {
            get { return _users.Values.AsQueryable(); }
            set { throw new NotSupportedException(); }
        }

        public void InsertOnSubmit(Data.User user)
        {
            _users.Add(user.FacebookUserId, user);
        }

        public void DeleteAllOnSubmit(IEnumerable<Data.UserAchievement> achievements)
        {
            foreach (Data.UserAchievement userAchievement in achievements.ToArray())
            {
                _userAchievements.Remove(userAchievement.Id);
            }
        }

        public void DeleteOnSubmit(Data.User user)
        {
            _users.Remove(user.FacebookUserId);
        }

        public void SubmitChanges()
        {
        }

        public void InsertOnSubmit(Achievement achievement)
        {
            achievement.Id = _achievements.Keys.Max() + 1;
            _achievements.Add(achievement.Id, achievement);
        }

        public void InsertAllOnSubmit(IEnumerable<Data.UserAchievement> achievements)
        {
            foreach (Data.UserAchievement userAchievement in achievements)
            {
                userAchievement.Id = _userAchievements.Keys.Max() + 1;
                _userAchievements.Add(userAchievement.Id, userAchievement);
            }
        }

        #endregion
    }
}