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
using SteamAchievements.Services.Properties;

namespace SteamAchievements.Services
{
    /// <summary>
    /// Description of FacebookCookieParser.
    /// </summary>
    public class FacebookCookieParser
    {
        private readonly HttpRequest _request;
        private readonly HttpServerUtility _serverUtility;

        public FacebookCookieParser(HttpContext context)
        {
            _serverUtility = context.Server;
            _request = context.Request;
        }

        /// <summary>
        /// Gets the logged in user's id from the facebook cookie.
        /// </summary>
        /// <returns>The logged in user's id</returns>
        public long GetUserId()
        {
            // IE requires a P3P policy since we're accessing a cookie from a different domain. See http://code.google.com/p/facebooksteamachievements/issues/detail?id=53

            // based on the php example at http://developers.facebook.com/docs/guides/canvas/#canvas
            HttpCookie cookie = _request.Cookies["fbs_" + ServiceSettings.APIKey];
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
                    pairs.ToDictionary(pair => pair.Key, pair => _serverUtility.UrlDecode(pair.Value));

                StringBuilder payload = new StringBuilder();
                foreach (KeyValuePair<string, string> pair in cookieValues)
                {
                    if (pair.Key != "sig")
                    {
                        payload.Append(pair.Key + "=" + pair.Value);
                    }
                }

                string sig = cookieValues["sig"];
                string hash = GetMd5Hash(payload + ServiceSettings.ApplicationSecret);

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