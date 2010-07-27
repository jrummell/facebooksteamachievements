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
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;

namespace SteamAchievements
{
    public partial class PublishDialogTest : Page
    {
        //TODO: use the example at http://developers.facebook.com/docs/guides/canvas/#canvas for authentication
        
        protected string FacebookClientId
        {
            get { return WebConfigurationManager.AppSettings["APIKey"]; }
        }

        protected string FacebookCallbackUrl
        {
            get { return WebConfigurationManager.AppSettings["Callback"]; }
        }

        protected bool IsLoggedIn { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            HttpCookie cookie = GetCookie();
            IsLoggedIn = cookie != null;
        }

        private HttpCookie GetCookie()
        {
            // based on the php example at http://developers.facebook.com/docs/authentication/
            HttpCookie cookie = Request.Cookies["fbs_" + FacebookClientId];
            StringBuilder payload = new StringBuilder();
            if (cookie != null)
            {
                foreach (string key in cookie.Values.Keys)
                {
                    if (key != "sig")
                    {
                        payload.Append(key + "=" + cookie.Values[key]);
                    }
                }

                string sig = cookie.Values["sig"];

                if (sig == GetMD5Hash(payload.ToString()))
                {
                    return cookie;
                }
            }

            return null;
        }

        public string GetMD5Hash(string input)
        {
            MD5CryptoServiceProvider cryptoServiceProvider = new MD5CryptoServiceProvider();
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            bytes = cryptoServiceProvider.ComputeHash(bytes);
            StringBuilder s = new StringBuilder();

            foreach (byte b in bytes)
            {
                s.Append(b.ToString("x2").ToLower());
            }

            return s.ToString();
        }
    }
}