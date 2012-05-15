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

using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using SteamAchievements.Services;
using SteamAchievements.Services.Models;
using SteamAchievements.Web.Models;

namespace SteamAchievements.Web.Controllers
{
    public class AccountController : FacebookController
    {
        private readonly IFacebookClientService _facebookClient;

        public AccountController(IUserService userService, IFacebookClientService facebookClient)
            : base(userService)
        {
            _facebookClient = facebookClient;
        }

        public ActionResult LogOn()
        {
            //TODO: LogOn view
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(LogOnViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return LogOnResult(model);
            }

            string userName = model.FacebookUserId.ToString();

            long facebookUserId = _facebookClient.GetUserId(model.AccessToken);

            if (facebookUserId == 0)
            {
                ModelState.AddModelError("AccessToken", "Invalid AccessToken");
            }

            if (facebookUserId != model.FacebookUserId)
            {
                ModelState.AddModelError("FacebookUserId", "Invalid FacebookUserId.");
            }

            User user = UserService.GetUser(model.FacebookUserId) ?? new User();
            user.AccessToken = model.AccessToken;

            UserService.UpdateUser(user);
            UserSettings = user;

            if (!User.Identity.IsAuthenticated)
            {
                FormsAuthentication.SetAuthCookie(userName, false);
            }
            else if (User.Identity.Name != userName)
            {
                FormsAuthentication.SignOut();
                FormsAuthentication.SetAuthCookie(userName, false);
            }

            return LogOnResult(model);
        }

        private ActionResult LogOnResult(LogOnViewModel model)
        {
            string redirectUrl = Url.Action("Index", "Home");
            StringBuilder message = new StringBuilder();

            foreach (var error in ModelState)
            {
                foreach (var propertyError in error.Value.Errors)
                {
                    message.AppendLine(propertyError.ErrorMessage);
                }
            }

            if (Request.IsAjaxRequest())
            {
                return Json(new
                                {
                                    redirectUrl,
                                    isValid = ModelState.IsValid,
                                    message = message.ToString()
                                });
            }
            else
            {
                if (ModelState.IsValid)
                {
                    return Redirect(redirectUrl);
                }
                else
                {
                    return View("LogOn", model);
                }
            }
        }
    }
}