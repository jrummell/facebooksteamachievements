using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SteamAchievements.Services;
using SteamAchievements.Services.Models;

namespace SteamAchievements.Web.Spa.Controllers
{
    /// <summary>
    /// Gets a user's steam profile
    /// </summary>
    /// <seealso cref="SteamAchievements.Web.Spa.Controllers.ApiController" />
    public class ProfileController : ApiController
    {
        private readonly IAchievementService _achievementService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileController"/> class.
        /// </summary>
        /// <param name="achievementService">The achievement service.</param>
        public ProfileController(IAchievementService achievementService)
        {
            _achievementService = achievementService;
        }

        /// <summary>
        /// Gets the specified steam user.
        /// </summary>
        /// <param name="steamUserId">The steam user identifier.</param>
        /// <returns></returns>
        [HttpGet("{steamUserId}")]
        public async Task<ActionResult<SteamProfileModel>> Get(string steamUserId)
        {
            var model = _achievementService.GetProfile(steamUserId);

            if (model == null)
            {
                return NotFound();
            }

            return model;
        }
    }
}
