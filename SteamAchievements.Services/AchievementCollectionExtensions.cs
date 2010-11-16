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

namespace SteamAchievements.Services
{
    public static class AchievementCollectionExtensions
    {
        public static List<SimpleAchievement> ToSimpleAchievementList(this IEnumerable<Data.Achievement> achievements,
                                                                      IEnumerable<Game> games)
        {
            return (from game in games
                    from achievement in achievements
                    where achievement.GameId == game.Id
                    select new SimpleAchievement
                               {
                                   Id = achievement.Id,
                                   ImageUrl = achievement.ImageUrl,
                                   Name = achievement.Name,
                                   Description = achievement.Description,
                                   Game = new SimpleGame
                                              {
                                                  Id = game.Id,
                                                  Name = game.Name,
                                                  ImageUrl = game.ImageUrl.ToString(),
                                                  StatsUrl = game.StatsUrl.ToString(),
                                                  StoreUrl = game.StoreUrl.ToString()
                                              }
                               }).ToList();
        }

        public static IEnumerable<UserAchievement> ToAchievements(this IEnumerable<Data.Achievement> achievements,
                                                                  IEnumerable<Game> games)
        {
            return from achievement in achievements
                   from game in games
                   where achievement.GameId == game.Id
                   select new UserAchievement
                              {
                                  Name = achievement.Name,
                                  Description = achievement.Description,
                                  ImageUrl = new Uri(achievement.ImageUrl, UriKind.Absolute),
                                  Closed = true,
                                  Game = game
                              };
        }

        public static IEnumerable<Data.UserAchievement> ToDataAchievements(
            this IEnumerable<UserAchievement> achievements)
        {
            return from achievement in achievements
                   select new Data.UserAchievement
                              {
                                  Date = achievement.Date,
                                  SteamUserId = achievement.SteamUserId,
                                  Achievement = new Data.Achievement
                                                    {
                                                        Name = achievement.Name,
                                                        Description = achievement.Description,
                                                        ImageUrl = achievement.ImageUrl.ToString(),
                                                        GameId = achievement.Game.Id
                                                    }
                              };
        }
    }
}