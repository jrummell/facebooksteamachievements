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

namespace SteamAchievements
{
    public partial class PublishDialogTest : Page
    {
        protected string FacebookClientId
        {
            get { return WebConfigurationManager.AppSettings["APIKey"]; }
        }

        protected string FacebookCallbackUrl
        {
            get { return WebConfigurationManager.AppSettings["Callback"]; }
        }

        protected bool IsLoggedIn { get; set; }

        protected string FacebookUrlSuffix
        {
            get { return WebConfigurationManager.AppSettings["Suffix"]; }
        }

        private string FacebookSecret
        {
            get { return WebConfigurationManager.AppSettings["Secret"]; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            HttpCookie cookie = GetCookie();
            IsLoggedIn = cookie != null;
        }

        private HttpCookie GetCookie()
        {
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
                foreach (KeyValuePair<string, string> pair in cookieValues)
                {
                    Response.Write(pair.Key + ": " + pair.Value + "<br/>\n");

                    if (pair.Key != "sig")
                    {
                        payload.Append(pair.Key + "=" + pair.Value);
                    }
                }

                string sig = cookieValues["sig"];
                string hash = GetMd5Hash(payload + FacebookSecret);

                if (sig == hash)
                {
                    return cookie;
                }
            }

            return null;
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