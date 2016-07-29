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
using System.Web.Mvc;
using AutoMapper;
using SteamAchievements.Services;
using SteamAchievements.Services.Models;
using SteamAchievements.Web.Models;

namespace SteamAchievements.Web.Controllers
{
    public class HomeController : FacebookController
    {
        private readonly IAchievementService _achievementService;
        private readonly IFormsAuthenticationService _authenticationService;

        public HomeController(IAchievementService achievementService, IUserService userService, IErrorLogger errorLogger, IFormsAuthenticationService authenticationService)
            : base(userService, errorLogger)
        {
            _achievementService = achievementService;
            _authenticationService = authenticationService;
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            ViewResultBase result = filterContext.Result as ViewResultBase;
            if (result != null)
            {
                SettingsViewModel model = result.Model as SettingsViewModel;
                if (model != null)
                {
                    model.EnableLog = Properties.Settings.Default.EnableClientLogging;
                }
            }
        }

        public ActionResult Index()
        {
            SettingsViewModel model = Mapper.Map<User, SettingsViewModel>(UserSettings ?? new User());

            return View(model);
        }

        public ActionResult Games()
        {
            User user = UserSettings ?? new User();

            IndexViewModel model = Mapper.Map<User, IndexViewModel>(user);

            return View(model);
        }

        public ActionResult Settings()
        {
            User user = UserSettings ?? new User();
            SettingsViewModel model = Mapper.Map<User, SettingsViewModel>(user);
            return View(model);
        }

        [HttpPost]
        public ActionResult SaveSettings(SettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Settings", model);
            }

            User user = UserService.GetUser(UserSettings.Id);

            user.SteamUserId = model.SteamUserId;
            user.PublishDescription = model.PublishDescription;

            UserService.UpdateUser(user);

            ViewBag.Success = true;

            return View("Settings", model);
        }

        [AllowAnonymous]
        public ActionResult Canvas()
        {
            return View();
        }

        public ActionResult Deauthorize()
        {
            if (UserSettings == null)
            {
                return View();
            }

            UserService.DeauthorizeUser(UserSettings.Id);

            _authenticationService.SignOut();

            return View();
        }
    }
}