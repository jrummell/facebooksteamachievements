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
using System.Collections.Generic;
using System.Web.Mvc;
using SteamAchievements.Services;
using SteamAchievements.Web.Models;

namespace SteamAchievements.Web.Controllers
{
    [ElmahHandleError]
    public class AchievementController : FacebookController
    {
        private readonly IAchievementService _achievementService;

        public AchievementController(IAchievementService achievementService, IUserService userService,
                                     IFacebookContextSettings facebookSettings)
            : base(userService, facebookSettings)
        {
            _achievementService = achievementService;
        }

        public JsonResult GetProfile()
        {
            return Json(_achievementService.GetProfile(UserSettings.SteamUserId));
        }

        public JsonResult GetUnpublishedAchievements(DateTime? oldestDate)
        {
            return Json(_achievementService.GetUnpublishedAchievements(UserSettings.SteamUserId, oldestDate));
        }

        public JsonResult GetGames()
        {
            return Json(_achievementService.GetGames(UserSettings.SteamUserId));
        }

        public JsonResult UpdateAchievements()
        {
            return Json(_achievementService.UpdateAchievements(UserSettings.SteamUserId));
        }

        public JsonResult PublishAchievements(IEnumerable<int> achievementIds)
        {
            return Json(_achievementService.PublishAchievements(UserSettings.SteamUserId, achievementIds));
        }

        public JsonResult HideAchievements(IEnumerable<int> achievementIds)
        {
            return Json(_achievementService.HideAchievements(UserSettings.SteamUserId, achievementIds));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _achievementService.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}