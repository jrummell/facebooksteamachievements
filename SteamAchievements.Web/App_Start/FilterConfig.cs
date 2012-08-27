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
using Elmah.Contrib.Mvc;
using SteamAchievements.Web.Controllers;
using SteamAchievements.Web.Filters;
using SteamAchievements.Web.Helpers;

namespace SteamAchievements.Web.App_Start
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters, IDependencyResolver dependencyResolver)
        {
            filters.Add(new RequestCultureAttribute());
            filters.Add(new ElmahHandleErrorAttribute());

            IFilterProvider provider = new ConditionalFilterProvider(
                new Func<ControllerContext, ActionDescriptor, object>[]
                    {
                        (controller, action) =>
                        controller.Controller.GetType() != typeof (AccountController)
                            ? dependencyResolver.GetService<AuthorizeAttribute>()
                            : null
                    });

            FilterProviders.Providers.Add(provider);
        }
    }
}