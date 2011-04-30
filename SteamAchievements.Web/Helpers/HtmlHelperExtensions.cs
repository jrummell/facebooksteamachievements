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
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using SteamAchievements.Web.Properties;

namespace SteamAchievements.Web.Helpers
{
    public static class HtmlHelperExtensions
    {
        private static readonly Settings _settings = Settings.Default;

        public static MvcHtmlString HelpLink(this HtmlHelper html, string linkText, string anchor = null,
                                             object htmlAttributes = null)
        {
            string link = String.Format("{0}#{1}", _settings.HelpUrl, anchor ?? String.Empty);

            TagBuilder a = new TagBuilder("a");
            a.MergeAttribute("href", link);
            a.MergeAttribute("target", "_blank");
            a.AddCssClass("help");
            a.SetInnerText(linkText);

            if (htmlAttributes != null)
            {
                a.MergeAttributes(new RouteValueDictionary(htmlAttributes), false);
            }

            return MvcHtmlString.Create(a.ToString());
        }

        public static MvcHtmlString CanvasLink(this HtmlHelper html, string linkText, string canvasAction = null,
                                               object htmlAttributes = null)
        {
            TagBuilder a = new TagBuilder("a");
            a.MergeAttribute("href", UrlHelperExtensions.GetCanvasUrl(canvasAction));
            a.MergeAttribute("target", "_top");
            a.SetInnerText(linkText);

            if (htmlAttributes != null)
            {
                a.MergeAttributes(new RouteValueDictionary(htmlAttributes), false);
            }

            return MvcHtmlString.Create(a.ToString());
        }

        public static MvcHtmlString Ad(this HtmlHelper html, AdLocation location)
        {
            return MvcHtmlString.Create(location == AdLocation.Top ? _settings.TopAdMarkup : _settings.BottomAdMarkup);
        }

        public static MvcHtmlString UserVoice(this HtmlHelper html)
        {
            return MvcHtmlString.Create(_settings.UserVoiceMarkup);
        }

        public static MvcHtmlString Disclaimer(this HtmlHelper html)
        {
            return MvcHtmlString.Create(_settings.DisclaimerMarkup);
        }

        public static MvcHtmlString Version(this HtmlHelper html)
        {
            return MvcHtmlString.Create(Assembly.GetExecutingAssembly().GetName().Version.ToString());
        }
    }

    public enum AdLocation
    {
        Top,
        Bottom
    }
}