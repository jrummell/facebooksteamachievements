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
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using SteamAchievements.Services;
using SteamAchievements.Web.Models;
using SteamAchievements.Web.Properties;

namespace SteamAchievements.Web.Controllers
{
    [ElmahHandleError]
    public class AdminController : Controller
    {
        private static readonly string _autoUpdateAuthKey = Settings.Default.AutoUpdateAuthKey;
        private readonly IAutoUpdateLogger _log;
        private readonly IAutoUpdateManager _manager;
        private readonly StringBuilder _responseLog = new StringBuilder();
        private bool _authorized;

        public AdminController(IAutoUpdateManager manager, IAutoUpdateLogger log)
        {
            _manager = manager;
            _log = log;
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            _log.Log(filterContext.Exception);
            _log.Log(_responseLog.ToString());

            base.OnException(filterContext);
        }

        [HttpGet]
        public ActionResult LogOn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(LogOnViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool authenticated = FormsAuthentication.Authenticate(model.UserName, model.Password);
            if (authenticated)
            {
                FormsAuthentication.SetAuthCookie(model.UserName, true);
                string redirectUrl = FormsAuthentication.GetRedirectUrl(model.UserName, true);

                return Redirect(redirectUrl);
            }

            return View(model);
        }

        /// <summary>
        /// Publishes the user achievements.
        /// </summary>
        /// <param name="authKey">The auth key.</param>
        /// <param name="steamUserId">The steam user id.</param>
        /// <returns></returns>
        public ContentResult PublishUserAchievements(string authKey, string steamUserId)
        {
            if (VerifyAuthKey(authKey))
            {
                _manager.PublishUserAchievements(steamUserId);

                _log.Flush();

                _responseLog.AppendLine(steamUserId + " published.");

                // delete logs more than two weeks old
                _log.Delete(DateTime.UtcNow.AddDays(-14).Date);
            }

            return Content(_responseLog.ToString());
        }

        /// <summary>
        /// Gets the auto update users.
        /// </summary>
        /// <param name="authKey">The auth key.</param>
        /// <returns></returns>
        public ContentResult GetAutoUpdateUsers(string authKey)
        {
            if (VerifyAuthKey(authKey))
            {
                _log.Log("Getting auto update users");

                string users = _manager.GetAutoUpdateUsers();

                _log.Log(users);

                _log.Flush();

                _responseLog.Append(users);
            }

            return Content(_responseLog.ToString());
        }

        /// <summary>
        /// Verifies the auth key.
        /// </summary>
        /// <param name="authKey">The auth key.</param>
        /// <returns></returns>
        private bool VerifyAuthKey(string authKey)
        {
            _authorized = _autoUpdateAuthKey == authKey;

            if (!_authorized)
            {
                _log.Log("Invalid auth key");
                _responseLog.AppendLine("Invalid auth key");
            }

            return _authorized;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _manager.Dispose();
                _log.Flush();
            }

            base.Dispose(disposing);
        }
    }
}