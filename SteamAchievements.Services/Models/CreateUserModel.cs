#region License

//  Copyright  John Rummell
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

using System.ComponentModel.DataAnnotations;

namespace SteamAchievements.Services.Models
{
    public class CreateUserModel
    {
        /// <summary>
        /// Gets or sets the facebook user identifier.
        /// </summary>
        /// <value>
        /// The facebook user identifier.
        /// </value>
        [Required]
        [Range(1, long.MaxValue)]
        public long FacebookUserId { get; set; }
    }
}