#region License

//  Copyright 2012 John Rummell
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
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using SteamAchievements.Services;
using SteamAchievements.Services.Models;

namespace SteamAchievements.Web.Controllers
{
    public class AchievementController : FacebookController
    {
        private readonly IAchievementService _achievementService;
        private readonly IFacebookClientService _facebookService;
        private readonly IUserService _userService;

        public AchievementController(IAchievementService achievementService, IUserService userService,
                                     IFacebookClientService facebookService, IErrorLogger errorLogger)
            : base(userService, errorLogger)
        {
            _achievementService = achievementService;
            _userService = userService;
            _facebookService = facebookService;
        }

        [HttpPost]
        public JsonResult ValidateProfile(string steamUserId)
        {
            if (steamUserId == null && UserSettings != null)
            {
                steamUserId = UserSettings.SteamUserId;
            }

            if (String.IsNullOrEmpty(steamUserId))
            {
                return Json(false);
            }

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
                _achievementService.GetUnpublishedAchievements(UserSettings.FacebookUserId, null);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ViewBag.Achievements = serializer.Serialize(achievements);

            return PartialView(achievements);
        }

        [HttpPost]
        public PartialViewResult Games(string steamUserId)
        {
            steamUserId = steamUserId ?? UserSettings.SteamUserId;

            return PartialView(_achievementService.GetGames(steamUserId));
        }

        [HttpPost]
        public JsonResult UpdateAchievements()
        {
            try
            {
                return Json(_achievementService.UpdateAchievements(UserSettings.FacebookUserId));
            }
            catch (Exception exception)
            {
                // signaling the exception will rethrow it, so we'll simply log and continue
                ErrorLogger.Log(exception);

                return Json(new {Error = new {exception.Message, StackTrace = exception.ToString()}});
            }
        }

        [HttpPost]
        public JsonResult PublishAchievements(IEnumerable<int> achievementIds)
        {
            return Json(_achievementService.PublishAchievements(UserSettings.FacebookUserId, achievementIds));
        }

        [HttpPost]
        public JsonResult HideAchievements(IEnumerable<int> achievementIds)
        {
            return Json(_achievementService.HideAchievements(UserSettings.FacebookUserId, achievementIds));
        }

        [HttpPost]
        public JsonResult UpdateAccessToken()
        {
            string accessToken = _facebookService.UpdateAccessToken(UserSettings.AccessToken);
            if (!String.IsNullOrEmpty(accessToken))
            {
                UserSettings.AccessToken = accessToken;
                _userService.UpdateUser(UserSettings);

                return Json(true);
            }

            return Json(false);
        }

        [HttpPost]
        public JsonResult SetAccessToken(string accessToken)
        {
            if (!String.IsNullOrEmpty(accessToken))
            {
                UserSettings.AccessToken = accessToken;
                UserSettings.FacebookUserId = _facebookService.GetUserId(accessToken);
                _userService.UpdateUser(UserSettings);

                return Json(true);
            }

            return Json(false);
        }
    }
}