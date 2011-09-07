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

using System.Collections.Generic;
using System.Linq;

namespace SteamAchievements.Services
{
    public static class AchievementCollectionExtensions
    {
        /// <summary>
        /// Converts the <see cref="Data.Achievement"/>s to <see cref="Achievement"/>s.
        /// </summary>
        /// <param name="achievements">The achievements.</param>
        /// <param name="games">The games.</param>
        /// <returns></returns>
        public static List<Achievement> ToSimpleAchievementList(
            this IEnumerable<Data.Achievement> achievements, IEnumerable<Game> games)
        {
            return (from game in games
                    from achievement in achievements
                    where achievement.GameId == game.Id
                    select new Achievement
                               {
                                   Id = achievement.Id,
                                   ApiName = achievement.ApiName,
                                   ImageUrl = achievement.ImageUrl,
                                   Name = achievement.Name,
                                   Description = achievement.Description,
                                   Game = game
                               }).ToList();
        }

        /// <summary>
        /// Converts the <see cref="UserAchievement"/>s to <see cref="Data.UserAchievement"/>s.
        /// </summary>
        /// <param name="achievements">The achievements.</param>
        /// <param name="facebookUserId">The facebook user id.</param>
        /// <returns></returns>
        public static ICollection<Data.UserAchievement> ToDataAchievements(
            this ICollection<UserAchievement> achievements, long facebookUserId)
        {
            return (from achievement in achievements
                    select new Data.UserAchievement
                               {
                                   Date = achievement.Date,
                                   FacebookUserId = facebookUserId,
                                   Achievement = new Data.Achievement
                                                     {
                                                         ApiName = achievement.AchievementApiName,
                                                         Name = achievement.Name,
                                                         Description = achievement.Description,
                                                         ImageUrl = achievement.ImageUrl.ToString(),
                                                         GameId = achievement.Game.Id
                                                     }
                               }).ToList();
        }
    }
}