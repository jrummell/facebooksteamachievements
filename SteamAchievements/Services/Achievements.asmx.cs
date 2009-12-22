using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Script.Services;
using System.Web.Services;
using SteamAchievements.Data;

namespace SteamAchievements.Services
{
    /// <summary>
    /// Summary description for Achievements
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [ScriptService]
    public class Achievements : WebService
    {
        [WebMethod]
        public void Update(string steamUserId)
        {
            if (steamUserId == null)
            {
                throw new ArgumentNullException("steamUserId");
            }

            SteamCommunityService communityService = new SteamCommunityService();
            AchievementService service = new AchievementService();
            AchievementCollection achievements = communityService.GetAchievements(steamUserId);
            service.UpdateAchievements(steamUserId, achievements);
        }

        [WebMethod]
        public IEnumerable<Achievement> Get(string steamUserId, int gameId)
        {
            AchievementService service = new AchievementService();

            return service.GetAchievements(steamUserId, gameId);
        }
    }
}