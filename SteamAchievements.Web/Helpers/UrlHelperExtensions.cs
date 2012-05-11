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
using SteamAchievements.Web.Properties;

namespace SteamAchievements.Web.Helpers
{
    public static class UrlHelperExtensions
    {
        private static readonly Settings _settings = Settings.Default;

        public static MvcHtmlString CanvasAction(this UrlHelper url, string canvasAction)
        {
            return MvcHtmlString.Create(GetCanvasUrl(canvasAction));
        }

        public static MvcHtmlString Help(this UrlHelper url, string anchor = null)
        {
            if (anchor == null)
            {
                return MvcHtmlString.Create(_settings.HelpUrl.ToString());
            }

            return MvcHtmlString.Create(String.Format("{0}#{1}", _settings.HelpUrl, anchor));
        }

        public static string GetCanvasUrl(string canvasAction)
        {
            string canvasUrl = _settings.FacebookCanvasUrl.ToString();
            StringBuilder canvasLink = new StringBuilder(canvasUrl);
            if (!canvasUrl.EndsWith("/"))
            {
                canvasLink.Append("/");
            }

            if (!String.IsNullOrEmpty(canvasAction))
            {
                canvasLink.Append(canvasAction);
            }

            return canvasLink.ToString();
        }
    }
}