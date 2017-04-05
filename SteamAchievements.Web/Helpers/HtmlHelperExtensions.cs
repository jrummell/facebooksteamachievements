#region License

//  Copyright 2012 John Rummell
//  
//  This file is part of SteamAchievements.
//  
//      SteamAchievements is free software: you can redistribute it and/or modify
//      it under the terms of the GNU General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      SteamAchievements is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU General Public License for more details.
//  
//      You should have received a copy of the GNU General Public License
//      along with SteamAchievements.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using SteamAchievements.Web.Models;
using SteamAchievements.Web.Properties;

namespace SteamAchievements.Web.Helpers
{
    public static class HtmlHelperExtensions
    {
        private static readonly Settings _settings = Settings.Default;

        public static MvcHtmlString CanvasLink(this HtmlHelper html, string linkText, string canvasAction = null,
                                               string canvasController = null,
                                               object htmlAttributes = null)
        {
            UrlHelper url = new UrlHelper(html.ViewContext.RequestContext);
            TagBuilder a = new TagBuilder("a");
            a.MergeAttribute("href", url.CanvasAction(canvasAction, canvasController));
            a.MergeAttribute("target", "_top");
            a.SetInnerText(linkText);

            if (htmlAttributes != null)
            {
                a.MergeAttributes(new RouteValueDictionary(htmlAttributes), false);
            }

            return MvcHtmlString.Create(a.ToString());
        }

        public static MvcHtmlString HelpButton(this HtmlHelper html, string linkText, string anchor = null,
                                               object htmlAttributes = null)
        {
            string link = Settings.Default.HelpUrl + "#" + anchor;

            TagBuilder icon = new TagBuilder("span");
            icon.AddCssClass("glyphicon glyphicon-question-sign");
            TagBuilder a = new TagBuilder("a");
            a.MergeAttribute("href", link);
            a.MergeAttribute("target", "_blank");
            a.AddCssClass("btn btn-default");
            a.SetInnerText(linkText);

            if (htmlAttributes != null)
            {
                a.MergeAttributes(new RouteValueDictionary(htmlAttributes), false);
            }

            a.InnerHtml = icon.ToString() + linkText;

            return MvcHtmlString.Create(a.ToString());
        }

        public static MvcHtmlString HelpLink(this HtmlHelper html, string linkText, string anchor = null,
                                             object htmlAttributes = null)
        {
            string link = Settings.Default.HelpUrl + "#" + anchor;

            TagBuilder a = new TagBuilder("a");
            a.MergeAttribute("href", link);
            a.MergeAttribute("target", "_blank");
            a.AddCssClass("");
            a.SetInnerText(linkText);

            if (htmlAttributes != null)
            {
                a.MergeAttributes(new RouteValueDictionary(htmlAttributes), false);
            }

            return MvcHtmlString.Create(a.ToString());
        }

        public static MvcHtmlString FacebookAppId(this HtmlHelper html)
        {
            return MvcHtmlString.Create(_settings.FacebookAppId);
        }

        public static MvcHtmlString ChannelUrl(this HtmlHelper html)
        {
            string channelPath = VirtualPathUtility.ToAbsolute("~/fbchannel.ashx");

            return MvcHtmlString.Create(channelPath);
        }

        public static MvcHtmlString Ad(this HtmlHelper html, AdLocation location)
        {
            return MvcHtmlString.Create(location == AdLocation.Top ? _settings.TopAdMarkup : _settings.BottomAdMarkup);
        }

        public static MvcHtmlString Analytics(this HtmlHelper html)
        {
            return MvcHtmlString.Create(_settings.AnalyticsMarkup);
        }

        public static MvcHtmlString IssueTracker(this HtmlHelper html)
        {
            return MvcHtmlString.Create(_settings.IssueTrackerMarkup);
        }

        public static MvcHtmlString Disclaimer(this HtmlHelper html)
        {
            return MvcHtmlString.Create(_settings.DisclaimerMarkup);
        }

        public static MvcHtmlString Version(this HtmlHelper html)
        {
            return MvcHtmlString.Create(Assembly.GetExecutingAssembly().GetName().Version.ToString(3));
        }
    }

    public enum AdLocation
    {
        Top,
        Bottom
    }
}