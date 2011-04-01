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
using System.Web;
using Microsoft.Practices.Unity;
using SteamAchievements.Services;

namespace SteamAchievements.Admin
{
    public class AutoUpdate : IHttpHandler
    {
        private AutoUpdateLogger _log;
        private AutoUpdateManager _manager;

        #region IHttpHandler Members

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                string logPath = context.Server.MapPath("~/App_Data/AutoUpdate");
                _log = new AutoUpdateLogger(logPath);

                IUnityContainer container = ContainerManager.Container;
                _manager = new AutoUpdateManager(container.Resolve<IAchievementService>(),
                                                 container.Resolve<IUserService>(),
                                                 container.Resolve<IFacebookPublisher>(), _log);

                bool authorized = context.Request["auth"] == Properties.Settings.Default.AutoUpdateAuthKey;
                if (!authorized)
                {
                    _log.Log("Invalid auth key");
                    context.Response.Write("Invalid auth key");
                }
                else
                {
                    string method = context.Request["method"];

                    if (method == "GetAutoUpdateUsers")
                    {
                        _log.Log("Getting auto update users");

                        string users = _manager.GetAutoUpdateUsers();

                        _log.Log(users);

                        _log.Flush();

                        context.Response.Write(users);
                    }
                    else if (method == "PublishUserAchievements")
                    {
                        string userName = context.Request["user"];

                        _manager.PublishUserAchievements(userName);

                        _log.Flush();

                        context.Response.Write(userName + " published.");

                        // delete logs more than two weeks old
                        _log.Delete(DateTime.UtcNow.AddDays(-14).Date);
                    }
                    else
                    {
                        context.Response.Write("Invalid method");
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Log(ex);
                _log.Write(context.Response);
            }
            finally
            {
                _manager.Dispose();
                _log.Flush();
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }

        #endregion
    }
}