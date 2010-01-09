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

namespace SteamAchievements.Data
{
    public class MockAchievementManager : IAchievementManager
    {
        #region IAchievementManager Members

        public string GetSteamUserId(long facebookUserId)
        {
            return "MySteamID";
        }

        public IEnumerable<Achievement> GetAchievements(string steamUserId, int gameId)
        {
            if (gameId == 1)
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
            else
            {
                return new List<Achievement>
                           {
                               new Achievement
                                   {
                                       Id = 1,
                                       Name = "Head Of The Class",
                                       Description = "Play a complete round with every class.",
                                       ImageUrl =
                                           "http://media.steampowered.com/steamcommunity/public/images/apps/440/tf_play_game_everyclass.jpg"
                                   },
                               new Achievement
                                   {
                                       Id = 2,
                                       Name = "World Traveler",
                                       Description =
                                           "Play a complete game on 2Fort, Dustbowl, Granary, Gravel Pit, Hydro, and Well (CP).",
                                       ImageUrl =
                                           "http://media.steampowered.com/steamcommunity/public/images/apps/440/tf_play_game_everymap.jpg"
                                   }
                           };
            }
        }

        public void UpdateAchievements(string steamUserId, IEnumerable<Achievement> achievements)
        {
        }

        public IEnumerable<Achievement> GetLatestAchievements(string steamUserId)
        {
            // return an empty array so that the service won't try to publish them.
            return new Achievement[0];
        }

        public IEnumerable<Game> GetGames()
        {
            return new List<Game>
                       {
                           new Game {Abbreviation = "l4d", Id = 1, Name = "Left 4 Dead"},
                           new Game {Abbreviation = "tf2", Id = 2, Name = "Team Fortress 2"}
                       };
        }

        public void UpdateSteamUserId(long facebookUserId, string steamUserId)
        {
        }

        #endregion
    }
}