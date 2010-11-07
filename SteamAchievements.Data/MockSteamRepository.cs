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

namespace SteamAchievements.Data
{
    /// <summary>
    /// A mock implementation of <see cref="ISteamRepository"/>.
    /// </summary>
    /// <remarks>
    /// Insert/Update/DeleteOnSubmit methods perform the operation immediately. <see cref="SubmitChanges"/> does nothing.
    /// </remarks>
    public class MockSteamRepository : ISteamRepository
    {
        private readonly List<Achievement> _achievementsToInsertOnSubmit = new List<Achievement>();
        private readonly List<UserAchievement> _userAchievementsToDeleteOnSubmit = new List<UserAchievement>();
        private readonly List<UserAchievement> _userAchievementsToInsertOnSubmit = new List<UserAchievement>();
        private readonly List<User> _usersToInsertOnSubmit = new List<User>();
        private Dictionary<int, Achievement> _achievements;
        private Dictionary<int, UserAchievement> _userAchievements;
        private Dictionary<UserKey, User> _users;

        public MockSteamRepository()
        {
            Init();
        }

        #region ISteamRepository Members

        /// <summary>
        /// Gets the achievements.
        /// </summary>
        /// <value>The achievements.</value>
        public IQueryable<Achievement> Achievements
        {
            get { return _achievements.Values.AsQueryable(); }
            set { _achievements = value.ToDictionary(x => x.Id, x => x); }
        }

        /// <summary>
        /// Gets the user achievements.
        /// </summary>
        /// <value>The user achievements.</value>
        public IQueryable<UserAchievement> UserAchievements
        {
            get { return _userAchievements.Values.AsQueryable(); }
            set { _userAchievements = value.ToDictionary(x => x.Id, x => x); }
        }

        /// <summary>
        /// Gets the users.
        /// </summary>
        /// <value>The users.</value>
        public IQueryable<User> Users
        {
            get { return _users.Values.AsQueryable(); }
            set { _users = value.ToDictionary(x => new UserKey(x), x => x); }
        }

        /// <summary>
        /// Inserts the user on submit.
        /// </summary>
        /// <param name="user">The user.</param>
        public void InsertOnSubmit(User user)
        {
            _usersToInsertOnSubmit.Add(user);
        }

        /// <summary>
        /// Deletes all given achievements on submit.
        /// </summary>
        /// <param name="achievements">The achievements.</param>
        public void DeleteAllOnSubmit(IEnumerable<UserAchievement> achievements)
        {
            _userAchievementsToDeleteOnSubmit.AddRange(achievements);
        }

        /// <summary>
        /// Submits the changes.
        /// </summary>
        public void SubmitChanges()
        {
            // UserAchievements
            int maxUserAchievementId = 0;
            if (_userAchievements.Any())
            {
                maxUserAchievementId = _userAchievements.Values.Max(ua => ua.Id);
            }

            foreach (UserAchievement userAchievement in _userAchievementsToInsertOnSubmit)
            {
                userAchievement.Id = ++maxUserAchievementId;
                _userAchievements.Add(userAchievement.Id, userAchievement);
            }
            _userAchievementsToInsertOnSubmit.Clear();

            // Achievements
            int maxAchievementId = 0;
            if (_achievements.Any())
            {
                maxAchievementId = _achievements.Values.Max(a => a.Id);
            }

            foreach (Achievement achievement in _achievementsToInsertOnSubmit)
            {
                // mimic the unique index on GameId, Name, Description
                Achievement achievement1 = achievement;
                if (
                    _achievements.Values.Where(
                        a =>
                        a.Name == achievement1.Name && a.GameId == achievement1.GameId &&
                        a.Description == achievement1.Description).Any())
                {
                    throw new InvalidOperationException("Cannot insert duplicate key row in object 'dbo.steam_Achievement' with unique index 'IX_steam_Achievement.\r\nGameId: "
                                                        + achievement.GameId + " Name: " + achievement.Name +
                                                        " Description: " + achievement.Description);
                }

                achievement.Id = ++maxAchievementId;
                _achievements.Add(achievement.Id, achievement);
            }
            _achievementsToInsertOnSubmit.Clear();

            // Users
            foreach (User user in _usersToInsertOnSubmit)
            {
                _users.Add(new UserKey(user), user);
            }
            _usersToInsertOnSubmit.Clear();

            foreach (UserAchievement userAchievement in _userAchievementsToDeleteOnSubmit)
            {
                _userAchievements.Remove(userAchievement.Id);
            }
            _userAchievementsToDeleteOnSubmit.Clear();
        }

