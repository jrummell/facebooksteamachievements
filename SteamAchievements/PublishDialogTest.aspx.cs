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
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using SteamAchievements.Data;
using SteamAchievements.Properties;
using SteamAchievements.Services.Properties;

namespace SteamAchievements
{
    public partial class PublishDialogTest : Page
    {
        protected string FacebookClientId
        {
            get { return ServiceSettings.APIKey; }
        }

        protected string FacebookCallbackUrl
        {
            get { return Settings.Default.CanvasUrl.ToString(); }
        }

        protected bool IsLoggedIn { get { return FacebookUserId > 0; } }

        protected string FacebookUrlSuffix
        {
            get { return Settings.Default.CanvasPageUrlSuffix; }
        }

        private string FacebookSecret
        {
            get { return ServiceSettings.ApplicationSecret; }
        }

        protected long FacebookUserId { get; private set; }

        protected string SteamUserId { get; private set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FacebookUserId = GetFacebookUserId();

            if (IsLoggedIn)
            {
                using (AchievementManager manager = new AchievementManager())
                {
                    SteamUserId = manager.GetSteamUserId(FacebookUserId);
                }
            }
        }

        private long GetFacebookUserId()
        {
            //TODO: why doesn't this work in IE8?

            // based on the php example at http://developers.facebook.com/docs/guides/canvas/#canvas
            HttpCookie cookie = Request.Cookies["fbs_" + FacebookClientId];
            if (cookie != null)
            {
                var pairs = from pair in cookie.Value.Trim('"', '\\').Split('&')
                            let indexOfEquals = pair.IndexOf('=')
                            orderby pair
                            select new
                                       {
                                           Key = pair.Substring(0, indexOfEquals).Trim(),
                                           Value = pair.Substring(indexOfEquals + 1).Trim()
                                       };

                IDictionary<string, string> cookieValues =
                    pairs.ToDictionary(pair => pair.Key, pair => Server.UrlDecode(pair.Value));

                StringBuilder payload = new StringBuilder();
                StringBuilder placeHolder = new StringBuilder();
                foreach (KeyValuePair<string, string> pair in cookieValues)
                {
                    placeHolder.AppendLine(pair.Key + ": " + pair.Value);

                    if (pair.Key != "sig")
                    {
                        payload.Append(pair.Key + "=" + pair.Value);
                    }
                }

                cookieValuesPlaceHolder.Controls.Add(new LiteralControl(placeHolder.ToString()));

                string sig = cookieValues["sig"];
                string hash = GetMd5Hash(payload + FacebookSecret);

                if (sig == hash)
                {
                    return Convert.ToInt64(cookieValues["uid"]);
                }
            }

            return 0;
        }

        private static string GetMd5Hash(string input)
        {
            MD5CryptoServiceProvider cryptoServiceProvider = new MD5CryptoServiceProvider();
            byte[] bytes = Encoding.Default.GetBytes(input);
            byte[] hash = cryptoServiceProvider.ComputeHash(bytes);

            StringBuilder s = new StringBuilder();
            foreach (byte b in hash)
            {
                s.Append(b.ToString("x2"));
            }

            return s.ToString();
        }
    }
}