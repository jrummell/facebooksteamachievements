using System.Collections.ObjectModel;
using SteamAchievements.Data;

namespace SteamAchievements.Services
{
    public class AchievementService : IAchievementService
    {
        private readonly SteamCommunityManager _communityService = new SteamCommunityManager();
        private readonly AchievementManager _service = new AchievementManager();

        #region IAchievementService Members

        public AchievementCollection GetAchievements(string steamUserId, int gameId)
        {
            return new AchievementCollection(steamUserId, "l4d")
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

            return _service.GetAchievements(steamUserId, gameId);
        }

        public Collection<Game> GetGames()
        {
            return new Collection<Game>
                       {
                           new Game {Abbreviation = "l4d", Id = 1, Name = "Left 4 Dead"},
                           new Game {Abbreviation = "tf2", Id = 2, Name = "Team Fortress 2"}
                       };

            //return _service.GetGames();
        }

        public void UpdateAchievements(string steamUserId)
        {
            AchievementCollection achievements = _communityService.GetAchievements(steamUserId);

            _service.UpdateAchievements(steamUserId, achievements);
        }

        #endregion
    }
}