﻿#region License

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

using System.Web.Mvc;
using SteamAchievements.Services;
using SteamAchievements.Services.Models;
using SteamAchievements.Web.Filters;
using SteamAchievements.Web.Models;
using SteamAchievements.Web.Properties;

namespace SteamAchievements.Web.Controllers
{
    public abstract class FacebookController : Controller
    {
        private static readonly FacebookMode _facebookMode = Settings.Default.Mode;
        private readonly IErrorLogger _errorLogger;
        private readonly IUserService _userService;

        /// <summary>
        ///   Initializes a new instance of the <see cref="FacebookController" /> class.
        /// </summary>
        /// <param name="userService"> The user service. </param>
        /// <param name="errorLogger"> </param>
        protected FacebookController(IUserService userService, IErrorLogger errorLogger)
        {
            _userService = userService;
            _errorLogger = errorLogger;
        }

        /// <summary>
        ///   Gets the user settings.
        /// </summary>
        public User UserSettings
        {
            get { return Session[CanvasAuthorizeAttribute.UserSettingsKey] as User; }
            set { Session[CanvasAuthorizeAttribute.UserSettingsKey] = value; }
        }

        /// <summary>
        ///   Gets the user service.
        /// </summary>
        protected IUserService UserService
        {
            get { return _userService; }
        }

        /// <summary>
        ///   Gets the error logger.
        /// </summary>
        protected IErrorLogger ErrorLogger
        {
            get { return _errorLogger; }
        }

        protected static FacebookMode FacebookMode
        {
            get { return _facebookMode; }
        }

        #region Overrides of Controller

        /// <summary>
        /// Called after the action method is invoked.
        /// </summary>
        /// <param name="filterContext">Information about the current request and action.</param>
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            ViewBag.FacebookMode = FacebookMode;

            base.OnActionExecuted(filterContext);
        }

        #endregion
    }
}