#region License

//  Copyright 2016 John Rummell
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
using System.ComponentModel.DataAnnotations.Schema;

namespace SteamAchievements.Data
{
    [Table("steam_UserAchievement")]
    public class UserAchievement
    {
        public int AchievementId { get; set; }

        public DateTime Date { get; set; }

        public int Id { get; set; }

        public bool Published { get; set; }

        public bool Hidden { get; set; }

        public virtual Achievement Achievement { get; set; }

        public virtual User User { get; set; }
        public int UserId { get; set; }
    }
}