using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SteamAchievements.Services;
using SteamAchievements.Services.Models;

namespace SteamAchievements.Web.Spa.Controllers
{
    public class ProfileController : ApiController
    {
        private readonly IAchievementService _achievementService;

        public ProfileController(IAchievementService achievementService)
        {
            _achievementService = achievementService;
        }

        [HttpGet("{steamUserId}")]
        public async Task<SteamProfileModel> Get(string steamUserId)
        {
            return _achievementService.GetProfile(steamUserId);
        }
    }
}
