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
using System.Web.Mvc;
using Elmah;

namespace SteamAchievements.Web.Models
{
    public class ElmahHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);

            Exception e = context.Exception;
            if (!context.ExceptionHandled // if unhandled, will be logged anyhow
                || RaiseErrorSignal(e) // prefer signaling, if possible
                || IsFiltered(context)) // filtered?
            {
                return;
            }

            LogException(e);
        }

        private static bool RaiseErrorSignal(Exception e)
        {
            HttpContext context = HttpContext.Current;
            if (context == null)
            {
                return false;
            }
            ErrorSignal signal = ErrorSignal.FromContext(context);
            if (signal == null)
            {
                return false;
            }
            signal.Raise(e, context);
            return true;
        }

        private static bool IsFiltered(ExceptionContext context)
        {
            ErrorFilterConfiguration config = context.HttpContext.GetSection("elmah/errorFilter")
                                              as ErrorFilterConfiguration;

            if (config == null)
            {
                return false;
            }

            ErrorFilterModule.AssertionHelperContext testContext = new ErrorFilterModule.AssertionHelperContext(
                context.Exception, HttpContext.Current);

            return config.Assertion.Test(testContext);
        }

        private static void LogException(Exception e)
        {
            HttpContext context = HttpContext.Current;
            ErrorLog.GetDefault(context).Log(new Error(e, context));
        }
    }
}