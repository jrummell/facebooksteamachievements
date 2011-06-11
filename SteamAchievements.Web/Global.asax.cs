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

using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Practices.Unity;
using SteamAchievements.Web.Bootstrapping;
using SteamAchievements.Web.Models;

namespace SteamAchievements.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        private static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ElmahHandleErrorAttribute());
        }

        private static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "DefaultController", // Route name
                "{action}", // URL with parameters
                new {controller = "Home", action = "Index"} // Parameter defaults
                );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new {controller = "Home", action = "Index", id = UrlParameter.Optional} // Parameter defaults
                );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            RegisterDependencyResolver();

            RegisterMappings();
        }

        private static void RegisterMappings()
        {
            ModelMapCreator mapCreator = new ModelMapCreator();
            mapCreator.CreateMap();
        }

        private static void RegisterDependencyResolver()
        {
            UnityContainer unityContainer = new UnityContainer();
            UnityRegistration unityRegistration = new UnityRegistration(unityContainer);
            unityRegistration.Register();

            IDependencyResolver unityDependencyResolver = new UnityDependencyResolver(unityContainer);
            DependencyResolver.SetResolver(unityDependencyResolver);

            IControllerFactory controllerFactory = new DependencyResolverControllerFactory(unityDependencyResolver);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
        }
    }
}