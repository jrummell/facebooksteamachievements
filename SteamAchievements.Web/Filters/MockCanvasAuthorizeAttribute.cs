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

using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SteamAchievements.Services;
using SteamAchievements.Services.Models;

namespace SteamAchievements.Web.Filters
{
    public class MockCanvasAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly MockUserService _userService = new MockUserService();

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            User user = _userService.GetUser(1234);
            httpContext.Session[CanvasAuthorizeAttribute.UserSettingsKey] = user;

            return true;
        }
    }
}