using System;
using System.Web;
using SteamAchievements.Services;

namespace SteamAchievements.Admin
{
    public class AutoUpdate : IHttpHandler
    {
        private AutoUpdateLogger _log;
        private AutoUpdateManager _manager;

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                string logPath = context.Server.MapPath("~/App_Data/AutoUpdate");
                _log = new AutoUpdateLogger(logPath);
                _manager = new AutoUpdateManager(_log);

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

                        _log.FlushLog();

                        context.Response.Write(users);
                    }
                    else if (method == "PublishUserAchievements")
                    {
                        string userName = context.Request["user"];

                        _manager.PublishUserAchievements(userName);

                        _log.FlushLog();

                        context.Response.Write(userName + " published.");
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
                _log.WriteLog(context.Response);
            }
            finally
            {
                _log.FlushLog();
                _manager.Dispose();
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}