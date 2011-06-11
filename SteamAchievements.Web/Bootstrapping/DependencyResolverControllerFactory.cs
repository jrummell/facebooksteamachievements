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
using System.Web.Mvc;
using System.Web.Routing;

namespace SteamAchievements.Web.Bootstrapping
{
    public class DependencyResolverControllerFactory : DefaultControllerFactory
    {
        private readonly IDependencyResolver _dependencyResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyResolverControllerFactory"/> class.
        /// </summary>
        /// <param name="dependencyResolver">The dependency resolver.</param>
        public DependencyResolverControllerFactory(IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
        }

        /// <summary>
        /// Gets the controller instance.
        /// </summary>
        /// <param name="requestContext">The context of the HTTP request, which includes the HTTP context and route data.</param>
        /// <param name="controllerType">Type of the controller.</param>
        /// <returns>
        /// The controller instance.
        /// </returns>
        /// <exception cref="T:System.Web.HttpException">
        ///   <paramref name="controllerType"/> is null.</exception>
        ///   
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="controllerType"/> cannot be assigned.</exception>
        ///   
        /// <exception cref="T:System.InvalidOperationException">An instance of <paramref name="controllerType"/> cannot be created.</exception>
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                throw new ArgumentNullException("controllerType");
            }

            if (!typeof (IController).IsAssignableFrom(controllerType))
            {
                throw new ArgumentException(string.Format(
                    "Type requested is not a controller: {0}", controllerType.Name),
                                            "controllerType");
            }

            return _dependencyResolver.GetService(controllerType) as IController;
        }
    }
}