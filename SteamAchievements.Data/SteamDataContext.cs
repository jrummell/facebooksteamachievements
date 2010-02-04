using System.Collections.Generic;
using System.Linq;

namespace SteamAchievements.Data
{
    internal partial class SteamDataContext : ISteamRepository
    {
        #region ISteamRepository Members

        /// <summary>
        /// Gets the achievements.
        /// </summary>
        /// <value>The achievements.</value>
        IQueryable<Achievement> ISteamRepository.Achievements
        {
            get { return Achievements; }
        }

        /// <summary>
        /// Gets the user achievements.
        /// </summary>
        /// <value>The user achievements.</value>
        IQueryable<UserAchievement> ISteamRepository.UserAchievements
        {
            get { return UserAchievements; }
        }

        /// <summary>
        /// Gets the games.
        /// </summary>
        /// <value>The games.</value>
        IQueryable<Game> ISteamRepository.Games
        {
            get { return Games; }
        }

        /// <summary>
        /// Gets the users.
        /// </summary>
        /// <value>The users.</value>
        IQueryable<User> ISteamRepository.Users
        {
            get { return Users; }
        }

        /// <summary>
        /// Inserts the user on submit.
        /// </summary>
        /// <param name="user">The user.</param>
        public void InsertOnSubmit(User user)
        {
            Users.InsertOnSubmit(user);
        }

        /// <summary>
        /// Deletes all given achievements on submit.
        /// </summary>
        /// <param name="achievements">The achievements.</param>
        public void DeleteAllOnSubmit(IEnumerable<UserAchievement> achievements)
        {
            UserAchievements.DeleteAllOnSubmit(achievements);
        }

        /// <summary>
        /// Inserts the achievement on submit.
        /// </summary>
        /// <param name="achievement">The achievement.</param>
        public void InsertOnSubmit(Achievement achievement)
        {
            Achievements.InsertOnSubmit(achievement);
        }

        /// <summary>
        /// Inserts all given achievements on submit.
        /// </summary>
        /// <param name="achievements">The achievements.</param>
        public void InsertAllOnSubmit(IEnumerable<UserAchievement> achievements)
        {
            UserAchievements.InsertAllOnSubmit(achievements);
        }

        #endregion
    }
}