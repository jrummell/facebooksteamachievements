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
using AutoMapper;
using Elmah;
using Facebook.Web.Mvc;
using SteamAchievements.Services;
using SteamAchievements.Web.Models;

namespace SteamAchievements.Web.Controllers
{
#if !DEBUG
    [CanvasAuthorize(Permissions = "publish_stream,offline_access")]
#endif
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

        [HttpPost]
        public JsonResult GetProfile(string steamUserId)
        {
            steamUserId = steamUserId ?? UserSettings.SteamUserId;

            return Json(_achievementService.GetProfile(steamUserId));
        }

        [HttpPost]
        public JsonResult GetUnpublishedAchievements(DateTime? oldestDate)
        {
            return Json(_achievementService.GetUnpublishedAchievements(UserSettings.SteamUserId, oldestDate));
        }

        [HttpPost]
        public JsonResult GetGames()
        {
            return Json(_achievementService.GetGames(UserSettings.SteamUserId));
        }

        [HttpPost]
        public JsonResult UpdateAchievements()
        {
            return Json(_achievementService.UpdateAchievements(UserSettings.SteamUserId));
        }

        [HttpPost]
        public JsonResult PublishAchievements(IEnumerable<int> achievementIds)
        {
            return Json(_achievementService.PublishAchievements(UserSettings.SteamUserId, achievementIds));
        }

        [HttpPost]
        public JsonResult HideAchievements(IEnumerable<int> achievementIds)
        {
            return Json(_achievementService.HideAchievements(UserSettings.SteamUserId, achievementIds));
        }

        [HttpPost]
        public JsonResult SaveSettings(SettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json("Invalid");
            }

            User user = UserService.GetUser(FacebookUserId);
            bool newUser = false;
            if (user == null)
            {
                newUser = true;
                user = new User {FacebookUserId = FacebookUserId, AccessToken = String.Empty};
            }

            Mapper.Map(model, user);

            try
            {
                UserService.UpdateUser(user);

                UserSettings = user;
            }
            catch (DuplicateSteamUserException)
            {
                return Json("DuplicateError");
            }

            if (newUser)
            {
                try
                {
                    _achievementService.UpdateNewUserAchievements(user);
                }
                catch (Exception ex)
                {
                    // log and swallow exceptions so that the settings can be saved
                    ErrorSignal.FromCurrentContext().Raise(ex);
                }
            }

            return Json("Success");
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