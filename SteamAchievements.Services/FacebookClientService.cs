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
using Facebook;
using SteamAchievements.Services.Models;

namespace SteamAchievements.Services
{
    public class FacebookClientService : IFacebookClientService
    {
        private readonly string _appSecret;
        private readonly string _appId;

        public FacebookClientService(string appId, string appSecret)
        {
            _appId = appId;
            _appSecret = appSecret;
        }

        /// <summary>
        /// Get's the facebook user id.
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public long GetUserId(string accessToken)
        {
            FacebookClient client = new FacebookClient(accessToken);
            dynamic user = client.Get("me", new { fields = "name,id" });

            if (user == null)
            {
                return 0;
            }

            return user.id;
        }

        /// <summary>
        /// Publishes a post to the user's profile.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="parameters">The post parameters.</param>
        public void Publish(User user, IDictionary<string, object> parameters)
        {
            FacebookClient client = new FacebookClient(user.AccessToken);
            string userFeedPath = String.Format("/{0}/feed/", user.FacebookUserId);
            client.Post(userFeedPath, parameters);
        }

        public object ParseSignedRequest(string signedRequest)
        {
            FacebookClient client = new FacebookClient{AppId = _appId, AppSecret = _appSecret};

            return client.ParseSignedRequest(signedRequest);
        }
    }

    public class MockFacebookClientService : IFacebookClientService
    {
        public long GetUserId(string accessToken)
        {
            return 1;
        }

        public void Publish(User user, IDictionary<string, object> parameters)
        {
            
        }

        public object ParseSignedRequest(string signedRequest)
        {
            return new { oauth_token = "1234abcd", user_id = 1234 };
        }
    }
}
