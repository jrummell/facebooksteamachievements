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
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Web;
using Elmah;

namespace SteamAchievements.Services
{
    /// <summary>
    /// Your handler to actually tell ELMAH about the problem. Based on Will Hughes StackOverflow answer at http://stackoverflow.com/questions/895901/exception-logging-for-wcf-services-using-elmah/906494#906494.
    /// </summary>
    public class HttpErrorHandler : IErrorHandler
    {
        #region IErrorHandler Members

        /// <summary>
        /// Enables error-related processing and returns a value that indicates whether subsequent HandleError implementations are called.
        /// </summary>
        /// <param name="error">The exception thrown during processing.</param>
        /// <returns>
        /// true if subsequent <see cref="T:System.ServiceModel.Dispatcher.IErrorHandler"/> implementations must not be called; otherwise, false. The default is false.
        /// </returns>
        public bool HandleError(Exception error)
        {
            return false;
        }

        /// <summary>
        /// Enables the creation of a custom <see cref="T:System.ServiceModel.FaultException`1"/> that is returned from an exception in the course of a service method.
        /// </summary>
        /// <param name="error">The <see cref="T:System.Exception"/> object thrown in the course of the service operation.</param>
        /// <param name="version">The SOAP version of the message.</param>
        /// <param name="fault">The <see cref="T:System.ServiceModel.Channels.Message"/> object that is returned to the client, or service, in the duplex case.</param>
        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            if (error == null)
            {
                return;
            }

            // Notify ELMAH of the exception.
            HttpContext context = HttpContext.Current;
            if (context == null)
            {
                ErrorLog.GetDefault(null).Log(new Error(error));
            }
            else
            {
                ErrorSignal.FromContext(context).Raise(error, context);
            }
        }

        #endregion
    }
}