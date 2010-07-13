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
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace SteamAchievements.Services
{
    /// <summary>
    /// So we can decorate Services with the [ServiceErrorBehaviour(typeof(HttpErrorHandler))]
    /// ...and errors reported to ELMAH. Based on Will Hughes StackOverflow answer at http://stackoverflow.com/questions/895901/exception-logging-for-wcf-services-using-elmah/906494#906494.
    /// </summary>
    public class ServiceErrorBehaviourAttribute : Attribute, IServiceBehavior
    {
        private readonly Type _errorHandlerType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceErrorBehaviourAttribute"/> class.
        /// </summary>
        /// <param name="errorHandlerType">Type of the error handler.</param>
        public ServiceErrorBehaviourAttribute(Type errorHandlerType)
        {
            _errorHandlerType = errorHandlerType;
        }

        #region IServiceBehavior Members

        /// <summary>
        /// Validates the specified description.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="serviceHostBase">The service host base.</param>
        public void Validate(ServiceDescription description, ServiceHostBase serviceHostBase)
        {
        }

        /// <summary>
        /// Adds the binding parameters.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="serviceHostBase">The service host base.</param>
        /// <param name="endpoints">The endpoints.</param>
        /// <param name="parameters">The parameters.</param>
        public void AddBindingParameters(ServiceDescription description, ServiceHostBase serviceHostBase,
                                         Collection<ServiceEndpoint> endpoints, BindingParameterCollection parameters)
        {
        }

        /// <summary>
        /// Applies the dispatch behavior.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="serviceHostBase">The service host base.</param>
        public void ApplyDispatchBehavior(ServiceDescription description, ServiceHostBase serviceHostBase)
        {
            IErrorHandler errorHandler = (IErrorHandler) Activator.CreateInstance(_errorHandlerType);
            foreach (ChannelDispatcherBase channelDispatcherBase in serviceHostBase.ChannelDispatchers)
            {
                ChannelDispatcher channelDispatcher = (ChannelDispatcher) channelDispatcherBase;
                channelDispatcher.ErrorHandlers.Add(errorHandler);
            }
        }

        #endregion
    }
}