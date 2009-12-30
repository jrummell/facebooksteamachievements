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
using SteamAchievements.Data;

namespace SteamAchievements.Services
{
    /// <summary>
    /// A test implementation of <see cref="IAchievementService"/> that doesn't rely on a database.
    /// </summary>
    public class MockAchievementService : IAchievementService
    {
        #region IAchievementService Members

        public List<Achievement> GetAchievements(SteamUserIdGameIdParameter json)
        {
            return new List<Achievement>
                       {
                           new Achievement
                               {
                                   Id = 1,
                                   Name = "Cr0wnd",
                                   Description = "Kill a Witch with a single headshot.",
                                   ImageUrl =
                                       "http://media.steampowered.com/steamcommunity/public/images/apps/500/2b9a77bc3d6052538797337dad9eee87e74f639d.jpg"
                               },
                           new Achievement
                               {
                                   Id = 2,
                                   Name = "Dead Stop",
                                   Description = "Punch a Hunter as he is pouncing.",
                                   ImageUrl =
                                       "http://media.steampowered.com/steamcommunity/public/images/apps/500/0abefbfb22ef97d532d95fbb5261ff2d52f55e5f.jpg"
                               }
                       };
        }

        public List<Game> GetGames()
        {
            return new List<Game>
                       {
                           new Game {Abbreviation = "l4d", Id = 1, Name = "Left 4 Dead"},
                           new Game {Abbreviation = "tf2", Id = 2, Name = "Team Fortress 2"}
                       };
        }

        public bool UpdateAchievements(SteamUserIdParameter json)
        {
            return true;
        }

        public bool UpdateSteamUserId(SteamUserIdFacebookUserIdParameter json)
        {
            return true;
        }

        public bool PublishLatestAchievements(SteamUserIdFacebookUserIdParameter json)
        {
            return true;
        }

        #endregion
    }
}