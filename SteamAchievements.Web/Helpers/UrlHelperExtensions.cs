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
using System.Text;
using System.Web.Mvc;
using SteamAchievements.Web.Models;
using SteamAchievements.Web.Properties;

namespace SteamAchievements.Web.Helpers
{
    public static class UrlHelperExtensions
    {
        private static readonly Settings _settings = Settings.Default;

        public static string Help(this UrlHelper url, string anchor = null)
        {
            var action = CanvasAction(url, "Index", "Help");
            if (!String.IsNullOrEmpty(anchor))
            {
                action += "#" + anchor;
            }

            return action;
        }

        public static string CanvasAction(this UrlHelper url, string canvasAction, string canvasController = null)
        {
            if (_settings.Mode != FacebookMode.Canvas)
            {
                return url.Action(canvasAction, canvasController ?? "Home");
            }

            string canvasUrl = _settings.FacebookCanvasUrl.ToString();
            StringBuilder canvasLink = new StringBuilder(canvasUrl);
            if (!canvasUrl.EndsWith("/"))
            {
                canvasLink.Append("/");
            }

            if (!String.IsNullOrEmpty(canvasController))
            {
                canvasLink.Append(canvasController).Append("/");
            }

            if (!String.IsNullOrEmpty(canvasAction))
            {
                canvasLink.Append(canvasAction);
            }

            return canvasLink.ToString();
        }
    }
}