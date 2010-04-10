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
        private Dictionary<int, Achievement> _achievements;
        private Dictionary<int, Game> _games;
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
        /// Gets the games.
        /// </summary>
        /// <value>The games.</value>
        public IQueryable<Game> Games
        {
            get { return _games.Values.AsQueryable(); }
            set { _games = value.ToDictionary(x => x.Id, x => x); }
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
            _users.Add(new UserKey(user), user);
        }

        /// <summary>
        /// Inserts the game on submit.
        /// </summary>
        /// <param name="game">The game.</param>
        public void InsertOnSubmit(Game game)
        {
            game.Id = _games.Values.Max(a => a.Id) + 1;
            _games.Add(game.Id, game);
        }

        /// <summary>
        /// Deletes all given achievements on submit.
        /// </summary>
        /// <param name="achievements">The achievements.</param>
        public void DeleteAllOnSubmit(IEnumerable<UserAchievement> achievements)
        {
            foreach (UserAchievement userAchievement in achievements)
            {
                _userAchievements.Remove(userAchievement.Id);
            }
        }

        /// <summary>
        /// Submits the changes.
        /// </summary>
        public void SubmitChanges()
        {
        }

        /// <summary>
        /// Inserts the achievement on submit.
        /// </summary>
        /// <param name="achievement">The achievement.</param>
        public void InsertOnSubmit(Achievement achievement)
        {
            achievement.Id = _achievements.Values.Max(a => a.Id) + 1;
            _achievements.Add(achievement.Id, achievement);
        }

        /// <summary>
        /// Inserts all given achievements on submit.
        /// </summary>
        /// <param name="achievements">The achievements.</param>
        public void InsertAllOnSubmit(IEnumerable<UserAchievement> achievements)
        {
            int maxId = _userAchievements.Values.Max(ua => ua.Id);

            foreach (UserAchievement userAchievement in achievements)
            {
                userAchievement.Id = ++maxId;

                _userAchievements.Add(userAchievement.Id, userAchievement);
            }
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

            IQueryable<Game> games =
                (new[]
                     {
                         new Game {Id = 1, Abbreviation = "game1", Name = "Game 1"},
                         new Game {Id = 2, Abbreviation = "game2", Name = "Game 2"}
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
                     }).AsQueryable();

            Achievements = achievements;
            Games = games;
            UserAchievements = userAchievements;
            Users = users;
        }

        #region IDisposable Members

        public void Dispose()
        {
            // nothing to dispose
        }

        #endregion

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

            public long FacebookUserId { get; set; }

            public string SteamUserId { get; set; }

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