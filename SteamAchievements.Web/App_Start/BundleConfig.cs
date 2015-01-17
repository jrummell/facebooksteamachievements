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

using System.Web.Optimization;
using SteamAchievements.Web.Helpers;

namespace SteamAchievements.Web.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // shared scripts
            Bundle canvasScripts =
                new ScriptBundle(BundlePaths.CanvasScripts);
            canvasScripts.Include("~/Scripts/modernizr-*",
                                  "~/Scripts/json2.js",
                                  "~/Scripts/bootstrap.message.js",
                                  "~/Scripts/Shared/AchievementService.js",
                                  "~/Scripts/toc.js",
                                  "~/Scripts/Shared/site.js");
            bundles.Add(canvasScripts);

            Bundle facebookScripts 
                = new ScriptBundle(BundlePaths.FacebookScripts);
            facebookScripts.Include("~/Scripts/Shared/facebook.js");
            bundles.Add(facebookScripts);

            // css themes
            Bundle canvasTheme =
                new StyleBundle(BundlePaths.CanvasTheme)
                    .IncludeDirectory("~/Content/themes/canvas", "*.css");
            bundles.Add(canvasTheme);

            Bundle mobileTheme =
                new StyleBundle(BundlePaths.MobileTheme)
                    .IncludeDirectory("~/Content/themes/mobile", "*.css");
            bundles.Add(mobileTheme);

            // view specific scripts
            Bundle indexScripts =
                new ScriptBundle(BundlePaths.GamesScripts)
                    .Include("~/Scripts/Home/games.js");
            bundles.Add(indexScripts);

            Bundle publishScripts =
                new ScriptBundle(BundlePaths.PublishScripts)
                    .Include("~/Scripts/Home/publish.js");
            bundles.Add(publishScripts);

            Bundle settingsScripts =
                new ScriptBundle(BundlePaths.SettingsScripts)
                    .Include("~/Scripts/Home/settings.js");
            bundles.Add(settingsScripts);
        }
    }
}