        /// <summary>
        /// Inserts the achievement on submit.
        /// </summary>
        /// <param name="achievement">The achievement.</param>
        public void InsertOnSubmit(Achievement achievement)
        {
            _achievementsToInsertOnSubmit.Add(achievement);
        }

        /// <summary>
        /// Inserts all given achievements on submit.
        /// </summary>
        /// <param name="achievements">The achievements.</param>
        public void InsertAllOnSubmit(IEnumerable<UserAchievement> achievements)
        {
            _userAchievementsToInsertOnSubmit.AddRange(achievements);
        }

        public void Dispose()
        {
            // nothing to dispose
        }

        #endregion

        private void Init()
        {
            IQueryable<Achievement> achievements =
                (new[]
                     {
                         new Achievement
                             {
                                 Id = 1,
                                 GameId = 1,
                                 Name = "Achievement 1 for Game 1",
                                 Description = "Achievement Description",
                                 ImageUrl = "http://example.com/image.png"
                             },
                         new Achievement
                             {
                                 Id = 2,
                                 GameId = 1,
                                 Name = "Achievement 2 for Game 1",
                                 Description = "Achievement Description",
                                 ImageUrl = "http://example.com/image.png"
                             },
                         new Achievement
                             {
                                 Id = 3,
                                 GameId = 1,
                                 Name = "Achievement 3 for Game 1",
                                 Description = "Achievement Description",
                                 ImageUrl = "http://example.com/image.png"
                             },
                         new Achievement
                             {
                                 Id = 4,
                                 GameId = 2,
                                 Name = "Achievement 1 for Game 2",
                                 Description = "Achievement Description",
                                 ImageUrl = "http://example.com/image.png"
                             },
                         new Achievement
                             {
                                 Id = 5,
                                 GameId = 2,
                                 Name = "Achievement 2 for Game 2",
                                 Description = "Achievement Description",
                                 ImageUrl = "http://example.com/image.png"
                             }
                     }).AsQueryable();

            IQueryable<User> users =
                (new[]
                     {
                         new User {FacebookUserId = 1234567890, SteamUserId = "user1"},
                         new User {FacebookUserId = 1234567891, SteamUserId = "user2"}
                     }).AsQueryable();

            IQueryable<UserAchievement> userAchievements =
                (new[]
                     {
                         new UserAchievement
                             {
                                 Id = 1,
                                 AchievementId = 1,
                                 Date = DateTime.Now,
                                 SteamUserId = "user1",
                                 Achievement = achievements.Single(a => a.Id == 1)
                             },
                         new UserAchievement
                             {
                                 Id = 2,
                                 AchievementId = 2,
                                 Date = DateTime.Now,
                                 SteamUserId = "user1",
                                 Achievement = achievements.Single(a => a.Id == 2)
                             },
                         new UserAchievement
                             {
                                 Id = 3,
                                 AchievementId = 3,
                                 Date = DateTime.Now,
                                 SteamUserId = "user1",
                                 Achievement = achievements.Single(a => a.Id == 3)
                             }
                         ,
                         new UserAchievement
                             {
                                 Id = 4,
                                 AchievementId = 4,
                                 Date = DateTime.Now,
                                 SteamUserId = "user1",
                                 Achievement = achievements.Single(a => a.Id == 4)
                             }
                     }).AsQueryable();

            Achievements = achievements;
            UserAchievements = userAchievements;
            Users = users;
        }

        #region Nested type: UserKey

        private class UserKey : IEquatable<UserKey>
        {
            public UserKey(User user)
            {
                if (user == null)
                {
                    throw new ArgumentNullException("user");
                }

                FacebookUserId = user.FacebookUserId;
                SteamUserId = user.SteamUserId;
            }

            private long FacebookUserId { get; set; }

            private string SteamUserId { get; set; }

            #region IEquatable<UserKey> Members

            public bool Equals(UserKey other)
            {
                if (ReferenceEquals(null, other))
                {
                    return false;
                }
                if (ReferenceEquals(this, other))
                {
                    return true;
                }
                return other.FacebookUserId == FacebookUserId && Equals(other.SteamUserId, SteamUserId);
            }

            #endregion

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }
                if (ReferenceEquals(this, obj))
                {
                    return true;
                }
                if (obj.GetType() != typeof (UserKey))
                {
                    return false;
                }
                return Equals((UserKey) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (int) ((FacebookUserId*397) ^ (SteamUserId != null ? SteamUserId.GetHashCode() : 0));
                }
            }

            public static bool operator ==(UserKey left, UserKey right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(UserKey left, UserKey right)
            {
                return !Equals(left, right);
            }
        }

        #endregion
    }
}