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
        private Dictionary<int, Achievement> _achievements = new Dictionary<int, Achievement>();
        private Dictionary<int, Game> _games = new Dictionary<int, Game>();
        private Dictionary<int, UserAchievement> _userAchievements = new Dictionary<int, UserAchievement>();
        private Dictionary<UserKey, User> _users = new Dictionary<UserKey, User>();

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