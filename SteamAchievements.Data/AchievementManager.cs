using System;
using System.Collections.Generic;
using System.Linq;

namespace SteamAchievements.Data
{
    public class AchievementManager
    {
        public string GetSteamUserId(long facebookUserId)
        {
            SteamDataContext context = new SteamDataContext();

            IQueryable<string> query = from user in context.Users
                                       where user.FacebookUserId == facebookUserId
                                       select user.SteamUserId;

            return query.SingleOrDefault();
        }

        public AchievementCollection GetAchievements(string steamUserId, int gameId)
        {
            if (steamUserId == null)
            {
                throw new ArgumentNullException("steamUserId");
            }

            SteamDataContext context = new SteamDataContext();
            IQueryable<Achievement> achievements =
                from userAchievement in context.UserAchievements
                where userAchievement.SteamUserId == steamUserId && userAchievement.Achievement.GameId == gameId
                select userAchievement.Achievement;

            return new AchievementCollection(achievements.ToList(), steamUserId, null);
        }

        public void UpdateAchievements(string steamUserId, IEnumerable<Achievement> achievements)
        {
            if (steamUserId == null)
            {
                throw new ArgumentNullException("steamUserId");
            }

            if (achievements == null)
            {
                throw new ArgumentNullException("achievements");
            }

            if (!achievements.Any())
            {
                return;
            }

            InsertMissingAchievements(achievements);
            AssignNewAchievements(steamUserId, achievements);
        }

        private void AssignNewAchievements(string steamUserId, IEnumerable<Achievement> achievements)
        {
            SteamDataContext context = new SteamDataContext();

            IQueryable<int> unassignedAchievements =
                from dbAchievement in context.Achievements
                join achievement in achievements on dbAchievement.Name equals achievement.Name
                select dbAchievement.Id;

            if (!unassignedAchievements.Any())
            {
                return;
            }

            DateTime now = DateTime.Now;
            foreach (int achievement in unassignedAchievements)
            {
                UserAchievement userAchievement = new UserAchievement
                                                      {
                                                          SteamUserId = steamUserId,
                                                          AchievementId = achievement,
                                                          Date = now
                                                      };
                context.UserAchievements.Attach(userAchievement);
            }

            context.SubmitChanges();
        }

        private void InsertMissingAchievements(IEnumerable<Achievement> achievements)
        {
            SteamDataContext context = new SteamDataContext();

            IEnumerable<Achievement> missingAchievements = from achievement in achievements
                                                           join dbAchievement in context.Achievements on
                                                               achievement.Name equals
                                                               dbAchievement.Name into x
                                                           from missing in x.DefaultIfEmpty()
                                                           where missing == null
                                                           select achievement;

            if (!missingAchievements.Any())
            {
                return;
            }

            foreach (Achievement achievement in missingAchievements)
            {
                Achievement newAchievement = new Achievement
                                                 {
                                                     Name = achievement.Name,
                                                     GameId = achievement.GameId,
                                                     Description = achievement.Description,
                                                     ImageUrl = achievement.ImageUrl
                                                 };
                context.Achievements.Attach(newAchievement);
            }

            context.SubmitChanges();
        }

        public IEnumerable<Game> GetGames()
        {
            SteamDataContext context = new SteamDataContext();
            return context.Games;
        }
    }
}