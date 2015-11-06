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
using SteamAchievements.Web.Helpers;
using SteamAchievements.Web.Models;
using SteamAchievements.Web.Resources;

namespace SteamAchievements.Web.Controllers
{
    public class AchievementController : FacebookController
    {
        private readonly IAchievementService _achievementService;

        public AchievementController(IAchievementService achievementService, IUserService userService,
                                     IErrorLogger errorLogger)
            : base(userService, errorLogger)
        {
            _achievementService = achievementService;
        }

        [HttpPost]
        public JsonResult ValidateProfile(string steamUserId)
        {
            if (steamUserId == null && UserSettings != null)
            {
                steamUserId = UserSettings.SteamUserId;
            }

            if (string.IsNullOrEmpty(steamUserId))
            {
                return Json(false);
            }

            var model = GetProfileViewModel(steamUserId);

            var data = new {Valid = !model.HasError, model.Error};
            return Json(data);
        }

        [HttpPost]
        public PartialViewResult Profile(string steamUserId)
        {
            steamUserId = steamUserId ?? UserSettings.SteamUserId;

            var model = GetProfileViewModel(steamUserId);

            return PartialView(model);
        }

        private ProfileViewModel GetProfileViewModel(string steamUserId)
        {
            var model = new ProfileViewModel();

            if (string.IsNullOrEmpty(steamUserId))
            {
                model.Error =
                    string.Format(
                                  "You haven't set your Steam Community Profile URL. Please set it on the <a href='{0}'>Settings</a> page.",
                                  Url.CanvasAction("Settings"));

                return model;
            }

            try
            {
                model.Profile = _achievementService.GetProfile(steamUserId);
            }
            catch (Exception ex)
            {
                model.Error = ex.Message;
            }

            if (model.Profile == null)
            {
                model.Error = Strings.SettingsInvalidCustomUrl;
            }

            if (model.HasError)
            {
                model.Error += string.Format(" <a class='error-settings-link' href='{0}'>{1}</a>",
                                             Url.CanvasAction("Settings"),
                                             Strings.MenuSettings);
            }

            return model;
        }

        [HttpPost]
        public PartialViewResult UnpublishedAchievements()
        {
            var achievements =
                _achievementService.GetUnpublishedAchievements(UserSettings.FacebookUserId, null);

            var serializer = new JavaScriptSerializer();
            ViewBag.Achievements = serializer.Serialize(achievements);

            return PartialView(achievements);
        }

        [HttpPost]
        public PartialViewResult Games(string steamUserId)
        {
            steamUserId = steamUserId ?? UserSettings.SteamUserId;

            var games = _achievementService.GetGames(steamUserId);
            return PartialView(games);
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
    }
}