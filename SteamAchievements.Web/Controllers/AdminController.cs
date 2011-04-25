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
using System.Web.Security;
using SteamAchievements.Web.Models;

namespace SteamAchievements.Web.Controllers
{
    [ElmahHandleError]
    public class AdminController : Controller
    {
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
    }
}