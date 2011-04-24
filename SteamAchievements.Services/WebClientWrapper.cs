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
using System.Net;
using System.Web;
using Elmah;

namespace SteamAchievements.Services
{
    public class WebClientWrapper : IWebClientWrapper
    {
        private readonly WebClient _webClient = new WebClient();

        #region IWebClientWrapper Members

        public string DownloadString(Uri url)
        {
            try
            {
                return _webClient.DownloadString(url);
            }
            catch (Exception ex)
            {
                HttpContext context = HttpContext.Current;
                if (context != null)
                {
                    ErrorSignal signal = ErrorSignal.FromContext(context);
                    if (signal != null)
                    {
                        string message = "Could not access url: " + url;
                        Exception exception = new InvalidOperationException(message, ex);
                        signal.Raise(exception);
                    }
                }

                return null;
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        #endregion

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                _webClient.Dispose();
            }
        }
    }
}