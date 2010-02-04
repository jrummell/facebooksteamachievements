using System.Collections.Generic;
using System.Linq;

namespace SteamAchievements.Data
{
    public interface ISteamRepository
    {
        /// <summary>
        /// Gets the achievements.
        /// </summary>
        /// <value>The achievements.</value>
        IQueryable<Achievement> Achievements { get; }

        /// <summary>
        /// Gets the user achievements.
        /// </summary>
        /// <value>The user achievements.</value>
        IQueryable<UserAchievement> UserAchievements { get; }

        /// <summary>
        /// Gets the games.
        /// </summary>
        /// <value>The games.</value>
        IQueryable<Game> Games { get; }

        /// <summary>
        /// Gets the users.
        /// </summary>
        /// <value>The users.</value>
        IQueryable<User> Users { get; }

        /// <summary>
        /// Inserts the user on submit.
        /// </summary>
        /// <param name="user">The user.</param>
        void InsertOnSubmit(User user);

        /// <summary>
        /// Deletes all given achievements on submit.
        /// </summary>
        /// <param name="achievements">The achievements.</param>
        void DeleteAllOnSubmit(IEnumerable<UserAchievement> achievements);

        /// <summary>
        /// Submits the changes.
        /// </summary>
        void SubmitChanges();

        /// <summary>
        /// Inserts the achievement on submit.
        /// </summary>
        /// <param name="achievement">The achievement.</param>
        void InsertOnSubmit(Achievement achievement);

        /// <summary>
        /// Inserts all given achievements on submit.
        /// </summary>
        /// <param name="achievements">The achievements.</param>
        void InsertAllOnSubmit(IEnumerable<UserAchievement> achievements);
    }
}