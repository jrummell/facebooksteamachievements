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

using System.Web.Mvc;
using AutoMapper;
using Facebook.Web.Mvc;
using SteamAchievements.Services;
using SteamAchievements.Web.Models;

namespace SteamAchievements.Web.Controllers
{
#if !DEBUG
    [CanvasAuthorize(Permissions = "publish_stream,offline_access")]
#endif
    public class HomeController : FacebookController
    {
        private readonly IAchievementService _achievementService;

        public HomeController(IAchievementService achievementService, IUserService userService,
                              IFacebookContextSettings facebookSettings)
            : base(userService, facebookSettings)
        {
            _achievementService = achievementService;
        }

        public ActionResult Index()
        {
            User user = UserSettings ?? new User();
            SettingsViewModel model = Mapper.Map<User, SettingsViewModel>(user);
            return View(model);
        }

        public ActionResult Publish()
        {
#if DEBUG
            ViewBag.EnableLog = true;
#else
            ViewBag.EnableLog = false;
#endif
            User user = UserSettings ?? new User();
            SettingsViewModel model = Mapper.Map<User, SettingsViewModel>(user);
            return View(model);
        }

        public ActionResult Settings()
        {
            User user = UserSettings ?? new User();
            SettingsViewModel model = Mapper.Map<User, SettingsViewModel>(user);
            return View(model);
        }

        public ActionResult Deauthorize()
        {
            if (UserSettings == null)
            {
                return View();
            }

            UserService.DeauthorizeUser(UserSettings.FacebookUserId);

            UserSettings = null;

            return View();
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