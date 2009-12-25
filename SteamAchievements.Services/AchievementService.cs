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
using System.Collections.ObjectModel;
using System.Linq;
using SteamAchievements.Data;

namespace SteamAchievements.Services
{
    public class AchievementService : IAchievementService
    {
        private readonly SteamCommunityManager _communityService = new SteamCommunityManager();
        private readonly AchievementManager _service = new AchievementManager();

        #region IAchievementService Members

        public AchievementCollection GetAchievements(GetAchievementsParameter json)
        {
            if (json == null)
            {
                throw new ArgumentNullException("json");
            }

            return _service.GetAchievements(json.SteamUserId, json.GameId);
        }

        public Collection<Game> GetGames()
        {
            return new Collection<Game>(_service.GetGames().ToList());
        }

        public bool UpdateAchievements(UpdateAchievementsParameter json)
        {
            if (json == null)
            {
                throw new ArgumentNullException("json");
            }

            AchievementCollection achievements = _communityService.GetAchievements(json.SteamUserId);

            _service.UpdateAchievements(json.SteamUserId, achievements);

            return true;
        }

        public bool UpdateSteamUserId(UpdateSteamUserIdParameter json)
        {
            if (json == null)
            {
                throw new ArgumentNullException("json");
            }

            _service.UpdateSteamUserId(json.FacebookUserId, json.SteamUserId);

            return true;
        }

        #endregion
    }

    public class MockAchievementService : IAchievementService
    {
        #region IAchievementService Members

        public AchievementCollection GetAchievements(GetAchievementsParameter json)
        {
            return new AchievementCollection("steamUser", "l4d")
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

        public Collection<Game> GetGames()
        {
            return new Collection<Game>
                       {
                           new Game {Abbreviation = "l4d", Id = 1, Name = "Left 4 Dead"},
                           new Game {Abbreviation = "tf2", Id = 2, Name = "Team Fortress 2"}
                       };

        }

        public bool UpdateAchievements(UpdateAchievementsParameter json)
        {
            return true;
        }

        public bool UpdateSteamUserId(UpdateSteamUserIdParameter json)
        {
            return true;
        }

        #endregion
    }
}