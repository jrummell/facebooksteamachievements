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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SteamAchievements.Services;
using SteamAchievements.Web.Spa.Models;

namespace SteamAchievements.Web.Spa.Controllers
{
    /// <summary>
    ///     Manages a user's achievements
    /// </summary>
    /// <seealso cref="SteamAchievements.Web.Spa.Controllers.ApiController" />
    public class AchievementController : ApiController
    {
        private readonly IAchievementService _achievementService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AchievementController" /> class.
        /// </summary>
        /// <param name="achievementService">The achievement service.</param>
        public AchievementController(IAchievementService achievementService)
        {
            _achievementService = achievementService;
        }

        /// <summary>
        ///     Updates the user's achievements.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        [HttpPost("Update/{userId}")]
        public async Task<ActionResult<int>> Update(int userId)
        {
            return _achievementService.UpdateAchievements(userId, CultureHelper.GetLanguage());
        }

        /// <summary>
        ///     Gets the user's unpublished achievements.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        [HttpGet("{userId}")]
        public async Task<ActionResult<GameAchievementsModel[]>> Get(int userId)
        {
            var gameAchievements = _achievementService.GetUnpublishedAchievements(userId, null)
                                                      .ToLookup(a => a.Game.Name)
                                                      .Select(group => new GameAchievementsModel
                                                                       {
                                                                           Game = group.First().Game,
                                                                           Achievements = group
                                                                       })
                                                      .ToArray();

            // we don't need the game property on model since we're returning games with achievements
            foreach (var model in gameAchievements)
            {
                foreach (var achievement in model.Achievements)
                {
                    achievement.Game = null;
                }
            }

            return gameAchievements;
        }

        /// <summary>
        /// Publishes the user's achievements.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="achievementIds">The achievement ids.</param>
        /// <returns></returns>
        [HttpPost("{userId}")]
        public async Task<ActionResult<bool>> Publish(int userId, IEnumerable<int> achievementIds)
        {
            return _achievementService.PublishAchievements(userId, achievementIds);
        }

        /// <summary>
        /// Hides the user's achievements.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="achievementIds">The achievement ids.</param>
        /// <returns></returns>
        [HttpDelete("{userId}")]
        public async Task<ActionResult<bool>> Hide(int userId, IEnumerable<int> achievementIds)
        {
            return _achievementService.HideAchievements(userId, achievementIds);
        }
    }
}
