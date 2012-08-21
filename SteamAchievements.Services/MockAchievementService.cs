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
            return new[]
                {
                    new Achievement
                        {
                            ApiName = "Achievement1",
                            Id = 1,
                            Description = "Achievement 1",
                            Game = new Game {Id = 1, Name = "Game 1"},
                            Name = "Achievement 1"
                        },
                    new Achievement
                        {
                            ApiName = "Achievement2",
                            Id = 2,
                            Description = "Achievement 2",
                            Game = new Game {Id = 1, Name = "Game 1"},
                            Name = "Achievement 2"
                        },
                    new Achievement
                        {
                            ApiName = "Achivement3",
                            Description = "Achievement 3",
                            Name = "Achievement 3",
                            Game = new Game {Id = 2, Name = "Game 2"},
                            Id = 3
                        }
                };
        }

        public ICollection<Game> GetGames(long facebookUserId)
        {
            return new[] {new Game {Id = 1, Name = "Game 1"}, new Game {Id = 2, Name = "Game 2"}};
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