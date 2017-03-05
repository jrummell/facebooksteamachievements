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
                                  "~/Scripts/bootstrap.message.js");
            bundles.Add(canvasScripts);

            bundles.Add(new ScriptBundle(BundlePaths.SharedScripts)
                            .Include("~/Scripts/dist/shared.js", "~/Scripts/dist/app.bundle.js"));

            bundles.Add(new ScriptBundle(BundlePaths.GamesScripts)
                            .Include("~/Scripts/dist/games.bundle.js"));

            bundles.Add(new ScriptBundle(BundlePaths.PublishScripts)
                            .Include("~/Scripts/dist/publish.bundle.js"));

            bundles.Add(new ScriptBundle(BundlePaths.SettingsScripts)
                            .Include("~/Scripts/dist/settings.bundle.js"));

            Bundle facebookScripts 
                = new ScriptBundle(BundlePaths.FacebookScripts);
            facebookScripts.Include("~/Scripts/facebook.js");
            bundles.Add(facebookScripts);

            // css themes
            Bundle siteTheme =
                new StyleBundle(BundlePaths.Styles)
                    .IncludeDirectory("~/Content/theme", "*.css");
            bundles.Add(siteTheme);
        }
    }
}