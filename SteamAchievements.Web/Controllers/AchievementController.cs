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
using System.Web.Script.Serialization;
using Elmah;
using SteamAchievements.Services;
using SteamAchievements.Services.Models;
using SteamAchievements.Web.Models;

namespace SteamAchievements.Web.Controllers
{
    public class AchievementController : FacebookController
    {
        private readonly IAchievementService _achievementService;

        public AchievementController(IAchievementService achievementService, IUserService userService,
                                     IFacebookContextSettings facebookSettings)
            : base(userService, facebookSettings)
        {
            _achievementService = achievementService;
        }

        [HttpPost]
        public JsonResult ValidateProfile(string steamUserId)
        {
            steamUserId = steamUserId ?? UserSettings.SteamUserId;

            return Json(_achievementService.GetProfile(steamUserId) != null);
        }

        [HttpPost]
        public PartialViewResult Profile(string steamUserId)
        {
            steamUserId = steamUserId ?? UserSettings.SteamUserId;

            return PartialView(_achievementService.GetProfile(steamUserId));
        }

        [HttpPost]
        public PartialViewResult UnpublishedAchievements()
        {
            ICollection<Achievement> achievements =
                _achievementService.GetUnpublishedAchievements(FacebookUserId, null);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ViewBag.Achievements = serializer.Serialize(achievements);

            return PartialView(achievements);
        }

        [HttpPost]
        public PartialViewResult Games()
        {
            return PartialView(_achievementService.GetGames(FacebookUserId));
        }

        [HttpPost]
        public JsonResult UpdateAchievements()
        {
            try
            {
                return Json(_achievementService.UpdateAchievements(FacebookUserId));
            }
            catch (Exception exception)
            {
                // signaling the exception will rethrow it, so we'll simply log and continue
                ErrorLog errorLog = ErrorLog.GetDefault(System.Web.HttpContext.Current);
                errorLog.Log(new Error(exception));

                return Json(new {Error = new {exception.Message, StackTrace = exception.ToString()}});
            }
        }

        [HttpPost]
        public JsonResult PublishAchievements(IEnumerable<int> achievementIds)
        {
            return Json(_achievementService.PublishAchievements(FacebookUserId, achievementIds));
        }

        [HttpPost]
        public JsonResult HideAchievements(IEnumerable<int> achievementIds)
        {
            return Json(_achievementService.HideAchievements(FacebookUserId, achievementIds));
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