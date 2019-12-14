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

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SteamAchievements.Services;
using SteamAchievements.Services.Models;

namespace SteamAchievements.Web.Spa.Controllers
{
    /// <summary>
    /// Gets a user's games
    /// </summary>
    /// <seealso cref="SteamAchievements.Web.Spa.Controllers.ApiController" />
    public class GameController : ApiController
    {
        private readonly IAchievementService _achievementService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameController"/> class.
        /// </summary>
        /// <param name="achievementService">The achievement service.</param>
        public GameController(IAchievementService achievementService)
        {
            _achievementService = achievementService;
        }

        /// <summary>
        /// Gets the user's games.
        /// </summary>
        /// <param name="steamUserId">The steam user identifier.</param>
        /// <returns></returns>
        [HttpGet("{steamUserId}")]
        public async Task<ActionResult<GameModel[]>> Get(string steamUserId)
        {
            return _achievementService.GetGames(steamUserId).ToArray();
        }
    }
}
