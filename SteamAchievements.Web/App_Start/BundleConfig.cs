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
            canvasScripts.Orderer = new ExplicitBundleOrderer();
            canvasScripts.Include("~/Scripts/modernizr-*",
                                  "~/Scripts/json2.js",
                                  "~/Scripts/columnizer.js",
                                  "~/Scripts/jquery.ui.message.js",
                                  "~/Scripts/Shared/AchievementService.js",
                                  "~/Scripts/Shared/facebook.js",
                                  "~/Scripts/Shared/canvas.js");
            bundles.Add(canvasScripts);

            Bundle mobileScripts =
                new ScriptBundle(BundlePaths.MobileScripts);
            mobileScripts.Orderer = new ExplicitBundleOrderer();
            mobileScripts.Include("~/Scripts/modernizr-*",
                                  "~/Scripts/json2.js",
                                  "~/Scripts/jquery.mobile.message.js",
                                  "~/Scripts/shared/AchievementService.js",
                                  "~/Scripts/Shared/facebook.js",
                                  "~/Scripts/Shared/mobile.js");
            bundles.Add(mobileScripts);

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
                new ScriptBundle(BundlePaths.IndexScripts)
                    .Include("~/Scripts/Home/index.js");
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