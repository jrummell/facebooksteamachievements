#region License

//  Copyright 2012 John Rummell
//  
//  This file is part of SteamAchievements.
//  
//      SteamAchievements is free software: you can redistribute it and/or modify
//      it under the terms of the GNU General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      SteamAchievements is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU General Public License for more details.
//  
//      You should have received a copy of the GNU General Public License
//      along with SteamAchievements.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using SteamAchievements.Services.Models;

namespace SteamAchievements.Services
{
    public class MockAchievementService : IAchievementService
    {
        #region IAchievementService Members

        public void Dispose()
        {
        }

        public SteamProfile GetProfile(string steamUserId)
        {
            return new SteamProfile {SteamUserId = steamUserId, Headline = "Witty headline"};
        }

        public ICollection<Achievement> GetUnpublishedAchievements(long facebookUserId, DateTime? oldestDate,
                                                                   string language = null)
        {
            var random = new Random();
            return new[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10}
                .Select(i => new {achievementId = i, gameId = random.Next(1, 3)})
                .OrderBy(a => a.gameId).ThenBy(a => a.achievementId)
                .Select(a =>
                        new Achievement
                        {
                            ApiName = "Achievement" + a.achievementId,
                            Id = a.achievementId,
                            Description =
                                "Achievement " + a.achievementId +
                                " with a longer description ... a longer description ... a longer description.",
                            Game = new Game {Id = a.gameId, Name = "Game " + a.gameId},
                            Name = "Achievement " + a.achievementId
                        }).ToArray();
        }

        public ICollection<Game> GetGames(long facebookUserId)
        {
            return new[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10}
                .Select(i => new Game {Id = i, Name = "Game " + i})
                .ToArray();
        }

        public ICollection<Game> GetGames(string steamUserId)
        {
            return GetGames(1);
        }

        public int UpdateAchievements(long facebookUserId, string language = null)
        {
            return 0;
        }

        public bool PublishAchievements(long facebookUserId, IEnumerable<int> achievementIds)
        {
            return true;
        }

        public bool HideAchievements(long facebookUserId, IEnumerable<int> achievementIds)
        {
            return true;
        }

        public void UpdateNewUserAchievements(User user)
        {
        }

        #endregion
    }
}